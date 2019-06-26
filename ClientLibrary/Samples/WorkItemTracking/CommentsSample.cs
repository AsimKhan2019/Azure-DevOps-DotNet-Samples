using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, "workitemscomments")]
    public class CommentsSample : ClientSample
    {
        [ClientSampleMethod]
        public WorkItemComment GetSingleWorkItemComment()
        {
            WorkItem newWorkItem;
            
            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                WorkItemsSample witSample = new WorkItemsSample();
                witSample.Context = this.Context;
                newWorkItem = witSample.CreateWorkItem("Sample work item for comments", "Task");
                Context.SetValue<WorkItem>("$newWorkItem", newWorkItem);
            }

            int id = Convert.ToInt32(newWorkItem.Id);           

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemComment result = workItemTrackingClient.GetCommentAsync(id, 1).Result;

            Console.WriteLine("Revision: {0}", result.Revision);
            Console.WriteLine("Text: {0}", result.Text);

            return result;
        }

        [ClientSampleMethod]
        public WorkItemComments GetPageOfWorkItemComments()
        {
            int id = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem").Id);
            
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
