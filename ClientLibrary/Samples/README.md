# Client library sample snippets

This project contains C# samples that show how to integrate with Azure DevOps using the [public client libraries](https://www.nuget.org/profiles/nugetvss). Most samples are short snippets that call just a single REST API.

You can use these snippets to jumpstart your own apps and services.

## Explore

Samples are organized by "area" (service) and "resource". Each sample class shows various ways for interacting with Azure DevOps.

## Run the samples

1. Clone this repository and open in Visual Studio (2015 or later)

2. Build the solution (you may need to restore the required NuGet packages first)

3. Run `Microsoft.Azure.DevOps.ClientSamples.exe` with the required arguments:
   * `/url:{value}`: URL of the account/collection to run the samples against.
   * `/area:{value}`: API area (work, wit, notification, git, core, build) to run the client samples for. Use * to include all areas.
   * `/resource:{value}`: API resource to run the client samples for. Use * to include all resources.

> **IMPORTANT**: some samples are destructive. It is recommended that you run these samples against a test account.

### Examples of how to run different samples

#### Run all samples

```
Microsoft.Azure.DevOps.ClientSamples.exe
    /url:https://dev.azure.com/fabrikam /area:* /resource:*
```

#### Run all work item tracking samples

```
Microsoft.Azure.DevOps.ClientSamples.exe
    /url:https://dev.azure.com/fabrikam /area:wit /resource:*
```

#### Run all graph samples

```
Microsoft.Azure.DevOps.ClientSamples.exe
    /url:https://dev.azure.com/fabrikam /area:graph /resource:*
```

#### Run all Git pull request samples

```
Microsoft.Azure.DevOps.ClientSamples.exe
    /url:https://dev.azure.com/fabrikam /area:git /resource:pullrequests
```

#### Run all samples against an on-premises TFS project collection

```
Microsoft.Azure.DevOps.ClientSamples.exe
    /url:https://mytfs:8080/tfs/testcollection /area:git /resource:*
```

### Save request and response data to a JSON file

To persist the HTTP request/response as JSON for each client sample method that is run, set the `/outputPath:{value}` argument. For example:

```
Microsoft.Azure.DevOps.ClientSamples.exe
    /url:https://dev.azure.com/fabrikam /area:* /resource:* /outputPath:c:\temp\http-output
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

## Contribute

For developers that want to contribute, learn how to [contribute a snippet sample](./contribute.md).
