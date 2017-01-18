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
    public class RecycleBin
    {
        private readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public RecycleBin(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public List<WorkItemDeleteReference> GetDeletedItems(string project, int[] ids)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                List<WorkItemDeleteReference> results = workItemTrackingHttpClient.GetDeletedWorkItemsAsync(project, ids).Result;
                return results;
            }
        }

        public WorkItemDelete GetDeletedItem(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemDelete result = workItemTrackingHttpClient.GetDeletedWorkItemAsync(id).Result;
                return result;
            }
        }

        public WorkItemDelete RestoreItem(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                WorkItemDeleteUpdate payload = new WorkItemDeleteUpdate() {
                    IsDeleted = false
                };

                WorkItemDelete result = workItemTrackingHttpClient.RestoreWorkItemAsync(payload, id).Result;
                return result;
            }
        }
        
        public void PermenentlyDeleteItem(int id)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                workItemTrackingHttpClient.DestroyWorkItemAsync(id);               
            }
        }       
    }
}
