# .NET samples for Azure DevOps

[![Build Status](https://dev.azure.com/ms/azure-devops-dotnet-samples/_apis/build/status/Microsoft.azure-devops-dotnet-samples?branchName=master)](https://dev.azure.com/ms/azure-devops-dotnet-samples/_build/latest?definitionId=82&branchName=master)

This repository contains C# samples that show how to integrate with  Azure DevOps Services and Azure using our [public client libraries](https://www.nuget.org/profiles/nugetvss), service hooks, and more.

## Explore the samples

Take a minute to explore the repo. It contains short snippets as well as longer examples that demonstrate how to integrate with Azure DevOps Services and Azure DevOps Server

* **Snippets**: short reusable code blocks demonstrating how to call specific APIs.
* **Quickstarts**: self-contained programs demonstrating a specific scenario, typically by calling multiple APIs.

## About the official client libraries

For .NET developers, the primary (and highly recommended) way to integrate with Azure DevOps Services and Azure DevOps Server is via our public .NET client libraries available on Nuget. [Microsoft.TeamFoundationServer.Client](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client) is the most popular Nuget package and contains clients for interacting with work item tracking, Git, version control, build, release management and other services.

See the [Azure DevOps client library documentation](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=vsts) for more details.

### Sample console program

Simple console program that connects to Azure DevOps using a personal access token and displays the field values of a work item.

```cs
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
```

## Request other samples

Not finding a sample that demonstrates something you are trying to do? Let us know by opening an issue.
