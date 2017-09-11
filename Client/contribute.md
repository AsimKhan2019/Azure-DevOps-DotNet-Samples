# Contribute to the samples

## Organization and style

1. Samples for an API area should live together under a folder with the name of the area, for example:
   ```
   |-- Microsoft.TeamServices.Samples.Client
       |-- Notification
   ```

2. A  class should be created for each resource in the area
   * The file name should be: `{Resource}Sample.cs`
     ```
     |-- Microsoft.TeamServices.Samples.Client, for example:
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
   * Avoid `var` typed variables
   * Go out of your way to show types so it is clear from the code what types are being used  
   * Samples should show catching exceptions for APIs where exceptions are common
   * Use constants from the client libraries for property names, area names, resource names, etc (avoid hard-coded strings)
   * Use line breaks and empty lines to help deliniate important sections or lines that need to stand out
   * Use the same "dummy" data across all samples so it's easier to correlate similar concepts

4. All samples **MUST** be runnable on their own without any input

5. All samples **SHOULD** clean up after themselves
   * A good pattern to follow: have a sample method create a resource (to demonstrate creation) and have a later sample method delete the previously created resource. In between the creation and deletion, you can show updating the resource (if applicable).
