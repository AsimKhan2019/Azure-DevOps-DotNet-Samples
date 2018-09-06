using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ShowWorkItemConsole
{
    /// <summary>
    /// Simple console program that shows information about a VSTS work item.
    ///
    /// Usage:
    ///      ShowWorkItemConsole [organizationName] [personalAccessToken] [workItemNumber]
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string organizationName = args[0];   // Organization (formerly called "account") name, for example: "fabrikam"  
            string accessToken = args[1];        // Personal access token. See https://docs.microsoft.com/vsts/integrate/get-started/authentication/pats?view=vsts
            int workItemId = int.Parse(args[2]); // Work item ID, for example: 12       

            try
            {
                WorkItem workitem = GetWorkItem(organizationName, accessToken, workItemId).GetAwaiter().GetResult();

                // Output the work item's field values
                foreach (var field in workitem.Fields)
                {
                    Console.WriteLine($"  {field.Key}: {field.Value}");
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
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred: " + ex.Message);
            }
        }

        static async Task<WorkItem> GetWorkItem(string organizationName, string accessToken, int workItemId)
        {            
            // Get the connection URL for the specified VSTS organization name
            Uri organizationUrl = await VssConnectionHelper.GetOrganizationUrlAsync(organizationName);
           
            // Create a connection to the organization
            VssConnection connection = new VssConnection(organizationUrl, new VssBasicCredential(string.Empty, accessToken));
            
            // Get an instance of the work item tracking client
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Make the call and return the work item
            return await witClient.GetWorkItemAsync(workItemId);
        }
    }
}