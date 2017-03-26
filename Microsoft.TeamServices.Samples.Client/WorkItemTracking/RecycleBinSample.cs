using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.WorkItems)]
    public class RecycleBinSample : ClientSample
    {

        [ClientSampleMethod]
        public List<WorkItemDeleteShallowReference> GetDeletedWorkItems()
        {
            string project = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteShallowReference> results = workItemTrackingClient.GetDeletedWorkItemsAsync(project).Result;

            return results;
        }

        [ClientSampleMethod]
        public WorkItemDelete GetDeletedWorkItem()
        {
            int workItemId = -1; // TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete result = workItemTrackingClient.GetDeletedWorkItemAsync(workItemId).Result;

            return result;
        }

        [ClientSampleMethod]
        public WorkItemDelete RestoreWorkItem()
        {
            int workItemId = -1; // TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDeleteUpdate updateParameters = new WorkItemDeleteUpdate() {
                IsDeleted = false
            };

            WorkItemDelete result = workItemTrackingClient.RestoreWorkItemAsync(updateParameters, workItemId).Result;

            return result;
        }

        [ClientSampleMethod]
        public void PermenentlyDeleteWorkItem()
        {
            int workItemId = -1; // TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            workItemTrackingClient.DestroyWorkItemAsync(workItemId);
        }       
    }
}
