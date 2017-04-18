using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.Groups.GroupsResourceName)]
    public class GroupsSample : ClientSample
    {
        /// <summary>
        /// Returns all groups in account.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public PagedGraphGroups GetAllGroups()
        {
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();
            PagedGraphGroups groups = graphClient.GetGroupsAsync().Result;

            foreach (var group in groups.GraphGroups)
            {
                LogGroup(group);
            }

            return groups;
        }

        /// <summary>
        /// Create a new account level group, change the description, and then delete the group
        /// </summary>
        [ClientSampleMethod]
        public void CreateUpdateDeleteVSTSGroup()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: create a group at the account level
            // 

            GraphGroupCreationContext createGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Developers",
                Description = "Group created via client library"
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(createGroupContext).Result; //Bug 963554: Graph REST API client is failing to parse base64 encoded GroupDescriptor
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: update the description attribute for the group
            // 

            Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument patchDocument = VssJsonPatchDocumentFactory.ConstructJsonPatchDocument(VisualStudio.Services.WebApi.Patch.Operation.Replace, Constants.GroupUpdateFields.Description, "Updated description");
            GraphGroup updatedGroup = graphClient.UpdateGroupAsync(groupDescriptor, patchDocument).Result;
            string groupDescription = updatedGroup.Description;

            Context.Log("Updated group description: {0}", groupDescription);

            //
            // Part 3: delete the group
            // 

            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            try
            {
                newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the deleted group:" + e.Message);
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory group to the VSTS account, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADGroupByOID()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD group
            // 

            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "77ed2186-aaf6-4299-ac9e-37ba282c2b95"
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(addAADGroupContext).Result; //Bug 963789: Graph REST: Creation of a new VSTS group fails when descriptor not provided
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: get the group
            //
            newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;

            //
            // Part 3: remove the group
            // 

            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            try
            {
                newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the removed group:" + e.Message);
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory group to the VSTS account, with a specific VSID, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADGroupByOIDWithVSID()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD group
            // 

            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "f0d20172-7b96-42f6-9436-941433654b48",
                Id = Guid.NewGuid()
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(addAADGroupContext).Result; //Bug 963789: Graph REST: Creation of a new VSTS group fails when descriptor not provided
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: get the group
            //
            newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;

            //
            // Part 3: remove the group
            // 

            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            try
            {
                newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the removed group:" + e.Message);
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory group to the VSTS account, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADGroupByOIDAsMemberOfVSTSGroup()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: create the VSTS group
            // 

            GraphGroupCreationContext createVSTSGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Developers",
                Description = "Group created via client library"
            };

            GraphGroup newVSTSGroup = graphClient.CreateGroupAsync(createVSTSGroupContext).Result; //Bug 963554: Graph REST API client is failing to parse base64 encoded GroupDescriptor
            IEnumerable<VisualStudio.Services.Common.SubjectDescriptor> parentGroup = new List<VisualStudio.Services.Common.SubjectDescriptor>() { newVSTSGroup.Descriptor };
            string vstsGroupDescriptor = newVSTSGroup.Descriptor;

            //
            // Part 2: add the AAD group
            // 

            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "7dee3381-2ec2-41c2-869a-7afe9b574095"
            };

            GraphGroup addedAADGroup = graphClient.CreateGroupAsync(addAADGroupContext, null, parentGroup).Result; //Bug 963789: Graph REST: Creation of a new VSTS group fails when descriptor not provided
            string aadGroupDescriptor = addedAADGroup.Descriptor;

            Context.Log("New group created! ID: {0}", aadGroupDescriptor);

            //
            // Part 3: get the AAD group
            //
            GraphGroup newGroup = graphClient.GetGroupAsync(aadGroupDescriptor).Result;

            //
            // Part 4: remove the AAD group
            // 

            graphClient.DeleteGroupAsync(aadGroupDescriptor).SyncResult();

            //
            // Part 5: delete the VSTS group
            //

            graphClient.DeleteGroupAsync(vstsGroupDescriptor).SyncResult();
        }

        protected void LogGroup(GraphGroup group)
        {
            Context.Log(" {0} {1} {2}",
                group.Descriptor.ToString().PadRight(8),
                group.DisplayName.PadRight(20),
                group.Description.PadRight(60));
        }
    }
}
