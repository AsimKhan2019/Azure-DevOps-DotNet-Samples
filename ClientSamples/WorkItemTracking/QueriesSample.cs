using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vsts.ClientSamples.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.Queries)]
    public class QueriesSample : ClientSample
    {

        public QueriesSample(ClientSampleContext context) : base(context)
        {
        }

        [ClientSampleMethod]
        public QueryHierarchyItem GetQueryByName(string project, string queryName)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            QueryHierarchyItem query = workItemTrackingClient.GetQueryAsync(project, queryName).Result;

            if (query != null)
            {
                return query;
            }
            else
            {
                throw new NullReferenceException("Query '" + queryName + "' not found in project");
            }
        }

        [ClientSampleMethod]
        public WorkItemQueryResult ExecuteQuery(Guid queryId)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemQueryResult queryResult = workItemTrackingClient.QueryByIdAsync(queryId).Result;

            if (queryResult != null && queryResult.WorkItems.Count() > 0)
            {
                return queryResult;
            }
            else
            {
                throw new NullReferenceException("Query '" + queryId.ToString().ToLower() + "' did not find any results");
            }
        }

        [ClientSampleMethod]
        public WorkItemQueryResult ExecuteByWiql(Wiql wiql, string project)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemQueryResult queryResult = workItemTrackingClient.QueryByWiqlAsync(wiql, project).Result;

            return queryResult;
        }

        [ClientSampleMethod]
        public IEnumerable<WorkItem> GetWorkItemsFromQuery(string projectName, string queryName)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            QueryHierarchyItem queryItem;

            try
            {
                // get the query object based on the query name and project
                queryItem = workItemTrackingClient.GetQueryAsync(projectName, queryName).Result;
            }
            catch (Exception ex)
            {
                // query was likely not found
                throw ex;
            }

            // now we have the query, so let'ss execute it and get the results
            WorkItemQueryResult queryResult = workItemTrackingClient.QueryByIdAsync(queryItem.Id).Result;

            if (queryResult.WorkItems.Count() == 0)
            {
                return new List<WorkItem>();
            }
            else
            {
                // need to get the list of our work item id's and put them into an array
                int[] workItemIds = queryResult.WorkItems.Select<WorkItemReference, int>(wif => { return wif.Id; }).ToArray();

                // build a list of the fields we want to see
                string[] fields = new []
                    {
                        "System.Id",
                        "System.Title",
                        "System.State"
                    };

                IEnumerable<WorkItem> workItems = workItemTrackingClient.GetWorkItemsAsync(workItemIds, fields, queryResult.AsOf).Result;

                return workItems;
            }
        }

        public IEnumerable<WorkItem> GetWorkItemsFromWiql(string project, string wiqlString = null)
        {
            // create a query to get your list of work items needed
            Wiql wiql = new Wiql()
            {
                Query = (string.IsNullOrEmpty(wiqlString) ? "Select [State], [Title] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] = 'New' " +
                        "Order By [State] Asc, [Changed Date] Desc" : wiqlString)
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // execute the query
            WorkItemQueryResult queryResult = workItemTrackingClient.QueryByWiqlAsync(wiql).Result;

            // check to make sure we have some results
            if (queryResult.WorkItems.Count() == 0)
            {
                return new List<WorkItem>();
            }
            else
            {
                // need to get the list of our work item id's and put them into an array
                int[] workItemIds = queryResult.WorkItems.Select<WorkItemReference, int>(wif => { return wif.Id; }).ToArray();

                // build a list of the fields we want to see
                string[] fields = new []
                    {
                        "System.Id",
                        "System.Title",
                        "System.State"
                    };

                IEnumerable<WorkItem> workItems = workItemTrackingClient.GetWorkItemsAsync(workItemIds, fields, queryResult.AsOf).Result;

                return workItems;
            }
        }

    }
}
