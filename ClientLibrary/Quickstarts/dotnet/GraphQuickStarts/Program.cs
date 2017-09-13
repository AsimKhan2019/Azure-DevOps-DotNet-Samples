using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace GraphQuickStarts
{
    class Program
    {
        //============= Config [Edit these with your settings] =====================
        internal const string vstsCollectionUrl = "https://myaccount.visualstudio.com"; //change to the URL of your VSTS account; NOTE: This must use HTTPS
        // internal const string vstsCollectioUrl = "http://myserver:8080/tfs/DefaultCollection" alternate URL for a TFS collection
        //==========================================================================        

        static void Main(string[] args)
        {
            VssConnection connection = new VssConnection(new Uri(vstsCollectionUrl), new VssClientCredentials());
            GraphHttpClient graphClient = connection.GetClient<GraphHttpClient>();

            // Get the first page of Users
            PagedGraphUsers users = graphClient.GetUsersAsync().Result;

            // If there are more than a page's worth of users, continue retrieving users from the server
            string continuationToken = users.ContinuationToken.FirstOrDefault();
            while (continuationToken != null)
            {
                users = graphClient.GetUsersAsync(continuationToken: continuationToken).Result;
                // DO SOMETHING WITH THE USERS LIST

                continuationToken = users.ContinuationToken.FirstOrDefault();
            }
        }
    }
}
