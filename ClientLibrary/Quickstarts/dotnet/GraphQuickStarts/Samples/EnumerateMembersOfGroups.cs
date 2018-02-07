using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GraphQuickStarts.Samples
{
    class EnumerateMembersOfGroups
    {
        readonly string _uri;
        readonly Guid _clientId;
        readonly Uri _replyUrl;

        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798"; //Constant value to target VSTS. Do not change
        internal const string GraphResourceId = "https://graph.microsoft.com";         //Constant value to target Microsoft Graph API. Do not change

        /// <summary>
        /// Constructor. Manaully set values to match your account.
        /// </summary>
        public EnumerateMembersOfGroups()
        {
            _uri = "https://accountname.vssps.visualstudio.com";
            _clientId = new Guid("XXXXXXX -XXXX-XXXX-XXXX-XXXXXXXXXXXX");
            _replyUrl = new Uri("http://MyAppUrl");
        }

        public EnumerateMembersOfGroups(string url, string clientId, string redirectURL)
        {
            _uri = url;
            _clientId = new Guid(clientId);
            _replyUrl = new Uri(redirectURL);
        }

        public List<string> RunEnumerateMembersOfGroupsUsingClientLib(string groupDisplayName)
        {
            Uri uri = new Uri(_uri);
            AuthenticationContext ctx = GetAuthenticationContext(null);
            AuthenticationResult vstsAuthResult = ctx.AcquireTokenAsync(VSTSResourceId, _clientId.ToString(), _replyUrl, new PlatformParameters(PromptBehavior.Always)).Result;
            VssConnection vssConnection = new VssConnection(new Uri(_uri), new VssOAuthAccessTokenCredential(vstsAuthResult.AccessToken));

            using (GraphHttpClient graphClient = vssConnection.GetClient<GraphHttpClient>())
            {
                // Get the VSTS group
                GraphGroup group = GetVSTSGroupByDisplayName(graphClient, groupDisplayName);

                // Expand membership of the VSTS group to users and AAD Groups
                GroupMemberships groupMemberships = ExpandVSTSGroup(graphClient, group);

                List<string> expandedUsers = new List<string>();
                foreach (GraphUser user in groupMemberships.Users)
                {
                    expandedUsers.Add(user.PrincipalName);
                }

                //exchange VSTS token for Microsoft graph token
                AuthenticationResult graphAuthResult = ctx.AcquireTokenAsync(GraphResourceId, _clientId.ToString(), _replyUrl, new PlatformParameters(PromptBehavior.Auto)).Result;

                // Resolve all AAD Groups to users using Microsoft graph
                foreach (GraphGroup AADGroup in groupMemberships.AADGroups)
                {
                    List<AadGroupMember> aadGroupUsers = ExpandAadGroups(graphAuthResult.AccessToken, AADGroup);
                    foreach (AadGroupMember aadGroupUser in aadGroupUsers)
                    {
                        expandedUsers.Add(aadGroupUser.userPrincipalName);
                    }
                }

                return expandedUsers;
            }
        }

        #region ADAL helpers
        private static AuthenticationContext GetAuthenticationContext(string tenant)
        {
            AuthenticationContext ctx = null;
            if (tenant != null)
                ctx = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            else
            {
                ctx = new AuthenticationContext("https://login.windows.net/common");
                if (ctx.TokenCache.Count > 0)
                {
                    string homeTenant = ctx.TokenCache.ReadItems().First().TenantId;
                    ctx = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
                }
            }
            return ctx;
        }
        #endregion  

        #region VSTS Graph helpers
        private static GraphGroup GetVSTSGroupByDisplayName(GraphHttpClient graphClient, string groupDisplayName)
        {
            PagedGraphGroups groups = graphClient.GetGroupsAsync().Result;

            GraphGroup selectedGroup = null;
            foreach (var group in groups.GraphGroups)
            {
                if (group.DisplayName.Equals(groupDisplayName))
                {
                    return selectedGroup = group;
                }
            }
            return null;
        }

        private static GroupMemberships ExpandVSTSGroup(GraphHttpClient graphClient, GraphGroup group)
        {
            GroupMemberships groupMemberships = new GroupMemberships();

            // Convert all memberships into GraphSubjectLookupKeys
            List<GraphSubjectLookupKey> lookupKeys = new List<GraphSubjectLookupKey>();
            List<GraphMembership> memberships = graphClient.GetMembershipsAsync(group.Descriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down).Result;
            foreach (var membership in memberships)
            {
                lookupKeys.Add(new GraphSubjectLookupKey(membership.MemberDescriptor));
            }
            IReadOnlyDictionary<SubjectDescriptor, GraphSubject> subjectLookups = graphClient.LookupSubjectsAsync(new GraphSubjectLookup(lookupKeys)).Result;
            foreach (GraphSubject subject in subjectLookups.Values)
            {
                switch (subject.Descriptor.SubjectType)
                {
                    //member is an AAD user
                    case Constants.SubjectType.AadUser:
                        groupMemberships.AddUser((GraphUser)subject);
                        break;

                    //member is an MSA user
                    case Constants.SubjectType.MsaUser:
                        groupMemberships.AddUser((GraphUser)subject);
                        break;

                    //member is a nested AAD group
                    case Constants.SubjectType.AadGroup:
                        groupMemberships.AddAADGroup((GraphGroup)subject);
                        break;

                    //member is a nested VSTS group
                    case Constants.SubjectType.VstsGroup:
                        GroupMemberships subGroupMemberships = ExpandVSTSGroup(graphClient, (GraphGroup)subject);
                        groupMemberships.Add(subGroupMemberships);
                        break;

                    default:
                        throw new Exception("Unknown SubjectType: " + subject.Descriptor.SubjectType);
                }
            }

            return groupMemberships;
        }
        #endregion

        #region Microsoft Graph helpers
        private static List<AadGroupMember> ExpandAadGroups(string accessToken, GraphGroup group)
        {
            //List of users in an AAD group
            List<AadGroupMember> aadUsers = new List<AadGroupMember>();

            //Getting all members in all groups and nesteed groups
            List<AadGroupMember> members = new List<AadGroupMember>();
            members.AddRange(GetAADGroupMembers(accessToken, group.OriginId));
            while (members.Count != 0)
            {
                List<AadGroupMember> nestedGroups = new List<AadGroupMember>();
                foreach (var aadMember in members)
                {
                    switch (aadMember.type)
                    {
                        //member is a user
                        case "#microsoft.graph.user":
                            aadUsers.Add(aadMember);
                            break;
                        //member is a nested AAD group
                        case "#microsoft.graph.group":
                            nestedGroups.AddRange(GetAADGroupMembers(accessToken, aadMember.id));
                            break;
                        default:
                            throw new Exception("shouldn't be here");
                    }
                }
                members.Clear();
                members.AddRange(nestedGroups);
            }
            return aadUsers;
        }

        private static List<AadGroupMember> GetAADGroupMembers(string accessToken, string aadGroupId)
        {
            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", accessToken);
            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.microsoft.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "GraphGroupMembershipSample");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("v1.0/groups/" + aadGroupId + "/members").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    string responseJsonStr = response.Content.ReadAsStringAsync().Result;
                    AadGroupMembers groupMembers = JsonConvert.DeserializeObject<AadGroupMembers>(responseJsonStr);
                    return groupMembers.members;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        #endregion
    }

    public class GroupMemberships
    {
        public List<GraphUser> Users;
        public List<GraphGroup> AADGroups;

        public GroupMemberships()
        {
            Users = new List<GraphUser>();
            AADGroups = new List<GraphGroup>();
        }

        public void Add(GroupMemberships memberships)
        {
            this.Users.AddRange(memberships.Users);
            this.AADGroups.AddRange(memberships.AADGroups);
        }

        public void AddUser(GraphUser user)
        {
            this.Users.Add(user);
        }

        public void AddAADGroup(GraphGroup group)
        {
            this.AADGroups.Add(group);
        }
    }

    #region JSON deserialization
    public class AadGroupMembers
    {
        [JsonProperty("@odata.context")]
        public string groupType { get; set; }
        [JsonProperty("value")]
        public List<AadGroupMember> members { get; set; }
    }
    public class AadGroupMember
    {
        [JsonProperty("@odata.type")]
        public string type { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("businessPhones")]
        public List<string> businessPhones { get; set; }
        [JsonProperty("displayName")]
        public string displayName { get; set; }
        [JsonProperty("givenName")]
        public string givenName { get; set; }
        [JsonProperty("jobTitle")]
        public string jobTitle { get; set; }
        [JsonProperty("mail")]
        public string mail { get; set; }
        [JsonProperty("mobilePhone")]
        public string mobilePhone { get; set; }
        [JsonProperty("officeLocation")]
        public string officeLocation { get; set; }
        [JsonProperty("preferredLanguage")]
        public string preferredLanguage { get; set; }
        [JsonProperty("surname")]
        public string surname { get; set; }
        [JsonProperty("userPrincipalName")]
        public string userPrincipalName { get; set; }
    }
    #endregion
}
