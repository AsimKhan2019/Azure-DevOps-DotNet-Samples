using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.ServiceHooks
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
        /// Create and delete a new web hook subscription that triggers a notification on all new work items created in the specified project.
        /// </summary>
        [ClientSampleMethod]
        public Subscription CreateDeleteWebHooksSubscription()
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
            Guid subscriptionId = newSubscription.Id;

            LogSubscription(newSubscription);

            // Delete the subscription
            serviceHooksClient.DeleteSubscriptionAsync(subscriptionId).SyncResult();

            // Try to get the subscription (should result in an exception)
            try
            {
                newSubscription = serviceHooksClient.GetSubscriptionAsync(subscriptionId).Result;
            } catch (Exception e)
            {
                Context.Log("Unable to get the deleted subscription:" + e.Message);
            }

            return newSubscription;
        }


        /// <summary>
        /// Returns all custom subscriptions for the caller.
        /// </summary>
        [ClientSampleMethod]
        public IEnumerable<Subscription> ListSubscriptions()
        {
            VssConnection connection = Context.Connection;
            ServiceHooksPublisherHttpClient serviceHooksClient = connection.GetClient<ServiceHooksPublisherHttpClient>();

            IList<Subscription> subscriptions = serviceHooksClient.QuerySubscriptionsAsync().Result;

            foreach(var subscription in subscriptions)
            {
                LogSubscription(subscription);
            }

            return subscriptions;
        }
        
        
        /// <summary>
        /// Create and deletes a new Release Management web hook subscription
        /// </summary>
        [ClientSampleMethod]
        public Subscription CreateDeleteRMWebHooksSubscription()
        {
            // Get the project to create the subscription in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get the client
            VssConnection connection = Context.Connection;
            ServiceHooksPublisherHttpClient serviceHooksClient = connection.GetClient<ServiceHooksPublisherHttpClient>();

            // Get the list of publishers
            IList<VisualStudio.Services.ServiceHooks.WebApi.Publisher> publishers = serviceHooksClient.GetPublishersAsync().Result;

            // Find the Release Management publisher and get its ServiceInstanceType
            VisualStudio.Services.ServiceHooks.WebApi.Publisher rmPublisher = publishers.First(p => p.Id == "rm");
            Guid rmServiceInstance = Guid.Parse(rmPublisher.ServiceInstanceType);

            // Get a new client using the RM ServiceInstanceType
            ServiceHooksPublisherHttpClient rmServiceHooksClient = connection.GetClient<ServiceHooksPublisherHttpClient>(rmServiceInstance);

            Subscription subscriptionParameters = new Subscription()
            {
                ConsumerId = "webHooks",
                ConsumerActionId = "httpRequest",
                ConsumerInputs = new Dictionary<string, string>
                {
                    { "url", "https://requestb.in/12h6lw21" }
                },
                EventType = "ms.vss-release.release-created-event",
                PublisherId = "rm",
                PublisherInputs = new Dictionary<string, string>
                {
                    { "projectId", project.Id.ToString() }
                },
            };
            Subscription newSubscription = rmServiceHooksClient.CreateSubscriptionAsync(subscriptionParameters).Result;
            Guid subscriptionId = newSubscription.Id;

            LogSubscription(newSubscription);

            // Delete the subscription
            rmServiceHooksClient.DeleteSubscriptionAsync(subscriptionId).SyncResult();

            // Try to get the subscription (should result in an exception)
            try
            {
                newSubscription = rmServiceHooksClient.GetSubscriptionAsync(subscriptionId).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the deleted subscription:" + e.Message);
            }

            return newSubscription;
        }

        
        /// <summary>
        /// Create and delete a new web hook subscription that triggers a notification on all work item updates across the entire account (project collection).
        /// 
        /// This requires account/collection level administrator permissions.
        /// 
        /// </summary>
        [ClientSampleMethod]
        public Subscription CreateDeleteAccountWideWebHooksSubscription()
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
            Guid subscriptionId = newSubscription.Id;

            LogSubscription(newSubscription);

            // Delete the subscription
            serviceHooksClient.DeleteSubscriptionAsync(subscriptionId).SyncResult();

            // Try to get the subscription (should result in an exception)
            try
            {
                newSubscription = serviceHooksClient.GetSubscriptionAsync(subscriptionId).Result;
            }
            catch (Exception e)
            {
                Context.Log("Unable to get the deleted subscription:" + e.Message);
            }

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