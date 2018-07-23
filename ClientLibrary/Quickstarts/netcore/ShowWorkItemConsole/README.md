# Show work item console app (.NET Core 2.0)

Simple [.NET Core](https://docs.microsoft.com/dotnet/core/) console app that uses [Microsoft.TeamFoundationServer.Client](https://www.nuget.org/packages/Microsoft.TeamFoundationServer.Client) to retrieve details about a Visual Studio Team Services work item.

This sample shows how to use the Work Item Tracking client, but other clients (build, release, extension management, etc) are also supported.

## How to run

1. If you do not already have a Visual Studio Team Services account, [create one](https://docs.microsoft.com/vsts/organizations/accounts/create-account-msa-or-work-student?view=vsts) (it's free)
2. Create a work item in your account
3. Create a personal access token ([steps](https://docs.microsoft.com/vsts/organizations/accounts/use-personal-access-tokens-to-authenticate?view=vsts))
4. Install [.NET SDK (2.0 or later)](https://microsoft.com/net/core)
5. Build `dotnet build`
   > Note: you may see warnings about certain dependencies not being fully compatible with the project. This is expected and is due to some dependencies not explicitly listing .NET Standard or .NET Core as a supported framework. 

6. Run `dotnet run https://{account}.visualstudio.com {token} {workItemId}` (substituting these values for your account name, your personal access token, and a valid work item ID)
