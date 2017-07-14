using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.WorkItems)]
    public class CommentsSample : ClientSample
    {
        [ClientSampleMethod]
        public WorkItemComment GetSingleWorkItemComment()
        {
            int id = 23; //TODO
            int revision = 1;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemComment result = workItemTrackingClient.GetCommentAsync(id, revision).Result;

            Console.WriteLine("Revision: {0}", result.Revision);
            Console.WriteLine("Text: {0}", result.Text);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemComments GetPageOfWorkItemComments()
        {
            int id = 23; //TODO           

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemComments result = workItemTrackingClient.GetCommentsAsync(id, 1).Result;

            Console.WriteLine("Total Revision Count: {0}", result.TotalCount);
            Console.WriteLine("From Revision Count: {0}", result.FromRevisionCount);
            Console.WriteLine("Comments...");
            
            foreach(var comment in result.Comments)
            {
                Console.WriteLine("{0}", comment.Text);
                Console.WriteLine();
            }

            return result;
        }
    }
}
