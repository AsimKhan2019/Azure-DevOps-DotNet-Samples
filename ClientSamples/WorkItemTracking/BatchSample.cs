using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsSamples.Client.WorkItemTracking
{
    public class BatchSample : ClientSample
    {

        public void Run()
        {
            VssConnection connection = this.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

           
        }

    }
}
