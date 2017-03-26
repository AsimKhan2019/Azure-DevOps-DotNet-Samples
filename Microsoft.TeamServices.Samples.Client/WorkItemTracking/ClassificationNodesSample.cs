using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Vsts.ClientSamples.WorkItemTracking
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
            int depth = -1;

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
            foreach (var child in node.Children)
            {
                ShowNodeTree(child, path);
            }
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode GetArea()
        {
            string project = null;
            string path = null;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, path, 0).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode GetIteration()
        {
            string project = null;
            string path = null;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(project, TreeStructureGroup.Iterations, path, 0).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode CreateArea()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            String areaName = "My new area";

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = areaName,
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

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.CreateOrUpdateClassificationNodeAsync(
                node,
                projectId.ToString(),
                TreeStructureGroup.Areas).Result;

            ShowNodeTree(result);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode CreateIteration()
        {
            //IDictionary<string, Object> dict = new Dictionary<string, Object>();

            //dict.Add("startDate", startDate);
            //dict.Add("finishDate", finishDate);

            string project = null;
            string name = null;

            WorkItemClassificationNode node = new WorkItemClassificationNode() {
                Name = name,
                StructureType = TreeNodeStructureType.Iteration
                //Attributes = dict
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.CreateOrUpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, "").Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode RenameArea()
        {
            string project = null;
            string path = null;
            string name = null;

            WorkItemClassificationNode node = new WorkItemClassificationNode() {
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(
                node, 
                project, 
                TreeStructureGroup.Areas, 
                path).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode RenameIteration()
        {
            string project = null;
            string path = null;
            string name = null;

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = name,
                StructureType = TreeNodeStructureType.Iteration
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, path).Result;

            return result;
        }

        public WorkItemClassificationNode UpdateIterationDates()
        {
            string project = null;
            string name = null;
            DateTime startDate;
            DateTime finishDate;

            IDictionary<string, Object> dict = new Dictionary<string, Object>();

            //dict.Add("startDate", startDate);
            //dict.Add("finishDate", finishDate);

            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                StructureType = TreeNodeStructureType.Iteration,
                Attributes = dict
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, name).Result;

            return result;
        }

        public WorkItemClassificationNode MoveArea(string project, string targetArea, int id)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Id = id,
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

        public WorkItemClassificationNode MoveIteration(string project, string targetIteration, int id)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Id = id,
                StructureType = TreeNodeStructureType.Iteration
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, targetIteration).Result;

            return result;
        }

        [ClientSampleMethod]
        public bool DeleteArea(string project, string areaPath, int reclassifyId)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                workItemTrackingClient.DeleteClassificationNodeAsync(project, TreeStructureGroup.Areas, areaPath, reclassifyId).SyncResult();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
    
        }

        [ClientSampleMethod]
        public bool DeleteIteration(string project, string iterationPath, int reclassifyId)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                workItemTrackingClient.DeleteClassificationNodeAsync(project, TreeStructureGroup.Iterations, iterationPath, reclassifyId).SyncResult();

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
