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
        public WorkItemClassificationNode GetAreas(string project, int depth)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, null, depth).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode GetIterations(string project, int depth)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(project, TreeStructureGroup.Iterations, null, depth).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode GetArea(string project, string path)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, path, 0).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode GetIteration(string project, string path)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.GetClassificationNodeAsync(project, TreeStructureGroup.Iterations, path, 0).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode CreateArea(string project, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.CreateOrUpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, "").Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode CreateIteration(string project, string name)
        {
            //IDictionary<string, Object> dict = new Dictionary<string, Object>();

            //dict.Add("startDate", startDate);
            //dict.Add("finishDate", finishDate);

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
        public WorkItemClassificationNode RenameArea(string project, string path, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode() {
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, path).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemClassificationNode RenameIteration(string project, string path, string name)
        {
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

        public WorkItemClassificationNode UpdateIterationDates(string project, string name, DateTime startDate, DateTime finishDate)
        {
            IDictionary<string, Object> dict = new Dictionary<string, Object>();

            dict.Add("startDate", startDate);
            dict.Add("finishDate", finishDate);

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

            WorkItemClassificationNode result = workItemTrackingClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, targetArea).Result;

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

            WalkTreeNode(rootNode, "", paths);

            return paths;
        }

        private void WalkTreeNode(WorkItemClassificationNode node, string nodeParentPath, List<string> paths)
        {
            paths.Add(nodeParentPath + node.Name);

            if (node.Children != null)
            {
                foreach (WorkItemClassificationNode childNode in node.Children)
                {
                    WalkTreeNode(childNode, nodeParentPath + node.Name + "/", paths);
                }
            }
        }
    }  
}
