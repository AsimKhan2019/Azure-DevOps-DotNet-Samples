using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                Uri accountUri = new Uri(args[0]);     // Account URL, for example: https://fabrikam.visualstudio.com                
                String personalAccessToken = args[1];  // See https://www.visualstudio.com/docs/integrate/get-started/authentication/pats                
                int workItemId = int.Parse(args[2]);   // ID of a work item, for example: 12

                // Create a connection to the account
                VssConnection connection = new VssConnection(accountUri, new VssBasicCredential(string.Empty, personalAccessToken));
                
                // Get an instance of the work item tracking client
                WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

                try
                {
                    // Get the specified work item
                    WorkItem workitem = witClient.GetWorkItemAsync(workItemId).Result;

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
            else
            {
                Console.WriteLine("Usage: ConsoleApp {accountUri} {personalAccessToken} {workItemId}");
            }
        }
    }
}