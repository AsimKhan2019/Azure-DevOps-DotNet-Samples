using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    /// <summary>
    /// Client samples for getting work item icons in Team Services and Team Foundation Server.
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, "workItemIcons")]
    public class WorkItemIconsSample : ClientSample
    {

        [ClientSampleMethod]
        public List<WorkItemIcon> GetWorkItemIcons()
        {
            int[] workitemIds = new int[] { 1, 5, 6, 10 };

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemIcon> icons = workItemTrackingClient.GetWorkItemIconsAsync().Result;

            foreach (var icon in icons)
            {
                Console.WriteLine(" {0}: {1}", icon.Id, icon.Url);
            }

            return icons;
        }
    }
}
