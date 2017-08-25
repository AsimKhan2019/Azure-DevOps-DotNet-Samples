using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.ServiceHooks
{
    /// <summary>
    /// 
    /// Samples showing how to use the Service Hooks client to create and manage service hook subscriptions
    /// 
    /// For more details, see https://www.visualstudio.com/en-us/docs/integrate/api/hooks/subscriptions
    /// 
    /// </summary>
    [ClientSample(ServiceHooksPublisherApiConstants.AreaName, "Subscriptions")]
    public class SubscriptionsSample : ClientSample
    {

        /// <summary>
        /// Create a new web hook subscription that triggers a notification on all new work items created in the specified project.
        /// </summary>
        [ClientSampleMethod]
        public Subscription CreateWebHooksSubscription()
        {
            // Get the project to create the subscription in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get the client
            VssConnection connection = Context.Connection;
            ServiceHooksPublisherHttpClient serviceHooksClient = connection.GetClient<ServiceHooksPublisherHttpClient>();

            Subscription subscriptionParameters = new Subscription()
            {
                ConsumerId = "webHooks",
                ConsumerActionId = "httpRequest",
                ConsumerInputs = new Dictionary<string, string>
                {
                    { "url", "https://requestb.in/12h6lw21" }
                },
                EventType = "workitem.created",
                PublisherId = "tfs",
                PublisherInputs = new Dictionary<string, string>
                {
                    { "projectId", project.Id.ToString() }
                },
            };

            Subscription newSubscription = serviceHooksClient.CreateSubscriptionAsync(subscriptionParameters).Result;

            LogSubscription(newSubscription);

            return newSubscription;
        }

        /// <summary>
        /// Create a new web hook subscription that triggers a notification on all work item updates across the entire account (project collection).
        /// 
        /// This requires account/collection level administrator permissions.
        /// 
        /// </summary>
        [ClientSampleMethod]
        public Subscription CreateAccountWideWebHooksSubscription()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            ServiceHooksPublisherHttpClient serviceHooksClient = connection.GetClient<ServiceHooksPublisherHttpClient>();

            Subscription subscriptionParameters = new Subscription()
            {
                ConsumerId = "webHooks",
                ConsumerActionId = "httpRequest",
                ConsumerInputs = new Dictionary<string, string>
                    {
                        { "url", "https://requestb.in/12h6lw21" }
                    },
                EventType = "workitem.updated",
                PublisherId = "tfs"
            };

            Subscription newSubscription = serviceHooksClient.CreateSubscriptionAsync(subscriptionParameters).Result;

            LogSubscription(newSubscription);

            return newSubscription;
        }

        protected void LogSubscription(Subscription subscription)
        {
            Context.Log(" {0} {1} {2} {3}",
                subscription.Id.ToString().PadRight(8),
                subscription.EventDescription.PadRight(40),
                subscription.ConsumerId.PadRight(15),
                subscription.ModifiedDate.ToShortDateString().PadRight(10),
                subscription.ModifiedBy?.DisplayName);
        }
    }
}