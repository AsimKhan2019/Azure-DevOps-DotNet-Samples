using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Reflection;

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
        public AttachmentReference UploadTextFile()
        {
            // Full path to the text file to upload as an attachment
            string filePath = ClientSampleHelpers.GetSampleTextFile();

            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            Console.WriteLine("Attempting upload of: {0}", filePath);

            // Upload the attachment
            AttachmentReference attachment = workItemTrackingClient.CreateAttachmentAsync(@filePath).Result;

            Console.WriteLine("Attachment created");            
            Console.WriteLine(" ID    : {0}", attachment.Id);
            Console.WriteLine(" URL   : {0}", attachment.Url);

            // Save the attachment ID for the "download" sample call later
            Context.SetValue<Guid>("$attachmentId", attachment.Id);
            Context.SetValue<string>("$attachmentFileName", Path.GetFileName(filePath));

            return attachment;
        }

        [ClientSampleMethod]
        public AttachmentReference UploadBinaryFile()
        {
            // Full path to the binary file to upload as an attachment
            string filePath = ClientSampleHelpers.GetSampleBinaryFile();

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            Console.WriteLine("Attempting upload of: {0}", filePath);

            AttachmentReference attachment = workItemTrackingClient.CreateAttachmentAsync(@filePath).Result;

            Console.WriteLine("Attachment created");
            Console.WriteLine(" ID    : {0}", attachment.Id);
            Console.WriteLine(" URL   : {0}", attachment.Url);

            return attachment;
        }

        [ClientSampleMethod]
        public bool DownloadAttachment()
        {
            Guid attachmentId;
            if (!Context.TryGetValue<Guid>("$attachmentId", out attachmentId))
            {
                throw new Exception("Run the upload attachent sample prior to running this sample.");
            }

            string fileFullPath = Path.GetTempFileName();

            // Get the client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get a stream for the attachment
            Stream attachmentStream = workItemTrackingClient.GetAttachmentContentAsync(attachmentId).Result;

            // Write the file to disk
            using (FileStream writeStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.ReadWrite))
            {
                attachmentStream.CopyTo(writeStream);
            }

            Console.WriteLine("Attachment downloaded");
            Console.WriteLine(" Full path: {0}", fileFullPath);

            return true;
        }
    }
}
