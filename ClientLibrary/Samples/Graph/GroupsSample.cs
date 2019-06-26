using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.Graph
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
            PagedGraphGroups groups = graphClient.ListGroupsAsync().Result;

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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateGroup");
            GraphGroupCreationContext createGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Developers-" + Guid.NewGuid(),
                Description = "Group created via client library"
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(createGroupContext).Result;
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: update the description attribute for the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "UpdateGroup");
            Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument patchDocument = VssJsonPatchDocumentFactory.ConstructJsonPatchDocument(VisualStudio.Services.WebApi.Patch.Operation.Replace, Constants.GroupUpdateFields.Description, "Updated description");
            GraphGroup updatedGroup = graphClient.UpdateGroupAsync(groupDescriptor, patchDocument).Result;
            string groupDescription = updatedGroup.Description;

            Context.Log("Updated group description: {0}", groupDescription);

            //
            // Part 3: delete the group
            // 

            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteGroup");
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetDisabledGroup");
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
        /// Create a new project level group and then delete the group
        /// </summary>
        [ClientSampleMethod]
        public void CreateDeleteProjectVSTSGroup()
        {
            // Get the client
            VssConnection connection = Context.Connection;

            //
            // Part 1: get the project id
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetProjectId");
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            TeamProject project = projectClient.GetProject(projectName, includeCapabilities: true, includeHistory: true).Result;
            Guid projectId = project.Id;

            //
            // Part 2: get the project scope descriptor
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetProjectScopeDescriptor");
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();
            GraphDescriptorResult projectDescriptor = graphClient.GetDescriptorAsync(projectId).Result;

            //
            // Part 3: create a group at the project level
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateGroupInProject");
            GraphGroupCreationContext createGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Project Developers-" + Guid.NewGuid(),
                Description = "Group at project level created via client library"
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(createGroupContext, projectDescriptor.Value).Result;
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 4: delete the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteGroup");
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetDisabledGroup");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADGroupByOID");
            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "77ed2186-aaf6-4299-ac9e-37ba282c2b95"
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(addAADGroupContext).Result;
            string groupDescriptor = newGroup.Descriptor;
            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: get the group
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetGroup-AddRemoveAADGroupByOID");
            newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;

            //
            // Part 3: remove the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteGroup-AddRemoveAADGroupByOID");
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetDisabledGroup-AddRemoveAADGroupByOID");
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
        /// Add an existing Azure Active Directory group to the VSTS account, with a specific StorageKey, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADGroupByOIDWithStorageKey()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADGroupByOIDWithStorageKey");
            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "f0d20172-7b96-42f6-9436-941433654b48",
                StorageKey = Guid.NewGuid()
                //TODO: Remove Hard coded GUID StorageKey = new Guid("fc24f8cc-aed7-4bd4-be08-052d7fd30c39")
            };

            GraphGroup newGroup = graphClient.CreateGroupAsync(addAADGroupContext).Result;
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: get the group
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetGroup-AddRemoveAADGroupByOIDWithStorageKey");
            newGroup = graphClient.GetGroupAsync(groupDescriptor).Result;

            //
            // Part 3: remove the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteGroup-AddRemoveAADGroupByOIDWithStorageKey");
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            // Try to get the deleted group (should result in an exception)
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetDisabledGroup-AddRemoveAADGroupByOIDWithStorageKey");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateVSTSGroup");
            GraphGroupCreationContext createVSTSGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Developers-" + Guid.NewGuid(),
                Description = "Group created via client library"
            };

            GraphGroup newVSTSGroup = graphClient.CreateGroupAsync(createVSTSGroupContext).Result;
            IEnumerable<VisualStudio.Services.Common.SubjectDescriptor> parentGroup = new List<VisualStudio.Services.Common.SubjectDescriptor>() { newVSTSGroup.Descriptor };
            string vstsGroupDescriptor = newVSTSGroup.Descriptor;

            //
            // Part 2: add the AAD group
            // 

            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "7dee3381-2ec2-41c2-869a-7afe9b574095"
            };

            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADGroupByOIDAsMember");
            GraphGroup addedAADGroup = graphClient.CreateGroupAsync(addAADGroupContext, null, parentGroup).Result;
            string aadGroupDescriptor = addedAADGroup.Descriptor;

            Context.Log("New group created! ID: {0}", aadGroupDescriptor);

            //
            // Part 3: get the AAD group
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetGroup-AddRemoveAADGroupByOIDAsMemberOfVSTSGroup");
            GraphGroup newGroup = graphClient.GetGroupAsync(aadGroupDescriptor).Result;

            //
            // Part 4: remove the AAD group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteAADGroup-AddRemoveAADGroupByOIDAsMemberOfVSTSGroup");
            graphClient.DeleteGroupAsync(aadGroupDescriptor).SyncResult();

            //
            // Part 5: delete the VSTS group
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteVSTSGroup-AddRemoveAADGroupByOIDAsMemberOfVSTSGroup");
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