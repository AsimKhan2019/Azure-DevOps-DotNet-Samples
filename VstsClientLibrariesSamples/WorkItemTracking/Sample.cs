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
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class Sample
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;
        
        public Sample(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public string QueryAndUpdateWorkItems()
        {
            //create a query to get your list of work items needed
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                       "From WorkItems " +
                       "Where [Work Item Type] = 'Bug' " +
                       "And [System.TeamProject] = '" + _configuration.Project + "' " +
                       "And [System.State] = 'New' " +
                       "Order By [State] Asc, [Changed Date] Desc"
            };

            //create a patchDocument that is used to update the work items
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            //change the backlog priority
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.BacklogPriority",
                    Value = "2",
                    From = _configuration.Identity
                }
            );

            //move the state to active
            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/System.State",
                   Value = "Active",
                   From = _configuration.Identity
               }
            );

            //add some comments
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "comment from client lib sample code",
                    From = _configuration.Identity
                }
            );

            //create instance of work item tracking http client
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                //execute the query
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

                //check to make sure we have some results
                if (workItemQueryResult == null || workItemQueryResult.WorkItems.Count() == 0)
                {                    
                    throw new NullReferenceException("Wiql '" + wiql.Query + "' did not find any results");                   
                }

                //loop thru the work item results and update each work item
                foreach (WorkItemReference workItemReference in workItemQueryResult.WorkItems)
                {
                    WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemReference.Id).Result;
                }
            }

            wiql = null;
            patchDocument = null;

            return "success";           
        }

    }
}