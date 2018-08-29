# Helpers

This project include various useful "helper" samples.

## OrganizationUrlHelpers 

Provides sample helper methods for properly constructing URLs to VSTS organizations (formerly "account") using an organization ID or name.

### Get the URL for a VSTS organization (by name)

```cs
public async Task DoSomething()
{
    Uri orgUrl = Samples.Helpers.OrganizationUrlHelpers
        .GetUrl("myOrgName");  // https://myorgname.visualstudio.com

   HttpClient client = new HttpClient();

    await client.GetAsync(orgUrl);
}
```

### Get the URL for a VSTS organization (by ID)

```cs
public async Task DoSomething()
{
    Guid orgId = Guid.Parse("279ed1c4-2f72-4e61-8c95-4bafcc13fd02"); // ID for the "myOrgName" organzation 

    Uri orgUrl = Samples.Helpers.OrganizationUrlHelpers
        .GetUrl(orgId);  // https://myorgname.visualstudio.com

    HttpClient client = new HttpClient();
    
    await client.GetAsync(orgUrl);
}
```
