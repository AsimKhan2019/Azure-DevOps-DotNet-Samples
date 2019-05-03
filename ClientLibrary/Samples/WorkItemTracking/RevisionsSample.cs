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
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.Revisions)]
    public class RevisionsSample : ClientSample
    {
        [ClientSampleMethod]
        public List<WorkItem> GetListOfWorkItemRevisions()
        {
            int id = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItem> revisions = workItemTrackingClient.GetRevisionsAsync(id).Result;

            Console.WriteLine("Work Item Revisions...");

            foreach (var item in revisions)
            {
                Console.WriteLine("Id:           {0}", item.Id);
                Console.WriteLine("Revision:     {0}", item.Rev);
                Console.WriteLine("Fields");

                foreach (var field in item.Fields)
                {
                    Console.WriteLine("{0} : {1}", field.Key, field.Value);
                }

                Console.WriteLine();
            }

            return revisions;
        }

        [ClientSampleMethod]
        public List<WorkItem> GetListOfWorkItemRevisionsPaged()
        {
            int id = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            //skip revision 1 and give me the next 2
            List<WorkItem> revisions = workItemTrackingClient.GetRevisionsAsync(id, 2, 1).Result;

            Console.WriteLine("Work Item Revisions...");

            foreach (var item in revisions)
            {
                Console.WriteLine("Id:           {0}", item.Id);
                Console.WriteLine("Revision:     {0}", item.Rev);
                Console.WriteLine("Fields");

                foreach (var field in item.Fields)
                {
                    Console.WriteLine("{0} : {1}", field.Key, field.Value);
                }

                Console.WriteLine();
            }

            return revisions;
        }

        [ClientSampleMethod]
        public WorkItem GetWorkItemRevision()
        {
            int id = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // give me revision #2
            WorkItem revision = workItemTrackingClient.GetRevisionAsync(id, 2).Result;

            Console.WriteLine("Work Item Revision...");
            Console.WriteLine("Id:           {0}", revision.Id);
            Console.WriteLine("Revision:     {0}", revision.Rev);
            Console.WriteLine("Fields");

            foreach (var field in revision.Fields)
            {
                Console.WriteLine("{0} : {1}", field.Key, field.Value);
            }

            Console.WriteLine();
                       
            return revision;
        }
    }
}
