using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    /// <summary>
    /// Client samples for managing work items in Team Services and Team Foundation Server.
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, "workitemssamples")]
    public class WorkItemsSample : ClientSample
    {
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
                    Value = "Sample task 1"
                }
            );            

            //patchDocument.Add(
            //    new JsonPatchOperation()
            //    {
            //        Operation = Operation.Add,
            //        Path = "/fields/System.IterationPath",
            //        Value = "Test Project\\Iteration 1"
            //    }
            //);

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get the project to create the sample work item in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Create the new work item
            WorkItem newWorkItem = workItemTrackingClient.CreateWorkItemAsync(patchDocument, project.Id, "Task").Result;

            Console.WriteLine("Created work item ID {0} {1}", newWorkItem.Id, newWorkItem.Fields["System.Title"]);

            // Save this newly created for later samples
            Context.SetValue<WorkItem>("$newWorkItem", newWorkItem);

            return newWorkItem;
        }

        [ClientSampleMethod]
        public WorkItem CreateWorkItem(string title, string type)
        {
            // Construct the object containing field values required for the new work item
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = title
                }
            );

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get the project to create the sample work item in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Create the new work item
            WorkItem newWorkItem = workItemTrackingClient.CreateWorkItemAsync(patchDocument, project.Id, type).Result;

            Console.WriteLine("Created work item ID {0} {1}", newWorkItem.Id, newWorkItem.Fields["System.Title"]);
            
            return newWorkItem;
        }

        [ClientSampleMethod]
        public void CreateSampleWorkItemData()
        {
            WorkItem newWorkItem;

            newWorkItem = this.CreateWorkItem("Sample Task #1", "Task");           
            Context.SetValue<WorkItem>("$newWorkItem1", newWorkItem);

            newWorkItem = this.CreateWorkItem("Sample Task #2", "Task");
            Context.SetValue<WorkItem>("$newWorkItem2", newWorkItem);

            newWorkItem = this.CreateWorkItem("Sample Task #3", "Task");
            Context.SetValue<WorkItem>("$newWorkItem3", newWorkItem);
        }   
              
        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsByIDs()
        {
            Context.SetValue<int[]>("$workitemIds", new int[] { Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id), Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id), Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem3").Id) });
            
            int[] workitemIds = Context.GetValue<int[]>("$workitemIds");

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
        public WorkItemTemplate CreateTemplate()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();

            Dictionary<string, string> field = new Dictionary<string, string>
            {
                { "System.State", "New" }
            };

            WorkItemTemplate newTemplate = new WorkItemTemplate()
            {
                Name = "Test Template",
                Description = "Template to be created",
                WorkItemTypeName = "Feature",
                Fields = field
            };
            WorkItemTemplate result = null;

            try
            {
                result = client.CreateTemplateAsync(newTemplate, getTeamContext()).Result;
                Context.SetValue<Guid>("$newTemplateId", result.Id);
                Console.WriteLine("Create template Successed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Create template Failed:" + e.Message);
            }
            return result;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetWorkItemsWithSpecificFields()
        {
            int[] workitemIds = Context.GetValue<int[]>("$workitemIds");

            string[] fieldNames = new string[] {
                "System.Id",
                "System.Title",
                "System.WorkItemType"               
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
            int[] workitemIds = Context.GetValue<int[]>("$workitemIds");

            string[] fieldNames = new string[] {
               "System.Id",
               "System.Title",
               "System.WorkItemType"               
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
            int[] workitemIds = Context.GetValue<int[]>("$workitemIds");

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> workitems = workItemTrackingClient.GetWorkItemsAsync(workitemIds, expand: WorkItemExpand.Links | WorkItemExpand.Relations).Result;
            
            foreach(var workitem in workitems)
            {
                Console.WriteLine("Work item {0}", workitem.Id);

                if (workitem.Relations == null)
                {
                    Console.WriteLine("  No relations found on this work item");
                }
                else { 
                    foreach (var relation in workitem.Relations)
                    {
                        Console.WriteLine("  {0} {1}", relation.Rel, relation.Url);
                    }
                }
            }

            return workitems;
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItemById()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);

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
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);

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

            if (workitem.Relations == null)
            {
                Console.WriteLine("  No relations found for this work item");
            }
            else
            {
                foreach (var relation in workitem.Relations)
                {
                    Console.WriteLine("  {0} {1}", relation.Rel, relation.Url);
                }
            }
            

            return workitem;
        }
        
        [ClientSampleMethod]
        public WorkItem CreateAndLinkToWorkItem()
        {
            string title = "My new work item with links";
            string description = "This is a new work item that has a link also created on it.";
            string linkUrl = Context.GetValue<WorkItem>("$newWorkItem1").Url; //get the url of a previously added work item
                        
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
        public WorkItem UpdateValidateOnly()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem").Id);

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
                    Path = "/fields/System.Title",
                    Value = "Hello World"
                }
            );          

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            //set validateOnly param == true. This will only validate the work item. it will not attempt to save it.
            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id, true).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem ChangeFieldValue()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem").Id);

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
                    Path = "/fields/System.Title",
                    Value = "This is the new title for my work item"
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
        public WorkItem AddTags()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);
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
            int sourceWorkItemId = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);
            int targetWorkItemId = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);

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

        /// <summary>
        /// get a list of the child work items for a specific parent
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public List<WorkItem> GetChildWorkItemsByParent()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem workitem = workItemTrackingClient.GetWorkItemAsync(id, expand: WorkItemExpand.Relations).Result;
            List<WorkItem> workitems = null;

            if (workitem.Relations == null)
            {
                Console.WriteLine("  No relations found for this work item");
            }
            else
            {
                List<int> list = new List<int>();

                Console.WriteLine("Getting child work items from parent...");

                foreach (var relation in workitem.Relations)
                {
                    //get the child links
                    if (relation.Rel == "System.LinkTypes.Hierarchy-Forward")
                    {
                        var lastIndex = relation.Url.LastIndexOf("/");
                        var itemId = relation.Url.Substring(lastIndex + 1);
                        list.Add(Convert.ToInt32(itemId));

                        Console.WriteLine("  {0} ", itemId);
                    };
                }

                int[] workitemIds = list.ToArray();

                workitems = workItemTrackingClient.GetWorkItemsAsync(workitemIds).Result;

                Console.WriteLine("Getting full work item for each child...");

                foreach (var item in workitems)
                {
                    Console.WriteLine(" {0}: {1} : {2}", item.Fields["System.WorkItemType"], item.Id, item.Fields["System.Title"]);
                }
            }

            return workitems;
        }

        [ClientSampleMethod]
        public WorkItem UpdateLinkComment()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);

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
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);

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
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem3").Id);
            string filePath = ClientSampleHelpers.GetSampleTextFile();

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
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem3").Id);
            string rev = "0";

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
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);            

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
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "Hyperlink",
                        url = "https://docs.microsoft.com/en-us/rest/api/vsts/"                        
                    }
                }
            );

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }      

        [ClientSampleMethod]
        public WorkItem UpdateWorkItemUsingByPassRules()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);

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
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Delete the work item (but don't destroy it completely)
            WorkItemDelete results = workItemTrackingClient.DeleteWorkItemAsync(id, destroy: false).Result;

            return results;
        }
        
        public WorkItem MoveToAnotherProject()
        {
            int id = -1;
            string targetProject = null;
            string targetAreaPath = null;
            string targetIterationPath = null;

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.TeamProject",
                    Value = targetProject
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.AreaPath",
                    Value = targetAreaPath
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
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

        public WorkItem ChangeType()
        {
            WorkItem newWorkItem;
            using (new ClientSampleHttpLoggerOutputSuppression())
            {                
                newWorkItem = this.CreateWorkItem("Another Sample Work Item", "Task");
            }

            int id = Convert.ToInt32(newWorkItem.Id);

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.WorkItemType",
                    Value = "User Story"
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
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

        public WorkItem UpdateWorkItemLinkToPullRequest()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);
            
            VssConnection connection = Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            //you will need to edit this line and enter a legit pull request id
            var pullRequest = gitClient.GetPullRequestByIdAsync(999).Result;
            var pullRequestUri = pullRequest.ArtifactId;

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Test,
                   Path = "/rev",
                   Value = "3"
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
                        url = pullRequestUri,
                        attributes = new { 
                            comment = "Fixed in Commit",
                            name = "pull request"
                        }
                    }
                }
            );

            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

        public WorkItem UpdateWorkItemLinkToCommit()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem2").Id);
            System.Guid repositoryId = new Guid("2f3d611a-f012-4b39-b157-8db63f380226");
            string commitId = "be67f8871a4d2c75f13a51c1d3c30ac0d74d4ef4";

            VssConnection connection = Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            //you will need to edit this line and enter a legit repo id and commit id
            //these are samples and will not work
            var commit = gitClient.GetCommitAsync(commitId, repositoryId).Result;
            var commitUri = commit.Url;

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Test,
                   Path = "/rev",
                   Value = "3"
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
                        attributes = new
                        {
                            comment = "Fixed in Commit",
                            name = "commit"
                        }
                    }
                }
            );


            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem result = workItemTrackingClient.UpdateWorkItemAsync(patchDocument, id).Result;

            return result;
        }

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

        [ClientSampleMethod]
        public WorkItem GetWorkItemTemplate()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();
            
            // Get the project to create the sample work item in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            WorkItem result = client.GetWorkItemTemplateAsync(project.Name, "Bug").Result;

            return result;
        }

        [ClientSampleMethod]
        public List<WorkItemTemplateReference> ListTemplates()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemTemplateReference> result = client.GetTemplatesAsync(getTeamContext()).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItem GetTemplate()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            WorkItem result = client.GetWorkItemTemplateAsync(project.Name, "Feature").Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemTemplate ReplaceTemplate()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();

            Dictionary<string, string> field = new Dictionary<string, string>
            {
                { "System.State", "Replaced" }
            };

            WorkItemTemplate newTemplate = new WorkItemTemplate()
            {
                Name = "New Test Template",
                Description = "Replacing template",
                WorkItemTypeName = "Feature",
                Fields = field
            };
            WorkItemTemplate result = null;

            try
            {
                result = client.ReplaceTemplateAsync(newTemplate, getTeamContext(), Context.GetValue<Guid>("$newTemplateId")).Result;
                Console.WriteLine("Replace template successed.");
            }
            catch(Exception e)
            {
                result = null;
                Console.WriteLine("Replace template failed: " + e.Message);
            }
            return result;
        }

        [ClientSampleMethod]
        public WorkItem UpdateWorkItemBoardColumn()
        {
            string targetColumn = "Testing"; //need to set to match your board

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            JsonPatchDocument patchDocument = new JsonPatchDocument();

            //create a work item that drops into the new column by default
            WorkItem workItem = this.CreateWorkItem("Board Column Test", "User Story");
                       
            string wefField = "";

            //find the WEF field
            //todo: do something smarter rather than loop through all fields to find the WEF
            foreach (var field in workItem.Fields)
            {               
                if (field.Key.Contains("_Kanban.Column"))
                {
                    wefField = field.Key.ToString();
                    break;
                }
            }

            //build a patch document to update the WEF field
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
                     Path = "/fields/" + wefField,
                     Value = targetColumn
                 }
            );

            WorkItem result = witClient.UpdateWorkItemAsync(patchDocument, Convert.ToInt32(workItem.Id)).Result;

            Context.Log("Updated work item to teams board column '" + targetColumn + "'");

            return result;
        }

        [ClientSampleMethod]
        public void DeleteTemplate()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                client.DeleteTemplateAsync(getTeamContext(), Context.GetValue<Guid>("$newTemplateId"));
                Console.WriteLine("Delete template successed.");
            }
            catch
            {
                Console.WriteLine("Delete template Failded.");
            }
        }

        private TeamContext getTeamContext()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            WebApiTeamRef team = ClientSampleHelpers.FindAnyTeam(this.Context, project.Id);
            TeamContext teamContext = new TeamContext(project.Name, team.Name);
            return teamContext;
        }
    }
}
