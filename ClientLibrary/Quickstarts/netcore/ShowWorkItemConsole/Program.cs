using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;

using Samples.Helpers;

namespace Samples.ClientLibrary.Quickstarts.ShowWorkItemConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string organizationName = args[0];   // Organization (formerly "account") name, for example: "fabrikam"  
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
            catch (OrganizationNotFoundException onfe)
            {
                Console.WriteLine($"Could not find organizatiokn {organizationName}: {onfe.Message}");
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
            // Get the connection URL for the specified VSTS organization
            Uri organizationUrl = await OrganizationUrlHelpers.GetUrl(organizationName);
           
            // Create a connection to the organization
            VssConnection connection = new VssConnection(organizationUrl, new VssBasicCredential(string.Empty, accessToken));
            
            // Get an instance of the work item tracking client
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Return the specified work item
           return await witClient.GetWorkItemAsync(workItemId);
        }
    }
}