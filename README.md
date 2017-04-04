# Team Services Samples for .NET

![buildstatus](https://mseng.visualstudio.com/_apis/public/build/definitions/b924d696-3eae-4116-8443-9a18392d8544/5045/badge)

This repository contains C# samples that show how to integrate with Team Services and Team Foundation Server using our [public client libraries](https://www.nuget.org/profiles/nugetvss).

## Explore

Samples are organized by "area" (service) and "resource" within the `Microsoft.TeamServices.Samples.Client` project. Each sample class shows various ways for interacting with Team Services and Team Foundation Server.  

## Run the samples

1. Clone this repository and open in Visual Studio (2015 or later)

2. Build the solution (you may need to restore the required NuGet packages first)

3. Run the `Microsoft.TeamServices.Samples.Client.Runner` project with the required arguments:
   * `/url:{value}`: URL of the account/collection to run the samples against.
   * `/area:{value}`: API area (work, wit, notification, git, core, build) to run the client samples for. Use * to include all areas.
   * `/resource:{value}`: API resource to run the client samples for. Use * to include all resources.

> **IMPORTANT**: some samples are destructive. It is recommended that you first run the samples against a test account.

### Examples

#### Run all samples

```
Microsoft.TeamServices.Samples.Client.Runner.exe
    /url:https://fabrikam.visualstudio.com /area:* /resource:*
```

#### Run all work item tracking samples

```
Microsoft.TeamServices.Samples.Client.Runner.exe
    /url:https://fabrikam.visualstudio.com /area:wit /resource:*
```

#### Run all Git pull request samples

```
Microsoft.TeamServices.Samples.Client.Runner.exe
    /url:https://fabrikam.visualstudio.com /area:git /resource:pullrequests
```

#### Run all samples against a TFS on-premises collection

```
Microsoft.TeamServices.Samples.Client.Runner.exe
    /url:https://mytfs:8080/tfs/testcollection /area:git /resource:*
```

### Save request and response data to a JSON file

To persist the HTTP request/response as JSON for each client sample method that is run, set the `/outputPath:{value}` argument. For example:

```
Microsoft.TeamServices.Samples.Client.Runner.exe
    /url:https://fabrikam.visualstudio.com /area:* /resource:* /outputPath:c:\temp\http-output
```

This creates a folder for each area, a folder for each resource under the area folder, and a file for each client sample method that was run. The name of the JSON file is determined by the name of the client sample method. For example:

```
|-- temp
    |-- http-output
        |-- Notification
            |-- EventTypes
                |-- ...
            |-- Subscriptions
                |-- CreateSubscriptionForUser.json
                |-- QuerySubscriptionsByEventType.json
                |-- ...
```

Note: certain HTTP headers like `Authorization` are removed for security/privacy purposes.

## About the client libraries

For .NET developers building Windows apps and services that integrate with Visual Studio Team Services, client libraries are available for integrating with work item tracking, version control, build, and other services are now available. These packages replace the traditional TFS Client OM installer and make it easy to acquire and redistribute the libraries needed by your app or service.

See [.NET client libraries for Team Services documentation](https://www.visualstudio.com/docs/integrate/get-started/client-libraries/dotnet) for more getting started details.

### Other useful resources

* [Official NuGet packages](https://www.nuget.org/profiles/nugetvss)
* [.NET Client library intro](https://www.visualstudio.com/docs/integrate/get-started/client-libraries/dotnet)
* [WIQL reference](https://msdn.microsoft.com/en-us/library/bb130198(v=vs.90).aspx)

## Contribute

For developers that want to contribute, learn how to [contribute a sample](./contribute.md).
