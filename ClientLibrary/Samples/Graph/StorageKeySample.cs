using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ClientSamples.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.StorageKeys.StorageKeysResourceName)]
    public class StorageKeySample : ClientSample
    {
        /// <summary>
        /// Get a storage key from a descriptor
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public void GetDescriptorById()
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
                OriginId = "e97b0e7f-0a61-41ad-860c-748ec5fcb20b",
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the storage key
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetStorageKeyBySubjectDescriptor");
            GraphStorageKeyResult storageKey = graphClient.GetStorageKeyAsync(userDescriptor).Result;

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUser");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            try
            {
                ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipState");
                GraphMembershipState membershipState = graphClient.GetMembershipStateAsync(userDescriptor).Result;
                if (membershipState.Active) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The deleted user is not disabled!");
            }
        }
    }
}
