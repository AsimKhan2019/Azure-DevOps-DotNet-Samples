using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

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

        public List<WorkItemDeleteReference> GetDeletedItems(string project)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            List<WorkItemDeleteReference> results = workItemTrackingHttpClient.GetDeletedWorkItemsAsync(project).Result;
            return results;
        }

        public WorkItemDelete GetDeletedItem(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemDelete result = workItemTrackingHttpClient.GetDeletedWorkItemAsync(id).Result;
            return result;
        }

        public WorkItemDelete RestoreItem(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDeleteUpdate payload = new WorkItemDeleteUpdate() {
                IsDeleted = false
            };

            WorkItemDelete result = workItemTrackingHttpClient.RestoreWorkItemAsync(payload, id).Result;
            return result;
        }
        
        public void PermenentlyDeleteItem(int id)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            workItemTrackingHttpClient.DestroyWorkItemAsync(id);
        }       
    }
}
