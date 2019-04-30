using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.Users.UsersResourceName)]
    public class UsersSample : ClientSample
    {
        /// <summary>
        /// Returns all users in account.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public PagedGraphUsers GetAllUsers()
        {
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();
            PagedGraphUsers users = graphClient.ListUsersAsync().Result;

            foreach (var user in users.GraphUsers)
            {
                Context.Log("{0} {1} {2}",
                    user.Descriptor.ToString().PadRight(8),
                    user.DisplayName.PadRight(20),
                    user.PrincipalName.PadRight(20)
                    );
            }

            return users;
        }

        /// <summary>
        /// Add an existing MSA guest user (by UPN), and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveMSAUserByUPN()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the MSA user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateUserMSA");
            GraphUserCreationContext addMSAUserContext = new GraphUserPrincipalNameCreationContext
            {
                PrincipalName = "fabrikamfiber4@hotmail.com"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addMSAUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetUserMSA");
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUserMSA");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipStateMSA");
            GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
            try
            {
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user (by UPN), and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADUserByUPN()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateUserAAD");
            GraphUserCreationContext addAADUserContext = new GraphUserPrincipalNameCreationContext
            {
                PrincipalName = "jtseng@vscsi.us"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetUserAAD");
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUserAAD");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipStateAAD");
            GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
            try
            {
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user (by UPN), added to a group, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADUserByUPNToGroup()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: create a group at the account level
            // 
            GraphGroupCreationContext createGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Developers-" + Guid.NewGuid(),
                Description = "Group created via client library"
            };

            GraphGroup newVSTSGroup = graphClient.CreateGroupAsync(createGroupContext).Result;
            IEnumerable<VisualStudio.Services.Common.SubjectDescriptor> parentGroup = new List<VisualStudio.Services.Common.SubjectDescriptor>() { newVSTSGroup.Descriptor };
            string groupDescriptor = newVSTSGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOIDAsMember");
            GraphUserCreationContext addAADUserContext = new GraphUserPrincipalNameCreationContext
            {
                PrincipalName = "jtseng@vscsi.us"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext, parentGroup).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 3: get the user
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetUser-AddRemoveAADUserByUPNToGroup");
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 4: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUser-AddRemoveAADUserByUPNToGroup");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipState-AddRemoveAADUserByUPNToGroup");
            GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
            try
            {
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }

            // Part 5: remove the group
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipStateAADGroup");
            membershipState = graphClient.GetMembershipStateAsync(groupDescriptor).Result;
            try
            {
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted group is not disabled!");
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user (by OID), and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADUserByOID()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOID");
            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "e97b0e7f-0a61-41ad-860c-748ec5fcb20b"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetUser-AddRemoveAADUserByOID");
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUser-AddRemoveAADUserByOID");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipState-AddRemoveAADUserByOID");
            GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
            try
            {
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user (by OID), with a specific StorageKey, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADUserByOIDWithStorageKey()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOIDWithStorageKey");
            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "27dbfced-5593-4756-98a3-913c39af7612",
                StorageKey = new Guid("9b71f216-4c4f-6b74-a911-efb0fa9c777f")
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetUser-AddRemoveAADUserByOIDWithStorageKey");
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUser-AddRemoveAADUserByOIDWithStorageKey");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipState-AddRemoveAADUserByOIDWithStorageKey");
            GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
            try
            {
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }
    }
}