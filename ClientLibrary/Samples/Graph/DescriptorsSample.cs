using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.DevOps.ClientSamples.Graph
{
    [ClientSample(GraphResourceIds.AreaName, GraphResourceIds.Descriptors.DescriptorsResourceName)]
    public class DescriptorsSample : ClientSample
    {
        /// <summary>
        /// Get a descriptor from a StorageKey
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
            Guid storageKey = new Guid("9b71f216-4c4f-6b74-a911-efb0fa9c777f");

            ClientSampleHttpLogger.SetOperationName(this.Context, "MaterializeAADUserByOIDWithStorageKey");
            GraphUserCreationContext addAADUserContext = new GraphUserOriginIdCreationContext
            {
                OriginId = "27dbfced-5593-4756-98a3-913c39af7612",
                StorageKey = storageKey
            };

            GraphUser newUser = graphClient.CreateUserAsync(addAADUserContext).Result;
            string userDescriptor = newUser.Descriptor;

            Context.Log("New user added! ID: {0}", userDescriptor);

            //
            // Part 2: get the descriptor
            //
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetDescriptorById");
            GraphDescriptorResult descriptor = graphClient.GetDescriptorAsync(storageKey).Result; 
            try
            {
                if (descriptor.Value != userDescriptor) throw new Exception();
            }
            catch (Exception)
            {
                Context.Log("The descriptors don't match!");
            }

            //
            // Part 3: remove the user
            // 
            ClientSampleHttpLogger.SetOperationName(this.Context, "DeleteUser");
            graphClient.DeleteUserAsync(userDescriptor).SyncResult();

            // Try to get the deleted user
            ClientSampleHttpLogger.SetOperationName(this.Context, "GetMembershipState");
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
