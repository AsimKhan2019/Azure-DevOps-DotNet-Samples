using System;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

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
       
        public string GetAreas(string project, int depth)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, null, depth).Result;
            }

            return "success";
        }

        public string GetArea(string project, string path)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                try
                {
                    WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, TreeStructureGroup.Areas, path, 0).Result;
                }
                catch (System.AggregateException ex)
                {
                    return ex.InnerException.ToString();
                }                
            }

            return "success";
        }

        public string CreateArea(string project, string path, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.CreateOrUpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, path).Result;
            }

            return "success";
        }

        public string UpdateArea(string project, string path, string name)
        {
            WorkItemClassificationNode node = new WorkItemClassificationNode()
            {                      
                Name = name,
                StructureType = TreeNodeStructureType.Area
            };

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                try
                {
                    WorkItemClassificationNode result = workItemTrackingHttpClient.UpdateClassificationNodeAsync(node, project, TreeStructureGroup.Areas, path).Result;
                }
                catch (System.AggregateException ex)
                {
                    return ex.InnerException.ToString();
                }

                return "success";
               
            }

            return "success";
        }
       
    }
}
