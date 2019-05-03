using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    /// <summary>
    /// 
    /// Samples showing how to work with work item tracking areas and iterations.
    /// 
    /// See https://www.visualstudio.com/docs/integrate/api/wit/classification-nodes for more details.
    /// 
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.ClassificationNodes)]
    public class ClassificationNodesSample : ClientSample
    {
        [ClientSampleMethod]
        public WorkItemClassificationNode ListAreas()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int depth = 2;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode rootAreaNode = workItemTrackingClient.GetClassificationNodeAsync(
                projectName, 
                TreeStructureGroup.Areas, 
                null, 
                depth).Result;

            ShowNodeTree(rootAreaNode);

            return rootAreaNode;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode ListIterations()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            int depth = 4;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode rootIterationNode = workItemTrackingClient.GetClassificationNodeAsync(
                projectName, 
                TreeStructureGroup.Iterations, 
                null, 
                depth).Result;

            ShowNodeTree(rootIterationNode);

            return rootIterationNode;
        }
   
        private void ShowNodeTree(WorkItemClassificationNode node, string path = "")
        {        
            path = path + "/" + node.Name;
            Console.WriteLine(path);

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    ShowNodeTree(child, path);
                }
            }
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode CreateArea()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            String newAreaName = "My new area " + Guid.NewGuid();

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = newAreaName,
                StructureType = TreeNodeStructureType.Area,
                Children = new List<WorkItemClassificationNode>()
                {
                    new WorkItemClassificationNode()
                    {
                        Name = "Child 1"
                    },
                    new WorkItemClassificationNode()
                    {
                        Name = "Child 2"
                    }
                }
            };

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Create the new area
            WorkItemClassificationNode result = workItemTrackingClient.CreateOrUpdateClassificationNodeAsync(
                node,
                projectId.ToString(),
                TreeStructureGroup.Areas).Result;

            // Show the new node
            ShowNodeTree(result);

            // Save the new area for use in a later sample
            Context.SetValue<string>("$newAreaName", node.Name);
            Context.SetValue<Guid>("$newAreaProjectId", projectId);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode CreateIteration()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            string newIterationName = "New iteration " + Guid.NewGuid();

            WorkItemClassificationNode node = new WorkItemClassificationNode() {
                Name = newIterationName,
                StructureType = TreeNodeStructureType.Iteration,
                Attributes = new Dictionary<string, Object>()
                {
                    { "startDate", DateTime.Today },
                    { "finishDate", DateTime.Today.AddDays(7) },
                }
            };

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Create the new iteration
            WorkItemClassificationNode result = workItemTrackingClient.CreateOrUpdateClassificationNodeAsync(
                node, 
                projectId.ToString(),
                TreeStructureGroup.Iterations).Result;

            // Show the new node
            ShowNodeTree(result);

            // Save the new iteration for use in a later sample
            Context.SetValue<string>("$newIterationName", node.Name);
            Context.SetValue<Guid>("$newIterationProjectId", projectId);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode RenameArea()
        {
            Guid projectId;
            string currentName;

            // Get values from previous sample method that created a sample area
            Context.TryGetValue<Guid>("$newAreaProjectId", out projectId);
            Context.TryGetValue<string>("$newAreaName", out currentName);

            string newName = currentName + " (renamed)";

            WorkItemClassificationNode node = new WorkItemClassificationNode() {
                Name = newName,
                StructureType = TreeNodeStructureType.Area
            };

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Update the path of the selected node
            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(
                node,
                projectId.ToString(), 
                TreeStructureGroup.Areas,
                currentName).Result;

            // Save the new name for a later sample
            Context.SetValue<string>("$newAreaName", newName);

            // Show the new node
            ShowNodeTree(result);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode RenameIteration()
        {
            Guid projectId;
            string currentName;

            // Get values from previous sample method that created a sample iteration
            Context.TryGetValue<Guid>("$newIterationProjectId", out projectId);
            Context.TryGetValue<string>("$newIterationName", out currentName);

            string newName = currentName + " (renamed)";

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = newName,
                StructureType = TreeNodeStructureType.Iteration
            };

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(
                node,
                projectId.ToString(), 
                TreeStructureGroup.Iterations,
                currentName).Result;

            // Save the new name for a later sample
            Context.SetValue<string>("$newIterationName", newName);

            // Show the new node
            ShowNodeTree(result);

            return result;
        }

        public WorkItemClassificationNode UpdateIterationDates()
        {
            Guid projectId;
            string iterationName;

            // Get values from previous sample method that created a sample iteration
            Context.TryGetValue<Guid>("$newIterationProjectId", out projectId);
            Context.TryGetValue<string>("$newIterationName", out iterationName);

            DateTime newStartDate = DateTime.Today.AddDays(7);
            DateTime newFinishDate = DateTime.Today.AddDays(21);

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                StructureType = TreeNodeStructureType.Iteration,
                Attributes = new Dictionary<string, Object>()
                {
                    { "startDate", newStartDate },
                    { "finishDate", newFinishDate }
                }
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(
                node,
                projectId.ToString(),
                TreeStructureGroup.Iterations,
                iterationName).Result;

            // Show the new node
            ShowNodeTree(result);

            return result;
        }


        [ClientSampleMethod]
        public WorkItemClassificationNode GetArea()
        {
            Guid projectId;
            string areaPath;

            // Get values from previous sample method that created a sample iteration
            Context.TryGetValue<Guid>("$newAreaProjectId", out projectId);
            Context.TryGetValue<string>("$newAreaName", out areaPath);

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(
                projectId.ToString(),
                TreeStructureGroup.Areas,
                areaPath,
                5).Result;

            // Show the new node
            ShowNodeTree(result);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode GetIteration()
        {
            Guid projectId;
            string iterationPath;

            // Get values from previous sample method that created a sample iteration
            Context.TryGetValue<Guid>("$newIterationProjectId", out projectId);
            Context.TryGetValue<string>("$newIterationName", out iterationPath);

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(
                projectId.ToString(), 
                TreeStructureGroup.Iterations,
                iterationPath,
                4).Result;

            // Show the new node
            ShowNodeTree(result);

            return result;
        }

        public WorkItemClassificationNode MoveArea()
        {
            string project = "TBD";
            string targetArea = "TBD";
            int areaId = -1;

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Id = areaId,
                StructureType = TreeNodeStructureType.Area
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(
                node, 
                project,
                TreeStructureGroup.Areas, 
                targetArea).Result;

            return result;
        }

        public bool DeleteArea()
        {
            string project = "TBD";
            string areaPath = "TBD";
            int? reclassifyId = null;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                workItemTrackingClient.DeleteClassificationNodeAsync(
                    project, 
                    TreeStructureGroup.Areas,
                    areaPath, 
                    reclassifyId).SyncResult();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
    
        }

        public bool DeleteIteration()
        {
            string project = "TBD";
            string iterationPath = "TBD";
            int? reclassifyId = null;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                workItemTrackingClient.DeleteClassificationNodeAsync(
                    project, 
                    TreeStructureGroup.Iterations, 
                    iterationPath, 
                    reclassifyId).SyncResult();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [ClientSampleMethod]
        public List<string> GetFullTree(string project, TreeStructureGroup type)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode rootNode = workItemTrackingClient.GetClassificationNodeAsync(project, type, null, 1000).Result;

            List<string> paths = new List<string>();

            ShowNodeTree(rootNode);

            return paths;
        }
    }  
}
