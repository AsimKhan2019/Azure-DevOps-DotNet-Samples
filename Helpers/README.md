# Helpers

This project include various useful "helper" samples.

## OrganizationUrlHelpers 

Provides sample helper methods for properly constructing URLs to VSTS organizations (formerly "account") from an organizastion ID or name.

### Create a connection to an organization using its name

```cs
public async Task DoSomething()
{
    Uri orgUrl = OrganizationUrlHelpers
        .GetConnectionUrl("myOrgName");  // https://myorgname.visualstudio.com

    VssConnection orgConnection = new VssConnection(orgUrl, credentials);

    BuildHttpClient buildClient = orgConnection.GetClient<BuildHttpClient>();
}
```

### Create a connection to an organization using its ID

```cs
public async Task DoSomething()
{
    Guid orgId = Guid.Parse("279ed1c4-2f72-4e61-8c95-4bafcc13fd02"); // ID for the "MyOrgName" organzation 

    Uri orgUrl = OrganizationUrlHelpers
        .GetConnectionUrl(orgId);  // https://myorgname.visualstudio.com

    VssConnection orgConnection = new VssConnection(orgUrl, credentials);

    BuildHttpClient buildClient = orgConnection.GetClient<BuildHttpClient>();
}
```
