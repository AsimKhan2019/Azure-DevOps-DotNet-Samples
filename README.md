# Team Services .NET Samples

## Getting started



## Using the client libraries



## About the client libraries

For .NET developers building Windows apps and services that integrate with Visual Studio Team Services, client libraries are available for integrating with work item tracking, version control, build, and other services are now available. These packages replace the traditional TFS Client OM installer and make it easy to acquire and redistribute the libraries needed by your app or service.

| Package | Description | When to use |
|---------|-------------|---------------|
| [Microsoft.TeamFoundationServer.ExtendedClient](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.ExtendedClient/) | Integrate with Microsoft Team Foundation Server (2012, 2013, 2015) and Visual Studio Team Services from desktop-based Windows applications. Work with and manage version control, work items, and build, and other resources from your client application. | Existing Windows apps leveraging an older version of the TFS Client OM. 
| [Microsoft.TeamFoundationServer.Client](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client/) | Integrate with Team Foundation Server 2015 and Visual Studio Team Services from desktop-based, ASP.NET, and other Windows applications. Provides access to version control, work item tracking, build, and more via public REST APIs. | Window desktop apps and services that need to integrate with TFS 2015 and later and Visual Studio Team Services.
| [Microsoft.VisualStudio.Services.Client](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.Client/) | Integrate with Team Foundation Server 2015 and Visual Studio Team Services from desktop-based, ASP.NET, and other Windows applications. Provides access to shared platform services such as account, profile, identity, security, and more via public REST APIs. | Windows desktop apps and services that need to interact with "shared platform" services (account, profile, identity, security, etc).
| [Microsoft.VisualStudio.Services.InteractiveClient](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.InteractiveClient/) | Integrate with Team Foundation Server 2015 and Visual Studio Team Services from desktop-based Windows applications that require interactive sign-in by a user. | Windows desktop applications not utilizing basic authentication or OAuth for authentication.
| [Microsoft.VisualStudio.Services.DistributedTask.Client](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.DistributedTask.Client/) | Integrate with Team Foundation Server 2015 and Visual Studio Team Services from desktop-based, ASP.NET, and other Windows applications. Provides access to the Distributed Task Service via public REST APIs. | Window desktop apps and services that need to integrate with TFS 2015 and later and Visual Studio Team Services.
| [Microsoft.VisualStudio.Services.Release.Client](https://www.nuget.org/packages/Microsoft.VisualStudio.Services.Release.Client/) | Integrate with Team Foundation Server 2015 and Visual Studio Team Services from desktop-based, ASP.NET, and other Windows applications. Provides access to the Release Service via public REST APIs. | Window desktop apps and services that need to integrate with TFS 2015 and later and Visual Studio Team Services.

## Useful references

* [.NET Client library intro](https:// www.visualstudio.com/docs/integrate/get-started/client-libraries/dotnet)
* [WIQL reference](https:// msdn.microsoft.com/en-us/library/bb130198(v=vs.90).aspx)

## Contributing to the samples


