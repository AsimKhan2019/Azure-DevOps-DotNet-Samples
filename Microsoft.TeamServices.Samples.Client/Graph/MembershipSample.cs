using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.Memberships.MembershipsResourceName)]
    public class MembershipSample : ClientSample
    {
        /// <summary>
        /// Add a user to a group and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveUserMembership()
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

            GraphGroup newGroup = graphClient.CreateGroupAsync(createGroupContext).Result;
            string groupDescriptor = newGroup.Descriptor;

            Context.Log("New group created! ID: {0}", groupDescriptor);

            //
            // Part 2: add the user
            // 

            GraphUserCreationContext addUserContext = new GraphUserPrincipalNameCreationContext
            {
                PrincipalName = "jtseng@vscsi.us"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 3: Make the user a member of the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateMembershipUser");
            GraphMembership graphMembership = graphClient.AddMembershipAsync(userDescriptor, groupDescriptor).Result;

            //
            // Part 4: get the membership
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipUser");
            graphMembership = graphClient.GetMembershipAsync(userDescriptor, groupDescriptor).Result;

            //
            // Part 5: Check to see if the user is a member of the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipUser");
            graphClient.CheckMembershipAsync(userDescriptor, groupDescriptor).SyncResult();

            //
            // Part 6: Get every group the subject(user) is a member of
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipsUserUp");
            List<GraphMembership> membershipsForUser = graphClient.GetMembershipsAsync(userDescriptor).Result;

            //
            // Part 7: Get every member of the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipsGroupDown");
            List<GraphMembership> membershipsOfGroup = graphClient.GetMembershipsAsync(groupDescriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down.ToString()).Result; //Bug 967647: REST: GetMembershipsAsync shouldn't take direction as string, it should be the GraphTraversalDirection enum

            //
            // Part 8: Remove member from the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteMembershipUser");
            graphClient.RemoveMembershipAsync(userDescriptor, groupDescriptor).SyncResult();
            try {
                graphClient.CheckMembershipAsync(userDescriptor, groupDescriptor).SyncResult();
            }
            catch (Exception e) {
                Context.Log("User is no longer a member of the group:" + e.Message);
			}

			//
			// Part 9: delete the group
			// 

			graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            //
            // Part 10: remove the user

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();
            //
            // Try to get the deleted user
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
                if (!newUser.Disabled) throw new Exception();
            }
            catch (Exception e)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }

        /// <summary>
        /// Add a VSTS group to a group and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveVSTSGroupMembership()
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
            GraphGroup parentGroup = graphClient.CreateGroupAsync(createGroupContext).Result;
            string parentGroupDescriptor = parentGroup.Descriptor;
            Context.Log("New group created! ID: {0}", parentGroupDescriptor);

            //
            // Part 2: create a second group at the account level
            // 

            createGroupContext = new GraphGroupVstsCreationContext
            {
                DisplayName = "Contractors",
                Description = "Child group created via client library"
            };
            GraphGroup childGroup = graphClient.CreateGroupAsync(createGroupContext).Result;
            string childGroupDescriptor = childGroup.Descriptor;
            Context.Log("New group created! ID: {0}", childGroupDescriptor);

            //
            // Part 3: Make the 'Contractors' group a member of the 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateMembershipVSTSGroup");
            GraphMembership graphMembership = graphClient.AddMembershipAsync(childGroupDescriptor, parentGroupDescriptor).Result;

            //
            // Part 4: get the membership
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipVSTSGroup");
            graphMembership = graphClient.GetMembershipAsync(childGroupDescriptor, parentGroupDescriptor).Result;

            //
            // Part 5: Check to see if the 'Contractors' group is a member of the 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipVSTSGroup");
            graphClient.CheckMembershipAsync(childGroupDescriptor, parentGroupDescriptor).SyncResult();

            //
            // Part 6: Get every group the subject('Contractors') is a member of
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipsVSTSGroupUp");
            List<GraphMembership> membershipsForUser = graphClient.GetMembershipsAsync(childGroupDescriptor).Result;

            //
            // Part 7: Get every member of the 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipsVSTSGroupDown");
            List<GraphMembership> membershipsOfGroup = graphClient.GetMembershipsAsync(parentGroupDescriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down.ToString()).Result; //Bug 967647: REST: GetMembershipsAsync shouldn't take direction as string, it should be the GraphTraversalDirection enum

            //
            // Part 8: Remove member from the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteMembershipVSTSGroup");
            graphClient.RemoveMembershipAsync(childGroupDescriptor, parentGroupDescriptor).SyncResult();
            try
            {
                graphClient.CheckMembershipAsync(childGroupDescriptor, parentGroupDescriptor).SyncResult();
            }
            catch (Exception e)
            {
                Context.Log("'Contractors' is no longer a member of the group:" + e.Message);
            }

            //
            // Part 9: delete the groups
            // 
            graphClient.DeleteGroupAsync(childGroupDescriptor).SyncResult();
            graphClient.DeleteGroupAsync(parentGroupDescriptor).SyncResult();
        }

        /// <summary>
        /// Add an AAD group to a group and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADGroupMembership()
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
            GraphGroup parentGroup = graphClient.CreateGroupAsync(createGroupContext).Result;
            string parentGroupDescriptor = parentGroup.Descriptor;
            Context.Log("New group created! ID: {0}", parentGroupDescriptor);

            //
            // Part 2: add the AAD group
            // 

            GraphGroupCreationContext addAADGroupContext = new GraphGroupOriginIdCreationContext
            {
                OriginId = "a42aad15-d654-4b16-9309-9ee34d5aacfb"
            };
            GraphGroup aadGroup = graphClient.CreateGroupAsync(addAADGroupContext).Result;
            string aadGroupDescriptor = aadGroup.Descriptor;

            Context.Log("AAD group added! ID: {0}", aadGroupDescriptor);

            //
            // Part 3: Make the AAD group a member of the VSTS 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateMembershipAADGroup");
            GraphMembership graphMembership = graphClient.AddMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).Result;

            //
            // Part 4: get the membership
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipAADGroup");
            graphMembership = graphClient.GetMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).Result;

            //
            // Part 5: Check to see if the AAD group is a member of the VSTS 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipAADGroup");
            graphClient.CheckMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).SyncResult();

            //
            // Part 6: Get every group the subject(AAD group) is a member of
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipsAADGroupDown");
            List<GraphMembership> membershipsForUser = graphClient.GetMembershipsAsync(aadGroupDescriptor).Result;

            //
            // Part 7: Get every member of the VSTS 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipsAADGroupUp");
            List<GraphMembership> membershipsOfGroup = graphClient.GetMembershipsAsync(parentGroupDescriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down.ToString()).Result; //Bug 967647: REST: GetMembershipsAsync shouldn't take direction as string, it should be the GraphTraversalDirection enum

            //
            // Part 8: Remove member from the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteMembershipAADGroup");
            graphClient.RemoveMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).SyncResult();
            try
            {
                graphClient.CheckMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).SyncResult();
            }
            catch (Exception e)
            {
                Context.Log("AAD Group is no longer a member of the group:" + e.Message);
            }

            //
            // Part 9: delete the groups
            // 
            graphClient.DeleteGroupAsync(aadGroupDescriptor).SyncResult();
            graphClient.DeleteGroupAsync(parentGroupDescriptor).SyncResult();
        }
    }
}
