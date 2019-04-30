using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.Graph
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateVSTSGroup-AddRemoveUserMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "AddUserToGroup-AddRemoveUserMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateMembershipUser-AddRemoveUserMembership");
            GraphMembership graphMembership = graphClient.AddMembershipAsync(userDescriptor, groupDescriptor).Result;

            //
            // Part 4: get the membership
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipUser");
            graphMembership = graphClient.GetMembershipAsync(userDescriptor, groupDescriptor).Result;

            //
            // Part 5: Check to see if the user is a member of the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipExistenceUser");
            graphClient.CheckMembershipExistenceAsync(userDescriptor, groupDescriptor).SyncResult();

            //
            // Part 6: Get every group the subject(user) is a member of
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "BatchGetMembershipsUserUp");
            List<GraphMembership> membershipsForUser = graphClient.ListMembershipsAsync(userDescriptor).Result;

            //
            // Part 7: Get every member of the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "BatchGetMembershipsGroupDown");
            List<GraphMembership> membershipsOfGroup = graphClient.ListMembershipsAsync(groupDescriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down).Result;

            //
            // Part 8: Remove member from the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteMembershipUser");
            graphClient.RemoveMembershipAsync(userDescriptor, groupDescriptor).SyncResult();
            try
            {
                ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipExistenceUserDeleted");
                graphClient.CheckMembershipExistenceAsync(userDescriptor, groupDescriptor).SyncResult();
            }
            catch (Exception e)
            {
                Context.Log("User is no longer a member of the group:" + e.Message);
            }

            //
            // Part 9: delete the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteGroup-AddRemoveUserMembership");
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();

            //
            // Part 10: remove the user
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUser-AddRemoveUserMembership");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            //
            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipStateUser-AddRemoveUserMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateVSTSGroup-AddRemoveVSTSGroupMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "AddUserToGroup-AddRemoveVSTSGroupMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipExistenceVSTSGroup");
            graphClient.CheckMembershipExistenceAsync(childGroupDescriptor, parentGroupDescriptor).SyncResult();

            //
            // Part 6: Get every group the subject('Contractors') is a member of
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "BatchGetMembershipsVSTSGroupUp");
            List<GraphMembership> membershipsForUser = graphClient.ListMembershipsAsync(childGroupDescriptor).Result;

            //
            // Part 7: Get every member of the 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "BatchGetMembershipsVSTSGroupDown");
            List<GraphMembership> membershipsOfGroup = graphClient.ListMembershipsAsync(parentGroupDescriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down).Result;

            //
            // Part 8: Remove member from the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteMembershipVSTSGroup");
            graphClient.RemoveMembershipAsync(childGroupDescriptor, parentGroupDescriptor).SyncResult();
            try
            {
                ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipExistenceVSTSGroupDeleted");
                graphClient.CheckMembershipExistenceAsync(childGroupDescriptor, parentGroupDescriptor).SyncResult();
            }
            catch (Exception e)
            {
                Context.Log("'Contractors' is no longer a member of the group:" + e.Message);
            }

            //
            // Part 9: delete the groups
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteChildGroup-AddRemoveVSTSGroupMembership");
            graphClient.DeleteGroupAsync(childGroupDescriptor).SyncResult();
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteParentGroup-AddRemoveVSTSGroupMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateVSTSGroup-AddRemoveAADGroupMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "AddUserToGroup-AddRemoveAADGroupMembership");
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
            ClientSampleHttpLogger.SetOperationName(this.Context, "CreateMembershipAADGroup-AddRemoveAADGroupMembership");
            GraphMembership graphMembership = graphClient.AddMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).Result;

            //
            // Part 4: get the membership
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipAADGroup-AddRemoveAADGroupMembership");
            graphMembership = graphClient.GetMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).Result;

            //
            // Part 5: Check to see if the AAD group is a member of the VSTS 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipExistenceAADGroup");
            graphClient.CheckMembershipExistenceAsync(aadGroupDescriptor, parentGroupDescriptor).SyncResult();

            //
            // Part 6: Get every group the subject(AAD group) is a member of
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "BatchGetMembershipsAADGroupDown");
            List<GraphMembership> membershipsForUser = graphClient.ListMembershipsAsync(aadGroupDescriptor).Result;

            //
            // Part 7: Get every member of the VSTS 'Developers' group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "BatchGetMembershipsAADGroupUp");
            List<GraphMembership> membershipsOfGroup = graphClient.ListMembershipsAsync(parentGroupDescriptor, Microsoft.VisualStudio.Services.Graph.GraphTraversalDirection.Down).Result;

            //
            // Part 8: Remove member from the group
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteMembershipAADGroup");
            graphClient.RemoveMembershipAsync(aadGroupDescriptor, parentGroupDescriptor).SyncResult();
            try
            {
                ClientSampleHttpLogger.SetOperationName(this.Context, "CheckMembershipExistenceAADGroupDeleted");
                graphClient.CheckMembershipExistenceAsync(aadGroupDescriptor, parentGroupDescriptor).SyncResult();
            }
            catch (Exception e)
            {
                Context.Log("AAD Group is no longer a member of the group:" + e.Message);
            }

            //
            // Part 9: delete the groups
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteAADGroup-AddRemoveAADGroupMembership");
            graphClient.DeleteGroupAsync(aadGroupDescriptor).SyncResult();
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteParentGroup-AddRemoveAADGroupMembership");
            graphClient.DeleteGroupAsync(parentGroupDescriptor).SyncResult();
        }
    }
}