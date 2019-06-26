using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                Uri orgUrl = new Uri(args[0]);         // Organization URL, for example: https://dev.azure.com/fabrikam               
                String personalAccessToken = args[1];  // See https://docs.microsoft.com/azure/devops/integrate/get-started/authentication/pats
                int workItemId = int.Parse(args[2]);   // ID of a work item, for example: 12

                // Create a connection
                VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));

                // Show details a work item
                ShowWorkItemDetails(connection, workItemId).Wait();
            }
            else
            {
                Console.WriteLine("Usage: ConsoleApp {orgUrl} {personalAccessToken} {workItemId}");
            }

        }

        static private async Task ShowWorkItemDetails(VssConnection connection, int workItemId)
        {
            // Get an instance of the work item tracking client
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                // Get the specified work item
                WorkItem workitem = await witClient.GetWorkItemAsync(workItemId);

                // Output the work item's field values
                foreach (var field in workitem.Fields)
                {
                    Console.WriteLine("  {0}: {1}", field.Key, field.Value);
                }
            }
            catch (AggregateException aex)
            {
                VssServiceException vssex = aex.InnerException as VssServiceException;
                if (vssex != null)
                {
                    Console.WriteLine(vssex.Message);
                }
            }
        }
    }
}