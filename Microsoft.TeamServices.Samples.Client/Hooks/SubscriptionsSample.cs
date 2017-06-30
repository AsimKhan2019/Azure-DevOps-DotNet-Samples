using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;

namespace Microsoft.TeamServices.Samples.Client.Hooks
{
    [ClientSample(ServiceHooksPublisherApiConstants.AreaName, "Subscriptions")]
    public class SubscriptionsSample : ClientSample
    {
        /// <summary>
        /// Create a service hooks subscription to get notified about Git push events to a particular repo.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public Subscription CreateWebHooksSubscription()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            VssConnection connection = this.Context.Connection;
            ServiceHooksPublisherHttpClient client = connection.GetClient<ServiceHooksPublisherHttpClient>();

            Subscription gitPushSubscription = new Subscription()
            {
                PublisherId = "tfs",
                EventType = "git.push",
                ResourceVersion = "1.0",
                PublisherInputs = new Dictionary<string, string>()
                {
                    { "projectId", project.Id.ToString() },
                    { "repoId", "tbd" },
                },
                ConsumerId = "webHooks",
                ConsumerActionId = "httpRequest",
                ConsumerInputs = new Dictionary<string, string>()
                {
                    { "url", "https://someurl" }
                }
            };

            Subscription[] subscriptions = new Subscription[]
            {
                gitPushSubscription
            };

            foreach (var sub in subscriptions)
            {
                
            }

            Subscription newSubscription = client.CreateSubscriptionAsync(gitPushSubscription).Result;
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