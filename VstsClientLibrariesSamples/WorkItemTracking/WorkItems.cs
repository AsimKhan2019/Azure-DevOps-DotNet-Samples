using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class WorkItems
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public WorkItems(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public string UpdateWorkItemsByQueryResults(WorkItemQueryResult workItemQueryResult, string changedBy)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.BacklogPriority",
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

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/System.State",
                   Value = "Active",
                   From = changedBy
               }
           );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                foreach (WorkItemReference workItemReference in workItemQueryResult.WorkItems)
                {
                    WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemReference.Id).Result;                   
                }
            }            

            patchDocument = null;

            return "success";
        }

        public string CreateWorkItem(string projectName)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Authorization Errors"
                }
            );

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/Microsoft.VSTS.TCM.ReproSteps",
                   Value = "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http:// msdn.microsoft.com/en-us/library/live/hh826547.aspx"
               }
            );

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/Microsoft.VSTS.Common.Priority",
                   Value = "1"
               }
            );

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/Microsoft.VSTS.Common.Severity",
                   Value = "2 - High"
               }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {              
                WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Bug").Result;               
            }

            patchDocument = null;

            return "success";
        }

        public string UpdateWorkItem(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "adding some history"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Priority",
                    Value = "1"                  
                }                      
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {                
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            }

            patchDocument = null;

            return "success";
        }
        
        public string UpdateWorkItemUsingByPassRules(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.AssignedTo",
                    Value = "Invalid User"
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id, null, true).Result;
            }

            patchDocument = null;

            return "success";
        }
        
        public string GetWorkItem(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All).Result;
            }

            return "success";
        }
               
        public string AddLink(int id, int linkToId)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new {
                        rel = "System.LinkTypes.Dependency-forward",
                        url = _configuration.UriString + "/_apis/wit/workItems/" + linkToId.ToString(),
                        attributes = new { comment = "Making a new link for the dependency" }
                    }
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            }

            return "success";
        }
    }
}
