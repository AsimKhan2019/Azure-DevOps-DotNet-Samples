using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
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
        }

        public WorkItemQueryResult ExecuteQuery(Guid queryId)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
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

        public WorkItemQueryResult ExecuteByWiql(Wiql wiql, string project)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
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
}
