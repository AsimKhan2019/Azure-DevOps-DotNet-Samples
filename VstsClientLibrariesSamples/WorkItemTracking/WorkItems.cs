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
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids).Result;
                return results;
            }
        }

        public List<WorkItem> GetWorkItemsWithSpecificFields(IEnumerable<int> ids)
        {
           var fields = new string[] {
               "System.Id",
               "System.Title",
               "System.WorkItemType",
               "Microsoft.VSTS.Scheduling.RemainingWork"
           };    

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields).Result;
                return results;
            }
        }

        public List<WorkItem> GetWorkItemsAsOfDate(IEnumerable<int> ids, DateTime asOfDate)
        {
            var fields = new string[] {
               "System.Id",
               "System.Title",
               "System.WorkItemType",
               "Microsoft.VSTS.Scheduling.RemainingWork"
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids, fields, asOfDate).Result;
                return results;
            }
        }

        public List<WorkItem> GetWorkItemsWithLinksAndAttachments(IEnumerable<int> ids)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                List<WorkItem> results = workItemTrackingHttpClient.GetWorkItemsAsync(ids, null, null, WorkItemExpand.All).Result;
                return results;
            }
        }

        public WorkItem GetWorkItem(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {               
                WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id).Result;
                return result;                                      
            }
        }

        public WorkItem GetWorkItemWithLinksAndAttachments(int id)
        {
            using(WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id, null, null, WorkItemExpand.Relations).Result;
                return result;
            }
        }

        public WorkItem GetWorkItemFullyExpanded(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.GetWorkItemAsync(id, null, null, WorkItemExpand.All).Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Task").Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Task").Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.CreateWorkItemAsync(patchDocument, projectName, "Task", null, true).Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemMoveWorkItem(int id, string teamProject, string areaPath, string iterationPath)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.TeamProject",
                    Value = teamProject
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.AreaPath",
                    Value = areaPath
                }
            );

            patchDocument.Add(
               new JsonPatchOperation()
               {
                   Operation = Operation.Add,
                   Path = "/fields/System.IterationPath",
                   Value = iterationPath
               }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.History",
                    Value = "moving work item to new project"
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemChangeWorkItemType(int id)
        {
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemAddLink(int id, string linkToId)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/relations/-",
                    Value = new
                    {
                        rel = "System.LinkTypes.Dependency-forward",
                        url = _configuration.UriString + "/_apis/wit/workItems/" + linkToId.ToString(),
                        attributes = new { comment = "Making a new link for the dependency" }
                    }
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemUpdateLink(int id, string url)
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
                        rel = "System.LinkTypes.Dependency-forward",
                        url = url,
                        attributes = new { comment = "Making a new link for the dependency" }
                    }
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemRemoveLink(int id)
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
                    Operation = Operation.Remove,
                    Path = "/relations/0"
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemAddAttachment(int id, string filePath)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id).Result;
                return result;
            }
        }

        public WorkItem UpdateWorkItemUsingByPassRules(int id)
        {
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.CreatedBy",
                    Value = "Foo <Foo@hotmail.com>"
                }
            );

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItem result = workItemTrackingHttpClient.UpdateWorkItemAsync(patchDocument, id, null, true).Result;
                return result;
            }
        }

        public WorkItemDelete DeleteWorkItem(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemDelete results = workItemTrackingHttpClient.DeleteWorkItemAsync(id, false).Result;
                return results;
            }
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
 
    }
}
