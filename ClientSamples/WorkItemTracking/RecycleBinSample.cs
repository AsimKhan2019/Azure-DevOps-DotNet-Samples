using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace VstsSamples.Client.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.WorkItems)]
    public class RecycleBinSample : ClientSample
    {

        public RecycleBinSample(ClientSampleConfiguration configuration) : base(configuration)
        {
        }

        [ClientSampleMethod]
        public List<WorkItemDeleteShallowReference> GetDeletedItems(string project)
        {
            VssConnection connection = this.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteShallowReference> results = workItemTrackingClient.GetDeletedWorkItemsAsync(project).Result;

            return results;
        }

        [ClientSampleMethod]
        public WorkItemDelete GetDeletedItem(int workItemId)
        {
            VssConnection connection = this.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete result = workItemTrackingClient.GetDeletedWorkItemAsync(workItemId).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemDelete RestoreItem(int workItemId)
        {
            VssConnection connection = this.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDeleteUpdate updateParameters = new WorkItemDeleteUpdate() {
                IsDeleted = false
            };

            WorkItemDelete result = workItemTrackingClient.RestoreWorkItemAsync(updateParameters, workItemId).Result;

            return result;
        }

        [ClientSampleMethod]
        public void PermenentlyDeleteItem(int workItemId)
        {
            VssConnection connection = this.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            workItemTrackingClient.DestroyWorkItemAsync(workItemId);
        }       
    }
}
