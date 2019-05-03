using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, "workitemsrecyclebin")]
    public class RecycleBinSample : ClientSample
    {
        int _id;
        int[] _ids;

        [ClientSampleMethod]
        public void CreateSampleData()
        {           
            WorkItem workItem1;
            WorkItem workItem2;
            WorkItem workItem3;
            WorkItem newWorkItem;

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                WorkItemsSample witSample = new WorkItemsSample();
                witSample.Context = this.Context;
                newWorkItem = witSample.CreateWorkItem("Sample work item for comments", "Task");    
                
                _id = Convert.ToInt32(newWorkItem.Id);

                workItem1 = witSample.CreateWorkItem("Sample work item for comments #1", "Task");
                workItem2 = witSample.CreateWorkItem("Sample work item for comments #2", "Task");
                workItem3 = witSample.CreateWorkItem("Sample work item for comments #3", "Task");

                _ids = new int[] { Convert.ToInt32(workItem1.Id), Convert.ToInt32(workItem2.Id), Convert.ToInt32(workItem3.Id) };
            }
        }

        [ClientSampleMethod]
        public WorkItemDelete DeleteWorkItems()
        {
            int id = _id;
            int[] ids = _ids;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete result = workItemTrackingClient.DeleteWorkItemAsync(id).Result;
            result = workItemTrackingClient.DeleteWorkItemAsync(ids[0]).Result;
            result = workItemTrackingClient.DeleteWorkItemAsync(ids[1]).Result;
            result = workItemTrackingClient.DeleteWorkItemAsync(ids[2]).Result;

            return result;
        }

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
            int id = _id;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete result = workItemTrackingClient.GetDeletedWorkItemAsync(id).Result;

            return result;
        }

        [ClientSampleMethod]
        public List<WorkItemDeleteReference> GetMultipledDeletedWorkItems()
        {
            int[] ids = _ids;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteReference> result = workItemTrackingClient.GetDeletedWorkItemsAsync(ids).Result;
            return result;
        }

        [ClientSampleMethod]
        public WorkItemDelete RestoreWorkItem()
        {
            int id = _id;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDeleteUpdate updateParameters = new WorkItemDeleteUpdate() {
                IsDeleted = false
            };

            WorkItemDelete result = workItemTrackingClient.RestoreWorkItemAsync(updateParameters, id).Result;

            return result;
        }

        [ClientSampleMethod]
        public void RestoreMultipleWorkItems()
        {
            int[] ids = _ids;

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
        public void PermanentlyDeleteWorkItem()
        {
            int id = _id;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete result = workItemTrackingClient.DeleteWorkItemAsync(id).Result;           

            workItemTrackingClient.DestroyWorkItemAsync(id);
        }

        [ClientSampleMethod]
        public void PermanentlyDeleteMultipleWorkItems()
        {
            int[] ids = _ids;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemDelete delResult;
            delResult = workItemTrackingClient.DeleteWorkItemAsync(ids[0]).Result;
            delResult = workItemTrackingClient.DeleteWorkItemAsync(ids[1]).Result;
            delResult = workItemTrackingClient.DeleteWorkItemAsync(ids[2]).Result;

            List<WorkItemDeleteReference> result = workItemTrackingClient.GetDeletedWorkItemsAsync(ids).Result;      

            foreach(var item in result)
            {
                workItemTrackingClient.DestroyWorkItemAsync(Convert.ToInt32(item.Id));
            }            
        }
    }
}
