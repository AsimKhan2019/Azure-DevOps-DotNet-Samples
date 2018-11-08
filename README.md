# .NET samples for Azure DevOps

[![buildstatus](https://dev.azure.com/mseng/_apis/public/build/definitions/b924d696-3eae-4116-8443-9a18392d8544/5045/badge)](https://dev.azure.com/mseng/AzureDevOps/Open%20ALM/_build/index?context=mine&path=%5C&definitionId=5045&_a=completed)

This repository contains C# samples that show how to integrate with Azure DevOps and Team Foundation Server using our [public client libraries](https://www.nuget.org/profiles/nugetvss), service hooks, and more.

## Explore the samples

Take a minute to explore the repo. It contains short snippets as well as longer examples that demonstrate how to integrate with Azure DevOps and Team Foundation Server

* **Snippets**: short reusable code blocks demonstrating how to call specific APIs.
* **Quickstarts**: self-contained programs demonstrating a specific scenario, typically by calling multiple APIs.

## About the official client libraries

For .NET developers, the primary (and highly recommended) way to integrate with Azure DevOps and Team Foundation Server is via our public .NET client libraries available on Nuget. [Microsoft.TeamFoundationServer.Client](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client) is the most popular Nuget package and contains clients for interacting with work item tracking, Git, version control, build, release management and other services.

See the [Azure DevOps client library documentation](https://docs.microsoft.com/en-us/azure/devops/integrate/concepts/dotnet-client-libraries?view=vsts) for more details.

### Sample console program

Simple console program that connects to Azure DevOps using a personal access token and displays the field values of a work item.

```cs
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
                String personalAccessToken = args[1];  // See https://docs.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/pats?view=vsts              
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
```

## Request other samples

Not finding a sample that demonstrates something you are trying to do? Let us know by opening an issue.


