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
        public List<WorkItemDeleteReference> GetDeletedWorkItems()
        {
            string project = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteReference> results = workItemTrackingClient.GetDeletedWorkItemsAsync(project, null).Result;

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
        public List<WorkItemDeleteReference> GetMultipledDeletedWorkItems()
        {
            int[] ids = { 72, 73, 81 }; //TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteReference> result = workItemTrackingClient.GetDeletedWorkItemsAsync(ids).Result;
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
        public void RestoreMultipleWorkItems()
        {
            int[] ids = { 72, 73, 81 }; //TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteReference> result = workItemTrackingClient.GetDeletedWorkItemsAsync(ids).Result;

            WorkItemDeleteUpdate updateParameters = new WorkItemDeleteUpdate() {
                IsDeleted = false
            };

            foreach (var item in result)
            {
                var restore = workItemTrackingClient.RestoreWorkItemAsync(updateParameters, Convert.ToInt32(item.Id)).Result;               
            }
        }

        [ClientSampleMethod]
        public void PermenentlyDeleteWorkItem()
        {
            int workItemId = -1; // TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            workItemTrackingClient.DestroyWorkItemAsync(workItemId);
        }

        [ClientSampleMethod]
        public void PermenentlyDeleteMultipleWorkItems()
        {
            int[] ids = { 72, 73, 81 }; //TODO

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteReference> result = workItemTrackingClient.GetDeletedWorkItemsAsync(ids).Result;      

            foreach(var item in result)
            {
                workItemTrackingClient.DestroyWorkItemAsync(Convert.ToInt32(item.Id));
            }            
        }
    }
}
