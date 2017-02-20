using System;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class Queries
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public Queries(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public QueryHierarchyItem GetQueryByName(string project, string queryName)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            QueryHierarchyItem query = workItemTrackingHttpClient.GetQueryAsync(project, queryName).Result;

            if (query != null)
            {
                return query;
            }
            else
            {
                throw new NullReferenceException("Query '" + queryName + "' not found in project");
            }
        }

        public WorkItemQueryResult ExecuteQuery(Guid queryId)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemQueryResult queryResult = workItemTrackingHttpClient.QueryByIdAsync(queryId).Result;

            if (queryResult != null && queryResult.WorkItems.Count() > 0)
            {
                return queryResult;
            }
            else
            {
                throw new NullReferenceException("Query '" + queryId.ToString().ToLower() + "' did not find any results");
            }
        }

        public WorkItemQueryResult ExecuteByWiql(Wiql wiql, string project)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemQueryResult queryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql, project).Result;

            if (queryResult != null && queryResult.WorkItems.Count() > 0)
            {
                return queryResult;
            }
            else
            {
                throw new NullReferenceException("Wiql '" + wiql.Query + "' did not find any results");
            }
        }
    }
}
