using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

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
        public Subscription CreateGitPushWebHooksSubscription()
        {
            VssConnection connection = this.Context.Connection;
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            
            ServiceHooksPublisherHttpClient client = connection.GetClient<ServiceHooksPublisherHttpClient>();

            Subscription gitPushSubscription = new Subscription()
            {
                PublisherId = "tfs",
                EventType = "git.push",
                ResourceVersion = "1.0",
                PublisherInputs = new Dictionary<string, string>()
                {
                    { "projectId", project.Id.ToString() }
                },
                ConsumerId = "webHooks",
                ConsumerActionId = "httpRequest",
                ConsumerInputs = new Dictionary<string, string>()
                {
                    { "url", "https://someurl" }
                }
            };

            Subscription subscription = client.CreateSubscriptionAsync(gitPushSubscription).Result;

            LogSubscription(subscription);

            return subscription;
        }

        public void HandleIncomingGitPushEvent(WebHookEvent webHookEvent)
        {
            GitRefUpdate gitRefUpdate;

            // Check if the incoming event is a Git push event
            if (String.Equals(webHookEvent.EventType, "git.push"))
            {
                JObject resource = webHookEvent.Resource as JObject;
                if (resource != null)
                {
                    gitRefUpdate = resource.ToObject<GitRefUpdate>();

                    // TODO: show name, commit, etc
                }
            }
        }

        private static readonly IDictionary<string, Type> s_EventResourceTypes = new Dictionary<string, Type>()
        {
            { "git.push", typeof(GitRefUpdate) },
            { "workitem.updated", typeof(WorkItemUpdate) }
        };

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