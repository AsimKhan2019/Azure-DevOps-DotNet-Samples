using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using Vsts.ClientSamples;

namespace Vsts.ClientSamples.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.WorkItems)]
    public class WorkItemsSample : ClientSample
    {

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsByIDs(IEnumerable<int> ids)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> results = workItemTrackingClient.GetWorkItemsAsync(ids).Result;

            return results;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsWithSpecificFields(IEnumerable<int> ids)
        {
            var fields = new string[] {
                "System.Id",
                "System.Title",
                "System.WorkItemType",
                "Microsoft.VSTS.Scheduling.RemainingWork"
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> results = workItemTrackingClient.GetWorkItemsAsync(ids, fields).Result;

            return results;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsAsOfDate(IEnumerable<int> ids, DateTime asOfDate)
        {
            var fields = new string[] {
               "System.Id",
               "System.Title",
               "System.WorkItemType",
               "Microsoft.VSTS.Scheduling.RemainingWork"
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> results = workItemTrackingClient.GetWorkItemsAsync(ids, fields, asOfDate).Result;

            return results;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsWithLinksAndAttachments(IEnumerable<int> ids)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> results = workItemTrackingClient.GetWorkItemsAsync(ids, null, null, WorkItemExpand.All).Result;

            return results;
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItem(int id)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();  
             
            WorkItem result = workItemTrackingClient.GetWorkItemAsync(id).Result;

            return result;                                      
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItemWithLinksAndAttachments(int id)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.GetWorkItemAsync(id, null, null, WorkItemExpand.Relations).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItemFullyExpanded(int id)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem CreateWorkItem()
        {
            string projectName = ClientSampleHelpers.GetDefaultProject(this.Context).Name;
            string title = "Sample task";

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
                }
            );            

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.CreateWorkItemAsync(patchDocument, projectName, "Task").Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem CreateWorkItemWithWorkItemLink(string projectName, string title, string description, string linkUrl)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.CreateWorkItemAsync(patchDocument, projectName, "Task").Result;

            return result;
        }

        [ClientSampleMethod]
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.CreateWorkItemAsync(patchDocument, projectName, "Task", null, true).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem UpdateWorkItemFields(int id)
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem MoveWorkItem(int id, string targetProject, string targetAreaPath, string targetIterationPath)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.TeamProject",
                    Value = targetProject
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.AreaPath",
                    Value = targetAreaPath
                }
            );

            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = targetIterationPath
                }
            );           

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem ChangeWorkItemTypeToUserStory(int id)
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem AddTags(int id, IEnumerable<string> tags)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Tags",
                    Value = string.Join(";", tags)
                }
            );

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem AddLinkToOtherWorkItem(int id, int targetId)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get work target work item
            WorkItem targetWorkItem = workItemTrackingClient.GetWorkItemAsync(targetId).Result;

            JsonPatchDocument patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                new JsonPatchOperation() {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new {
                        rel = "System.LinkTypes.Dependency-forward",
                        url = targetWorkItem.Url,
                        attributes = new {
                            comment = "Making a new link for the dependency"
                        }
                    }
                }
            );

            
            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem AddAttachment(int id, string filePath)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // upload attachment to store and get a reference to that file
            AttachmentReference attachmentReference = workItemTrackingClient.CreateAttachmentAsync(filePath).Result;

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

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem UpdateWorkItemAddHyperLink(int id, Uri url = null, string urlComment = null)
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
                        url = (url == null ? new Uri("http://www.visualstudio.com/team-services") : url),
                        attributes = new { comment = (string.IsNullOrEmpty(urlComment) ? "Visual Studio Team Services" : urlComment) }
                    }
                }
            );

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        [ClientSampleMethod]
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id, null, true).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemDelete DeleteWorkItem(int id)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete results = workItemTrackingClient.DeleteWorkItemAsync(id, false).Result;

            return results;
        }

        [ClientSampleMethod]
        public void UpdateWorkItemsByQueryResults(WorkItemQueryResult workItemQueryResult, string changedBy)
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            foreach (WorkItemReference workItemReference in workItemQueryResult.WorkItems)
            {
                WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, workItemReference.Id).Result;
            }
        }
 
    }
}
