using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.Graph
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
            PagedGraphUsers users = graphClient.GetUsersAsync().Result;

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
            //newUser = graphClient.GetUserAsync(userDescriptor).Result; //BUG ???: {"TF14045: The identity with type 'Microsoft.IdentityModel.Claims.ClaimsIdentity' and identifier '45aa3d2d-7442-473d-b4d3-3c670da9dd96\\fabrikamfiber4@hotmail.com' could not be found."}

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUserMSA");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            try
            {
                ClientSampleHttpLogger.SetOperationName(this.Context, "GetDisabledUserMSA");
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
                // TODO: Fix if (!newUser.Disabled) throw new Exception();
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
            //newUser = graphClient.GetUserAsync(userDescriptor).Result;  //BUG ???: {"TF14045: The identity with type 'Microsoft.IdentityModel.Claims.ClaimsIdentity' and identifier '45aa3d2d-7442-473d-b4d3-3c670da9dd96\\jtseng@vscsi.us' could not be found."}

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUserAAD");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            try
            {
                ClientSampleHttpLogger.SetOperationName(this.Context, "GetDisabledUserAAD");
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
                // TODO: if (!newUser.Disabled) throw new Exception();
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
            //newUser = graphClient.GetUserAsync(userDescriptor).Result;  //BUG ???: {"TF14045: The identity with type 'Microsoft.IdentityModel.Claims.ClaimsIdentity' and identifier '45aa3d2d-7442-473d-b4d3-3c670da9dd96\\jtseng@vscsi.us' could not be found."}

            //
            // Part 4: remove the user
            // 

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
                // TODO: if (!newUser.Disabled) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }

            // Part 5: remove the group
            graphClient.DeleteGroupAsync(groupDescriptor).SyncResult();
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
            //newUser = graphClient.GetUserAsync(userDescriptor).Result; //BUG ???: TF14045: The identity with type 'Microsoft.IdentityModel.Claims.ClaimsIdentity' and identifier '45aa3d2d-7442-473d-b4d3-3c670da9dd96\jtseng@vscsi.us' could not be found.

            //
            // Part 3: remove the user
            // 

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
                // TODO: if (!newUser.Disabled) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user (by OID), with a specific VSID, and then remove it
        /// </summary>
        [ClientSampleMethod]
        public void AddRemoveAADUserByOIDWithVSID()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            //
            // Part 1: add the AAD user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOIDWithVSID");
            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "e97b0e7f-0a61-41ad-860c-748ec5fcb20b"
                /* TODO: Id = Guid.NewGuid() */
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            //newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
                // TODO: if (!newUser.Disabled) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }
    }
}
