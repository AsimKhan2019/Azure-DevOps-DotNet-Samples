using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;

namespace VstsClientLibrariesSamples.QueryAndUpdateWorkItems
{
    public class Scenerio
    {
        readonly IConfiguration _configuration;
        
        public Scenerio(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ExecuteFullScenerio()
        {
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                       "From WorkItems " +
                       "Where [Work Item Type] = 'Bug' " +
                       "And [System.TeamProject] = '" + _configuration.ProjectName + "' " +
                       "And [System.State] = 'New' " +
                       "Order By [State] Asc, [Changed Date] Desc"
            };

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.BacklogPriority",
                    Value = "2",
                    From = _configuration.Identity
                }
            );

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/System.State",
                   Value = "Active",
                   From = _configuration.Identity
               }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "comment from client lib sample code",
                    From = _configuration.Identity
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_configuration.Uri, _configuration.Credentials))
            {
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

                if (workItemQueryResult == null || workItemQueryResult.WorkItems.Count() == 0)
                {                    
                    throw new NullReferenceException("Wiql '" + wiql.Query + "' did not find any results");                   
                }

                foreach (WorkItemReference workItemReference in workItemQueryResult.WorkItems)
                {
                    WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemReference.Id).Result;
                }
            }

            wiql = null;
            patchDocument = null;

            return true;            
        }

    }
}
