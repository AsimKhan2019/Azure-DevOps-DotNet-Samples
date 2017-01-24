using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class ClassificationNodesSamples
    {
        private readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public ClassificationNodesSamples(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public List<string> GetFullTree(string project, TreeStructureGroup type)
        {
            List<string> list = new List<string>();

            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemClassificationNode result = workItemTrackingHttpClient.GetClassificationNodeAsync(project, type, null, 1000).Result;

                list.Add(result.Name);               
                
                foreach (var item in result.Children)
                {
                    var name = result.Name + "/" + item.Name;

                    list.Add(name);                   
                    walkTreeNode(item, list, name);
                }

                return list;
            }
        }

        public void walkTreeNode(WorkItemClassificationNode t, List<string> list, string node)
        {
            if (t.Children != null)
            {
                foreach (WorkItemClassificationNode child in t.Children)
                {
                    list.Add(node + "/" + child.Name);
                    walkTreeNode(child, list, node + "/" + child.Name);
                }
            }
        }
    }
}
