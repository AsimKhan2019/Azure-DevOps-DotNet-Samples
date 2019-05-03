# Contribute to the client library samples

## Organization and style

1. Samples for an API area should live together under a folder with the name of the area, for example:
   ```
   |-- ClientLibrary
       |-- Samples
           |-- Notification
   ```

2. A  class should be created for each resource in the area
   * The file name should be: `{Resource}Sample.cs`
     ```
         |-- Notification
             |-- SubscriptionsSample.cs
     ```
   * The name of the class should be `{Resource}Sample` and must extend from `ClientSample`, for example:
     ```
     public class SubscriptionsSample : ClientSample
     ```
   * The class must have the `[ClientSample]` attribute indicating the area/resource name the sample represents, for example:
     ```
     [ClientSample(NotificationApiConstants.AreaName, NotificationApiConstants.SubscriptionsResource.Name)]
     ```      
   * Each runnable client sample method must have 0 arguments and have the `[ClientSampleMethod]` attribute:
     ```cs
     [ClientSampleMethod]
     public IEnumerable<NotificationSubscription> ListCustomSubscriptions()
     {
         ...

         return subscriptions;
     }
     ```

3. Coding and style
   * Avoid `var` typed variables (show actual types so it is clear from the code what types are being used)
   * For calls where exceptions are common or expected, show a `try/catch` that uses the expected exception type
   * Use constants from the client libraries for property names, area names, resource names, etc (avoid hard-coded strings)
   * Use line breaks and empty lines to help deliniate important sections or lines that need to stand out
   * Use the same "dummy" data across all samples so it's easier to correlate similar concepts
   * Use `Context.Log` instead of `Console.WriteLine` for writing messages and data

4. All samples **MUST** be runnable on their own without any input

5. All samples **SHOULD** clean up after themselves
   * A good pattern to follow:
     * First sample method creates a resource
     * Subsequent methods show querying and updating the resource
     * Final method deletes the resource
     
6. Avoid destructive samples that have the potential to destroy real user data
   * Although a user should only ever run the samples against a test organization,avoid samples that destroy data arbitrarily (for example, a sample that finds the first project in the organization and deletes the first repository it finds). Instead follow the suggested pattern above and only destroy resources that earlier sample methods explicitly created.
