using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace VstsClientLibrariesSamples.QueryAndUpdateWorkItems
{
    public class Queries
    {
        readonly IConfiguration _configuration;

        public Queries(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public QueryHierarchyItem GetQueryByName(Guid projectId, string queryName)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_configuration.Uri, _configuration.Credentials))
            {
                QueryHierarchyItem query = workItemTrackingHttpClient.GetQueryAsync(projectId, queryName).Result;

                if (query != null)
                {
                    return query;
                }
                else
                {
                    throw new NullReferenceException("Query '" + queryName + "' not found in project");
                }
            }
        }

        public WorkItemQueryResult ExecuteQuery(Guid queryId)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_configuration.Uri, _configuration.Credentials))
            {

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
        }

        public WorkItemQueryResult ExecuteByWiql(Wiql wiql)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_configuration.Uri, _configuration.Credentials))
            {
                WorkItemQueryResult queryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

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
}
