using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace VstsClientLibrariesSamples.QueryAndUpdateWorkItems
{
    public class WorkItems
    {
        readonly IConfiguration _configuration;

        public WorkItems(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void UpdateWorkItemsByQueryResults(WorkItemQueryResult workItemQueryResult, string changedBy)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Replace,
                    Path = "Microsoft.VSTS.Common.BacklogPriority",
                    Value = "2",
                    From = changedBy
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "comment from client lib sample code",
                    From = changedBy
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_configuration.Uri, _configuration.Credentials))
            {
                foreach (WorkItemReference workItemReference in workItemQueryResult.WorkItems)
                {
                    WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemReference.Id).Result;                   
                }
            }

            patchDocument = null;
        }
    }
}
