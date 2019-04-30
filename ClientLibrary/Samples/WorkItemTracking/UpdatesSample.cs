using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    /// <summary>
    /// Client samples for managing work items in Team Services and Team Foundation Server.
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.Updates)]
    public class UpdatesSample : ClientSample
    {
        [ClientSampleMethod]
        public List<WorkItemUpdate> GetListOfWorkItemUpdates()
        {
            int id = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemUpdate> updates = workItemTrackingClient.GetUpdatesAsync(id).Result;

            Console.WriteLine("Work Item Updates...");

            foreach (var item in updates)
            {
                Console.WriteLine("Id:           {0}", item.Id);
                Console.WriteLine("Revision:     {0}", item.Rev);
                Console.WriteLine("Revised By:   {0}", item.RevisedBy.Name);
                Console.WriteLine("Revised Date: {0}", item.RevisedDate);
                Console.WriteLine();
            }

            return updates;
        }

        [ClientSampleMethod]
        public List<WorkItemUpdate> GetListOfWorkItemUpdatesPaged()
        {
            int id = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            //skip revision 1 and give me the next 2
            List<WorkItemUpdate> updates = workItemTrackingClient.GetUpdatesAsync(id, 2, 1).Result;

            Console.WriteLine("Work Item Updates...");

            foreach (var item in updates)
            {
                Console.WriteLine("Id:           {0}", item.Id);
                Console.WriteLine("Revision:     {0}", item.Rev);
                Console.WriteLine("Revised By:   {0}", item.RevisedBy.Name);
                Console.WriteLine("Revised Date: {0}", item.RevisedDate);
                Console.WriteLine();
            }

            return updates;
        }

        [ClientSampleMethod]
        public WorkItemUpdate GetWorkItemUpdate()
        {
            int id = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            //skip revision 1 and give me the next 2
            WorkItemUpdate update = workItemTrackingClient.GetUpdateAsync(id, 1).Result;

            Console.WriteLine("Work Item Update...");
                       
            Console.WriteLine("Id:           {0}", update.Id);
            Console.WriteLine("Revision:     {0}", update.Rev);
            Console.WriteLine("Revised By:   {0}", update.RevisedBy.Name);
            Console.WriteLine("Revised Date: {0}", update.RevisedDate);
            Console.WriteLine();
            
            return update;
        }

    }
}
