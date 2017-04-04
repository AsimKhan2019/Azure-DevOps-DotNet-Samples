using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTracking
{
    /// <summary>
    /// Client samples for managing work items in Team Services and Team Foundation Server.
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.WorkItems)]
    public class WorkItemsSample : ClientSample
    {

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsByIDs()
        {
            int[] workitemIds = new int[] { 1, 5, 6, 10 };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> workitems = workItemTrackingClient.GetWorkItemsAsync(workitemIds).Result;

            foreach (var workitem in workitems)
            {
                Console.WriteLine(" {0}: {1}", workitem.Id, workitem.Fields["System.Title"]);
            }

            return workitems;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsWithSpecificFields()
        {
            int[] workitemIds = new int[] { 1, 5, 6, 10, 22, 50 };

            string[] fieldNames = new string[] {
                "System.Id",
                "System.Title",
                "System.WorkItemType",
                "Microsoft.VSTS.Scheduling.RemainingWork"
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> workitems = workItemTrackingClient.GetWorkItemsAsync(workitemIds, fieldNames).Result;

            foreach (var workitem in workitems)
            {
                Console.WriteLine(workitem.Id);
                foreach (var fieldName in fieldNames)
                {
                    Console.Write("  {0}: {1}", fieldName, workitem.Fields[fieldName]);
                }
            }

            return workitems;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsAsOfDate()
        {
            int[] workitemIds = new int[] { 1, 5, 6, 10, 22, 50 };

            string[] fieldNames = new string[] {
               "System.Id",
               "System.Title",
               "System.WorkItemType",
               "Microsoft.VSTS.Scheduling.RemainingWork"
            };

            DateTime asOfDate = new DateTime(2016, 12, 31);

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> workitems = workItemTrackingClient.GetWorkItemsAsync(workitemIds, fieldNames, asOfDate).Result;

            foreach (var workitem in workitems)
            {
                Console.WriteLine(workitem.Id);
                foreach (var fieldName in fieldNames)
                {
                    Console.Write("  {0}: {1}", fieldName, workitem.Fields[fieldName]);
                }
            }

            return workitems;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsWithLinksAndAttachments()
        {
            int[] workitemIds = new int[] { 1, 5, 6, 10, 22, 50 };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> workitems = workItemTrackingClient.GetWorkItemsAsync(workitemIds, expand: WorkItemExpand.Links | WorkItemExpand.Relations).Result;
            
            foreach(var workitem in workitems)
            {
                Console.WriteLine("Work item {0}", workitem.Id);

                foreach (var relation in workitem.Relations)
                {
                    Console.WriteLine("  {0} {1}", relation.Rel, relation.Url);
                }
            }


            return workitems;
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItemById()
        {
            int id = 12;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();  
             
            WorkItem workitem = workItemTrackingClient.GetWorkItemAsync(id).Result;

            foreach (var field in workitem.Fields)
            {
                Console.WriteLine("  {0}: {1}", field.Key, field.Value);
            }

            return workitem;                                      
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItemFullyExpanded()
        {
            int id = 5;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem workitem = workItemTrackingClient.GetWorkItemAsync(id, expand: WorkItemExpand.All).Result;

            Console.WriteLine(workitem.Id);
            Console.WriteLine("Fields: ");
            foreach (var field in workitem.Fields)
            {
                Console.WriteLine("  {0}: {1}", field.Key, field.Value);
            }

            Console.WriteLine("Relations: ");
            foreach (var relation in workitem.Relations)
            {
                Console.WriteLine("  {0} {1}", relation.Rel, relation.Url);
            }

            return workitem;
        }

        [ClientSampleMethod]
        public WorkItem CreateWorkItem()
        {   
            // Construct the object containing field values required for the new work item
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Sample task"
                }
            );            

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get the project to create the sample work item in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Create the new work item
            WorkItem newWorkItem = workItemTrackingClient.CreateWorkItemAsync(patchDocument, project.Id, "Task").Result;

            Console.WriteLine("Created work item ID {0} (1}", newWorkItem.Id, newWorkItem.Fields["System.Title"]);

            // Save this newly created for later samples
            Context.SetValue<WorkItem>("$newWorkItem", newWorkItem);

            return newWorkItem;
        }

        [ClientSampleMethod]
        public WorkItem CreateAndLinkToWorkItem()
        {
            string title = "My new work item with links";
            string description = "This is a new work item that has a link also created on it.";
            string linkUrl = "https://integrate.visualstudio.com";

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
                    Value = description
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

            // Get the project to create the sample work item in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            WorkItem result = workItemTrackingClient.CreateWorkItemAsync(patchDocument, project.Name, "Task").Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem BypassRulesOnCreate()
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

            // Get the project to create the sample work item in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            WorkItem result = workItemTrackingClient.CreateWorkItemAsync(patchDocument, project.Name, "Task", bypassRules: true).Result;



            return result;
        }

        [ClientSampleMethod]
        public WorkItem ChangeFieldValue(int id)
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
        public WorkItem MoveToAnotherProject()
        {
            int id = -1;
            string targetProject = null;
            string targetAreaPath = null;
            string targetIterationPath = null;

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
        public WorkItem ChangeType()
        {
            int id = 12;
            string newType = "User Story";

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
        public WorkItem AddTags()
        {
            int id = 12;
            string[] tags = { "teamservices", "client", "sample" };

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
        public WorkItem LinkToOtherWorkItem()
        {
            int sourceWorkItemId = 1;
            int targetWorkItemId = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get work target work item
            WorkItem targetWorkItem = workItemTrackingClient.GetWorkItemAsync(targetWorkItemId).Result;

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

            
            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, sourceWorkItemId).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem UpdateLinkComment()
        {
            int id = 12;

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

        public WorkItem RemoveLink()
        {
            int id = 12;

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
        public WorkItem AddAttachment()
        {
            int id = -1;
            string filePath = null;

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
        public WorkItem RemoveAttachment()
        {
            int id = -1;
            string rev = null;

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
        public WorkItem UpdateWorkItemAddHyperLink()
        {
            int id = -1;
            Uri url = null;
            string urlComment = null;

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
        public WorkItem UpdateWorkItemAddCommitLink()
        {
            int workItemId = 12;
            string commitUri = null; // vstfs:///Git/Commit/1435ac99-ba45-43e7-9c3d-0e879e7f2691%2Fd00dd2d9-55dd-46fc-ad00-706891dfbc48%2F3fefa488aac46898a25464ca96009cf05a6426e3


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
                        url = commitUri,
                        attributes = new { comment = "Fixed in Commit" }
                    }
                }
            );

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, workItemId).Result;

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
        public WorkItemDelete DeleteWorkItem()
        {
            WorkItem workitem;
            if (!Context.TryGetValue<WorkItem>("$newWorkItem", out workitem) || workitem.Id == null)
            {
                Console.WriteLine("Run the create sample before running this.");
            }

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Delete the work item (but don't destroy it completely)
            WorkItemDelete results = workItemTrackingClient.DeleteWorkItemAsync(workitem.Id.Value, destroy: false).Result;

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
