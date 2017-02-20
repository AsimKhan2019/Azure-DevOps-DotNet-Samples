using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class WorkItems
    {
        private readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public WorkItems(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public List<WorkItem> GetWorkItemsByIDs(IEnumerable<int> ids)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids).Result;
            return results;
        }

        public List<WorkItem> GetWorkItemsWithSpecificFields(IEnumerable<int> ids)
        {
            var fields = new string[] {
                "System.Id",
                "System.Title",
                "System.WorkItemType",
                "Microsoft.VSTS.Scheduling.RemainingWork"
            };    

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields).Result;
            return results;
        }

        public List<WorkItem> GetWorkItemsAsOfDate(IEnumerable<int> ids, DateTime asOfDate)
        {
            var fields = new string[] {
               "System.Id",
               "System.Title",
               "System.WorkItemType",
               "Microsoft.VSTS.Scheduling.RemainingWork"
            };

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields, asOfDate).Result;
            return results;
        }

        public List<WorkItem> GetWorkItemsWithLinksAndAttachments(IEnumerable<int> ids)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids, null, null, WorkItemExpand.All).Result;
            return results;
        }

        public WorkItem GetWorkItem(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();   
            WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id).Result;
            return result;                                      
        }

        public WorkItem GetWorkItemWithLinksAndAttachments(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id, null, null, WorkItemExpand.Relations).Result;
            return result;
        }

        public WorkItem GetWorkItemFullyExpanded(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All).Result;
            return result;
        }

        public void GetDefaultValues(string type, string project)
        {
            
        }

        public WorkItem CreateWorkItem(string projectName)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "JavaScript implementation for Microsoft Account"
                }
            );            

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Task").Result;
            return result;
        }

        public WorkItem CreateWorkItemWithWorkItemLink(string projectName, string linkUrl)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "JavaScript implementation for Microsoft Account"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork",
                    Value = "4"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = "Follow the code samples from MSDN"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "Jim has the most context around this."
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "System.LinkTypes.Hierarchy-Reverse",
                        url = linkUrl,
                        attributes = new
                        {
                            comment = "decomposition of work"
                        }
                    }
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Task").Result;
            return result;
        }

        public WorkItem CreateWorkItemByPassingRules(string projectName)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "JavaScript implementation for Microsoft Account"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.CreatedDate",
                    Value = "6/1/2016"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.CreatedBy",
                    Value = "Art VanDelay"
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Task", null, true).Result;
            return result;
        }
              
        public WorkItem UpdateWorkItemUpdateField(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = "1"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Priority",
                    Value = "2"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "Changing priority"
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemMoveWorkItem(int id, string teamProject, string areaPath, string iterationPath)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.TeamProject",
                    Value = teamProject
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.AreaPath",
                    Value = areaPath
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = iterationPath
                }
            );           

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemChangeWorkItemType(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.WorkItemType",
                    Value = "User Story"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.State",
                    Value = "Active"
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemAddTag(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Tags",
                    Value = "Tag1; Tag2"
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemAddLink(int id, int linkToId)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new {
                        rel = "System.LinkTypes.Dependency-forward",
                        url = _configuration.UriString + "/_apis/wit/workItems/" + linkToId.ToString(),
                        attributes = new {
                            comment = "Making a new link for the dependency"
                        }
                    }
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemUpdateLink(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = "1"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Replace,
                    Path = "/relations/0/attributes/comment",
                    Value = "Adding traceability to dependencies"                  
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemRemoveLink(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = "1"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Remove,
                    Path = "/relations/0"
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemAddAttachment(int id, string filePath)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // upload attachment to attachment store and 
            // get a reference to that file
            AttachmentReference attachmentReference = workItemTrackingHttpClient.CreateAttachmentAsync(filePath).Result;

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = "1"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "Adding the necessary spec"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "AttachedFile",
                        url = attachmentReference.Url,
                        attributes = new { comment = "VanDelay Industries - Spec" }
                    }
                }
            );

            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemRemoveAttachment(int id, string rev)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Test,
                    Path = "/rev",
                    Value = "2"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Remove,
                    Path = "/relations/" + rev
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemAddHyperLink(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Test,
                   Path = "/rev",
                   Value = "1"
               }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "Hyperlink",
                        url = "http://www.visualstudio.com/team-services",
                        attributes = new { comment = "Visaul Studio Team Services" }
                    }
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemAddCommitLink(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Test,
                   Path = "/rev",
                   Value = "1"
               }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "ArtifactLink",
                        url = "vstfs:///Git/Commit/1435ac99-ba45-43e7-9c3d-0e879e7f2691%2Fd00dd2d9-55dd-46fc-ad00-706891dfbc48%2F3fefa488aac46898a25464ca96009cf05a6426e3",
                        attributes = new { comment = "Fixed in Commit" }
                    }
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
            return result;
        }

        public WorkItem UpdateWorkItemUsingByPassRules(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() { 
                    Operation = Operation.Add,
                    Path = "/fields/System.CreatedBy",
                    Value = "Foo <Foo@hotmail.com>"
                }
            );

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id, null, true).Result;
            return result;
        }

        public WorkItemDelete DeleteWorkItem(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemDelete results = workItemTrackingHttpClient.DeleteWorkItemAsync(id, false).Result;
            return results;
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

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            foreach (WorkItemReference workItemReference in workItemQueryResult.WorkItems)
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, workItemReference.Id).Result;
            }

            patchDocument = null;

            return "success";
        }
 
    }
}
