using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking;


namespace Microsoft.Azure.DevOps.ClientSamples.Notification
{
    /// <summary>
    /// 
    /// Samples showing how to use the Notification client to manage notification subscriptions in Team Services.
    /// 
    /// For more details, see https://www.visualstudio.com/docs/integrate/api/notification/subscriptions
    /// 
    /// </summary>
    [ClientSample(NotificationApiConstants.AreaName, NotificationApiConstants.SubscriptionsResource.Name)]
    public class SubscriptionsSample : ClientSample
    {

        /// <summary>
        //  List the user's subscriptions, creates, updates, and deletes a new subscription. 
        /// </summary>
        [ClientSampleMethod]
        public NotificationSubscription CreateUpdateDeleteSubscription()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();

            //
            // Part 1: create a subscription to get notified about certain pull request events
            // 

            // Create parameters for the new subscription
            NotificationSubscriptionCreateParameters createParams = new NotificationSubscriptionCreateParameters()
            {
                Description = "Someone is waiting on one of my pull requests",
                Filter = new ExpressionFilter("ms.vss-code.git-pullrequest-event") {
                    FilterModel = new ExpressionFilterModel()
                    {
                        Clauses = new ExpressionFilterClause[]
                        {
                            new ExpressionFilterClause()
                            {
                                FieldName = "Vote",
                                Operator = "Changes to",
                                Value = "Waiting for author",
                                LogicalOperator = "And"
                            },
                            new ExpressionFilterClause()
                            {
                                FieldName = "Created by",
                                Operator = "=",
                                Value = "[Me]",
                                LogicalOperator = "And"
                            }
                        }
                    }
                },
                Channel = new UserSubscriptionChannel()
            };

            // Scope to only events from one project
            ProjectHttpClient projectClient = this.Context.Connection.GetClient<ProjectHttpClient>();

            Guid projectId;
            String projectName;
  
            if (!this.Context.TryGetValue<String>("projectName", out projectName))
            {
                // Get the ID of the first project
                projectId = projectClient.GetProjects().Result.First().Id;
            }
            else
            {
                // Get the ID of the specified project
                projectId = projectClient.GetProject(projectName).Result.Id;
            }

            createParams.Scope = new SubscriptionScope() { Id = projectId };

            NotificationSubscription newSubscription = notificationClient.CreateSubscriptionAsync(createParams).Result;
            String subscriptionId = newSubscription.Id;

            Context.Log("New subscription created! ID: {0}", subscriptionId);

            //
            // Part 2: disable and delete the subscription
            // 

            // Disable the new subscription
            NotificationSubscriptionUpdateParameters updateParams = new NotificationSubscriptionUpdateParameters()
            {
                Status = SubscriptionStatus.Disabled
            };

            newSubscription = notificationClient.UpdateSubscriptionAsync(updateParams, subscriptionId).Result;

            Context.Log("Is subscription disabled? {0}", newSubscription.Status < 0);

            // Delete the subscription
            notificationClient.DeleteSubscriptionAsync(subscriptionId).SyncResult();

            // Try to get the subscription (should result in an exception)
            try
            {
                newSubscription = notificationClient.GetSubscriptionAsync(subscriptionId, SubscriptionQueryFlags.IncludeFilterDetails).Result;
            } catch (Exception e)
            {
                Context.Log("Unable to get the deleted subscription:" + e.Message);
            }

            // Try again (the default query flags says to return deleted subscriptions so this should work)
            newSubscription = notificationClient.GetSubscriptionAsync(subscriptionId).Result;

            return newSubscription;
        }

        public IEnumerable<IGrouping<string, NotificationSubscription>> GetSubscriptionsGroupedByEventType()
        {
            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();

            // Get existing subscriptions
            IEnumerable<NotificationSubscription> subscriptions = notificationClient.ListSubscriptionsAsync().Result;

            // Group subscriptions by event type
            var groupedSubscriptions = subscriptions.GroupBy<NotificationSubscription, String>(subscription =>
            {
                return String.IsNullOrEmpty(subscription.Filter.EventType) ? subscription.Filter.EventType : "";
            });

            // Create map of avaialble event types
            Dictionary<string, NotificationEventType> eventTypes = notificationClient.ListEventTypesAsync().Result.ToDictionary(
                eventType => { return eventType.Id; });

            // Show the subscriptions grouped by event type
            Context.Log("Custom subscriptions by event type");
            foreach (IGrouping<string, NotificationSubscription> group in groupedSubscriptions)
            {
                NotificationEventType eventType;
                if (eventTypes.TryGetValue(group.Key, out eventType))
                {
                    Context.Log("Event type {0}:", eventType.Name);
                    foreach (NotificationSubscription subscription in group)
                    {
                        Context.Log(" {0}, last modified: {1} by {2}",
                            subscription.Description,
                            subscription.ModifiedDate,
                            subscription.LastModifiedBy?.DisplayName);
                    }
                }
            }

            return groupedSubscriptions;
        }

        /// <summary>
        /// Returns all custom subscriptions for the caller.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<NotificationSubscription> ListCustomSubscriptions()
        {
            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();

            List<NotificationSubscription> subscriptions = notificationClient.ListSubscriptionsAsync().Result;

            foreach (var subscription in subscriptions)
            {
                LogSubscription(subscription);
            }

            return subscriptions;
        }

        /// <summary>
        /// Returns all custom subscriptions for the specified event type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<NotificationSubscription> QuerySubscriptionsByEventType()
        {
            String eventType;
            if (!Context.TryGetValue<string>("notification.subscriptions.eventType", out eventType))
            {
                eventType = "ms.vss-work.workitem-changed-event";
            }

            // Setup the query
            SubscriptionQuery query = new SubscriptionQuery()
            {
                Conditions = new[]
                {
                    new SubscriptionQueryCondition()
                    {
                        Flags = SubscriptionFlags.TeamSubscription,
                        Filter = new ExpressionFilter(eventType)
                    }
                }
            };

            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();

            IEnumerable<NotificationSubscription> subscriptions = notificationClient.QuerySubscriptionsAsync(query).Result;

            foreach(NotificationSubscription subscription in subscriptions)
            {
                LogSubscription(subscription);
            }

            return subscriptions;
        }

        /// <summary>
        /// Creates a custom personal subscription for the calling user. 
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public NotificationSubscription CreateSubscriptionForUser()
        {        
            NotificationHttpClient notificationClient = Context.Connection.GetClient<NotificationHttpClient>();

            // Query the available event types and find the first that can be used in a custom subscription
            List<NotificationEventType> eventTypes = notificationClient.ListEventTypesAsync().Result;
            NotificationEventType eventType = eventTypes.Find(e => { return e.CustomSubscriptionsAllowed; });

            NotificationSubscriptionCreateParameters createParams = new NotificationSubscriptionCreateParameters() {
                Description = "My first subscription!",
                Filter = new ExpressionFilter(eventType.Id),
                Channel = new UserSubscriptionChannel()
            };

            NotificationSubscription newSubscription = notificationClient.CreateSubscriptionAsync(createParams).Result;

            LogSubscription(newSubscription);

            return newSubscription;
        }

        /// <summary>
        /// Creates a custom team subscription.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public NotificationSubscription CreateSubscriptionForTeam()
        {
            NotificationSubscription newSubscription;

            WebApiTeamRef team = ClientSampleHelpers.FindAnyTeam(this.Context, null);

            NotificationHttpClient notificationClient = Context.Connection.GetClient<NotificationHttpClient>();

            // Query the available event types and find the first that can be used in a custom subscription
            List<NotificationEventType> eventTypes = notificationClient.ListEventTypesAsync().Result;
            NotificationEventType eventType = eventTypes.Find(e => { return e.CustomSubscriptionsAllowed; });

            NotificationSubscriptionCreateParameters createParams = new NotificationSubscriptionCreateParameters()
            {
                Description = "A subscription for our team",
                Filter = new ExpressionFilter(eventType.Id),
                Channel = new UserSubscriptionChannel(),
                Subscriber = new IdentityRef()
                {
                    Id = team.Id.ToString()
                }
            };

            newSubscription = notificationClient.CreateSubscriptionAsync(createParams).Result;

            LogSubscription(newSubscription);

            return newSubscription;
        }

        /// <summary>
        /// Returns all subscriptions for the specified team.
        /// </summary>
        /// <param name="projectName">Name or ID of the project that contains the team</param>
        /// <param name="teamName">Name of the team</param>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<NotificationSubscription> ListSubscriptionsForTeam()
        {
            WebApiTeamRef team = ClientSampleHelpers.FindAnyTeam(this.Context, null);

            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();

            IEnumerable<NotificationSubscription> subscriptions = notificationClient.ListSubscriptionsAsync(targetId: team.Id).Result;

            foreach (var subscription in subscriptions)
            {
                LogSubscription(subscription);
            }

            return subscriptions;
        }

        public IEnumerable<NotificationSubscription> ListSubscriptionsForGroup()
        {
            Guid groupId = Guid.Empty; // TODO fix

            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();

            // Return all subscriptions, includuing minimal details for subscriptions the caller doesn't have access to
            return notificationClient.ListSubscriptionsAsync(targetId: groupId,
                queryFlags: SubscriptionQueryFlags.AlwaysReturnBasicInformation).Result;
        }

        /// <summary>
        ///  Query for and show all team-managed subscriptions for the project.
        /// </summary>
        /// <param name="projectName"></param>
        [ClientSampleMethod]
        public void ShowAllTeamSubscriptions()
        {
            VssConnection connection = Context.Connection;

            //
            // Step 1: construct query to find all subscriptions belonging to teams in the project
            //

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get all teams in the project
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();
            IEnumerable<WebApiTeam> teams = teamClient.GetTeamsAsync(project.Id.ToString()).Result;

            // Construct a set of query conditions (one for each team)
            IEnumerable<SubscriptionQueryCondition> conditions = 
                teams.Select<WebApiTeam, SubscriptionQueryCondition>(team =>
                {
                    return new SubscriptionQueryCondition() { SubscriberId = team.Id };
                }
            );

            // Construct the query, making sure to return basic details for subscriptions the caller doesn't have read access to
            SubscriptionQuery query = new SubscriptionQuery()
            {
                Conditions = conditions,
                QueryFlags = SubscriptionQueryFlags.AlwaysReturnBasicInformation
            };

            //
            // Part 2: query and show the results
            //

            NotificationHttpClient notificationClient = connection.GetClient<NotificationHttpClient>();
            IEnumerable<NotificationSubscription> subscriptions = notificationClient.QuerySubscriptionsAsync(query).Result;

            var subscriptionsBySubscriber = subscriptions.GroupBy<NotificationSubscription, Guid>(sub => { return Guid.Parse(sub.Subscriber.Id); });

            foreach (var team in teams)
            {
                // Find the corresponding team for this group
                var group = subscriptionsBySubscriber.First(t => t.Key == team.Id);
                
                // Show the details for each subscription owned by this team 
                foreach (NotificationSubscription subscription in group)
                {
                    LogSubscription(subscription);
                }
            }

        }

        /// <summary>
        /// Follow a work item (get notified about certain updates to a work item)
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public NotificationSubscription FollowWorkItem()
        {
            NotificationSubscription newFollowSubscription;

            // Get a work item to follow. For this sample, just create a temporary work item.
            WorkItem newWorkItem;
            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                WorkItemsSample witSample = new WorkItemsSample();
                witSample.Context = this.Context;
                newWorkItem = witSample.CreateWorkItem();

                // Save the new work item so we can unfollow it later
                this.Context.SetValue<WorkItem>("$followedWorkItem", newWorkItem);
            }

            NotificationSubscriptionCreateParameters createParams = new NotificationSubscriptionCreateParameters()
            {
                Filter = new ArtifactFilter(null)
                {
                    ArtifactType = "WorkItem",
                    ArtifactId = newWorkItem.Id.ToString()
                }
            };

            VssConnection connection = Context.Connection;
            NotificationHttpClient notificationClient = Context.Connection.GetClient<NotificationHttpClient>();

            newFollowSubscription  = notificationClient.CreateSubscriptionAsync(createParams).Result;

            LogSubscription(newFollowSubscription);

            return newFollowSubscription;
        }
        
        /// <summary>
        /// Unfollow a workitem (stop getting notified about the certain updates to a work item)
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public void UnfollowWorkItem()
        {
            // Get the temporary work item created in the "follow work item" method above
            WorkItem workItem;
            if (!this.Context.TryGetValue<WorkItem>("$followedWorkItem", out workItem))
            {
                // should log an error
            }               
            else
            {
                VssConnection connection = Context.Connection;
                NotificationHttpClient notificationClient = Context.Connection.GetClient<NotificationHttpClient>();

                NotificationSubscription followSubscription;
                using (new ClientSampleHttpLoggerOutputSuppression())
                {

                    // We want to query for "artifact" (follow) subscription for the specified work item ID (for the calling user)
                    SubscriptionQuery query = new SubscriptionQuery()
                    {
                        Conditions = new[]
                        {
                            new SubscriptionQueryCondition()
                            {
                                Filter = new ArtifactFilter(null)
                                {
                                    ArtifactType = "WorkItem",
                                    ArtifactId = workItem.Id.ToString()
                                }
                            }
                        }
                    };

                    followSubscription = notificationClient.QuerySubscriptionsAsync(query).Result.FirstOrDefault();
                }

                LogSubscription(followSubscription);

                // Delete this subscription to "unfollow" the user from the work item
                notificationClient.DeleteSubscriptionAsync(followSubscription.Id).SyncResult();
            }           
        }

        /// <summary>
        /// Opts the calling user out of a team subscription. This creates a temporary team subscription for the purpose of opting out.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public SubscriptionUserSettings OptOutfTeamSubscription()
        {
            NotificationSubscription teamSubscription;

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                teamSubscription = CreateSubscriptionForTeam();
            }

            Guid teamMemberId = ClientSampleHelpers.GetCurrentUserId(Context);

            SubscriptionUserSettings userSettings = new SubscriptionUserSettings() { OptedOut = true };
            NotificationHttpClient notificationClient = this.Context.Connection.GetClient<NotificationHttpClient>();

            userSettings = notificationClient.UpdateSubscriptionUserSettingsAsync(userSettings, teamSubscription.Id, teamMemberId).Result;

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                notificationClient.DeleteSubscriptionAsync(teamSubscription.Id);
            }

            return userSettings;
        }

        protected void LogSubscription(NotificationSubscription subscription)
        {
            Context.Log(" {0} {1} {2} {3}",
                subscription.Id.PadRight(8),
                subscription.Description.PadRight(60),
                subscription.ModifiedDate.ToShortDateString().PadRight(10),
                subscription.LastModifiedBy?.DisplayName);
        }

    }

}
