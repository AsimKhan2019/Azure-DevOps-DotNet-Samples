using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.WebApi;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class ClassificationNodes
    {
        private readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public ClassificationNodes(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }
       
        public WorkItemClassificationNode GetAreas(string project, int depth)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, null, depth).Result;
                return result;
            }
            
        }
        
        public WorkItemClassificationNode GetIterations(string project, int depth)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, TreeStructureGroup.Iterations, null, depth).Result;
                return result;
            }         
        }
        
        public WorkItemClassificationNode GetArea(string project, string path)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {                
                WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, path, 0).Result;
                return result;
            }            
        }
        
        public WorkItemClassificationNode GetIteration(string project, string path)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {               
                WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, TreeStructureGroup.Iterations, path, 0).Result;
                return result;               
            }
        }

        public WorkItemClassificationNode CreateArea(string project, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {                
                WorkItemClassificationNode result = workItemTrackingHttpClient.CreateOrUpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, "").Result;
                return result;                
            }           
        }
        
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {               
                WorkItemClassificationNode result = workItemTrackingHttpClient.CreateOrUpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, "").Result;
                return result;               
            }
        }

        public WorkItemClassificationNode RenameArea(string project, string path, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode() {
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, path).Result;
                return result;
            }
        }
        
        public WorkItemClassificationNode RenameIteration(string project, string path, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = name,
                StructureType = TreeNodeStructureType.Iteration
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {               
                WorkItemClassificationNode result = workItemTrackingHttpClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, path).Result;
                return result;
            }        
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

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {               
                WorkItemClassificationNode result = workItemTrackingHttpClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, name).Result;
                return result;                               
            }           
        }

        public WorkItemClassificationNode MoveArea(string project, string targetArea, int id)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Id = id,
                StructureType = TreeNodeStructureType.Area
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, targetArea).Result;
                return result;
            }
        }
        
        public WorkItemClassificationNode MoveIteration(string project, string targetIteration, int id)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {                
                Id = id,
                StructureType = TreeNodeStructureType.Iteration
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Iterations, targetIteration).Result;
                return result;
            }
        }

        public void DeleteArea(string project, string areaPath, int reclassifyId)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                workItemTrackingHttpClient.DeleteClassificationNodeAsync(project, TreeStructureGroup.Areas, areaPath, reclassifyId).SyncResult();
            }
        }

        public void DeleteIteration(string project, string iterationPath, int reclassifyId)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                workItemTrackingHttpClient.DeleteClassificationNodeAsync(project, TreeStructureGroup.Iterations, iterationPath, reclassifyId).SyncResult();
            }
        }
    }
}
