using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTracking
{
    /// <summary>
    /// 
    /// Samples showing how to work with work item attachments.
    /// 
    /// See https://www.visualstudio.com/docs/integrate/api/wit/attachments for more details.
    /// 
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.Attachments)]
    public class AttachmentsSample : ClientSample
    {

        [ClientSampleMethod]
        public void DownloadAttachment(Guid attachmentId, string saveToFile)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();
            
            Stream attachmentStream = workItemTrackingClient.GetAttachmentContentAsync(attachmentId).Result;

            using (FileStream writeStream = new FileStream(@saveToFile, FileMode.Create, FileAccess.ReadWrite))
            {
                attachmentStream.CopyTo(writeStream);
            }
        }

        [ClientSampleMethod]
        public AttachmentReference UploadTextFile(string filePath)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            AttachmentReference attachmentReference = workItemTrackingClient.CreateAttachmentAsync(@filePath).Result;

            return attachmentReference;
        }

        [ClientSampleMethod]
        public AttachmentReference UploadBinaryFile(string filePath)
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            AttachmentReference attachmentReference = workItemTrackingClient.CreateAttachmentAsync(@filePath).Result;

            return attachmentReference;
        }
    }
}
