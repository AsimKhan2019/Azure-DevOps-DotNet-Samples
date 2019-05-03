using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
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
        public void GetAllAttachmentsOnWorkItem()
        {
            //this assumes you created a work item first from the WorkItemsSample.cs class
            //if not, you can manually add a work item id
            int workitemId = 0;
            string fileFullPath = Path.GetTempFileName();

            //check to see if the work item id is set in cache
            try
            {
                workitemId = Convert.ToInt32(Context.GetValue<WorkItem>("$newWorkItem1").Id);
            }
            catch (Exception)
            {
                Console.WriteLine("No work item found in cache. Either create a new work item and attachments or manually set the id for known work item id.");
                return;
            }           

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItem workitem = workItemTrackingClient.GetWorkItemAsync(workitemId, null, null, WorkItemExpand.Relations, null).Result;

            if (workitem == null)
            {
                Console.WriteLine("No work item found for id");
                return;
            }
          
            Console.WriteLine("Getting attachments on work item");

            if (workitem.Relations == null || workitem.Relations.Count == 0)
            {
                Console.WriteLine("No attachments found on work item");
                return;
            }

            foreach (var item in workitem.Relations)
            {
                if (item.Rel == "AttachedFile")
                {     
                    //string manipulation to get the guid off the end of the url                   
                    string[] splitString = item.Url.ToString().Split('/');                  
                    Guid attachmentId = new Guid(splitString[7].ToString());
                                               
                    Console.WriteLine("Getting attachment name and id: {0}", item.Attributes.GetValueOrDefault("name").ToString());

                    Stream attachmentStream = workItemTrackingClient.GetAttachmentContentAsync(attachmentId).Result;

                    using (FileStream writeStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        attachmentStream.CopyTo(writeStream);
                    }
                }
            }   
            
            return;
        }


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
