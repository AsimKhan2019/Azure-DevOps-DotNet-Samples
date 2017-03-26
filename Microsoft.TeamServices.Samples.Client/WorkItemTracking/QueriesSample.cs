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

        [ClientSampleMethod]
        public QueryHierarchyItem GetQueryByName()
        {
            string project = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string queryName = "Shared Queries/Current Sprint";

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            QueryHierarchyItem query = workItemTrackingClient.GetQueryAsync(project, queryName).Result;

            if (query != null)
            {
                return query;
            }
            else
            {
                throw new Exception(String.Format("Query '{0}' not found", queryName));
            }
        }

        [ClientSampleMethod]
        public WorkItemQueryResult ExecuteQuery()
        {
            Guid queryId = Guid.Parse("6e511ae8-aafe-455a-b318-a4158bbd0f1e"); // TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemQueryResult queryResult = workItemTrackingClient.QueryByIdAsync(queryId).Result;

            return queryResult;
        }

        [ClientSampleMethod]
        public WorkItemQueryResult ExecuteByWiql()
        {
            string project = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            Wiql wiql = new Wiql()
            {
                Query = "Select ID, Title from Issue where (State = 'Active') order by Title"
            };            

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemQueryResult queryResult = workItemTrackingClient.QueryByWiqlAsync(wiql, project).Result;

            return queryResult;
        }

        [ClientSampleMethod]
        public IEnumerable<WorkItem> GetWorkItemsFromQuery()
        {
            string project = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string queryName = "Shared Queries/Current Sprint";

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            QueryHierarchyItem queryItem;

            try
            {
                // get the query object based on the query name and project
                queryItem = workItemTrackingClient.GetQueryAsync(project, queryName).Result;
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

        public IEnumerable<WorkItem> GetWorkItemsFromWiql()
        {
            string project = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // create a query to get your list of work items needed
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] = 'New' " +
                        "Order By [State] Asc, [Changed Date] Desc"
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

                IEnumerable<WorkItem> workItems = workItemTrackingClient.GetWorkItemsAsync(
                    workItemIds, 
                    fields, 
                    queryResult.AsOf).Result;

                return workItems;
            }
        }

    }
}
