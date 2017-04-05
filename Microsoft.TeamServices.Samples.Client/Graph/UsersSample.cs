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
        public List<GraphUser> GetAllUsers()
        {
            VssConnection connection = Context.Connection;
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();
            List<GraphUser> users = graphClient.GetUsersAsync().Result;

            foreach (var user in users)
            {
                LogUser(user);
            }

            return users;
        }

        /// <summary>
        /// Add an existing MSA guest user by UPN, and then remove it
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

            GraphUserCreationContext addAADUserContext = new GraphUserPrincipalNameCreationContext
            { 
                PrincipalName = "fabrikamfiber8@hotmail.com"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user (should result in an exception)
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the removed user:" + e.Message);
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user by UPN, and then remove it
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

            GraphUserCreationContext addAADUserContext = new GraphUserPrincipalNameCreationContext
            {
                PrincipalName = "vscsia@microsoft.com" //TODO: Can we get a different user account?
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user (should result in an exception)
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the removed user:" + e.Message);
            }
        }

        /// <summary>
        /// Add an existing Azure Active Directory user by OID, and then remove it
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

            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "ce4fd5fc-0b94-4562-8c7c-c23fdd3b5aa2"
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the user
            //
            newUser = graphClient.GetUserAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 

            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user (should result in an exception)
            try
            {
                newUser = graphClient.GetUserAsync(userDescriptor).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the removed user:" + e.Message);
            }
        }

        protected void LogUser(GraphUser user)
        {
            Context.Log(" {0} {1} {2}",
                user.Descriptor.ToString().PadRight(8),
                user.DisplayName.PadRight(20),
                user.PrincipalName.PadRight(20)
                );
        }
    }
}
