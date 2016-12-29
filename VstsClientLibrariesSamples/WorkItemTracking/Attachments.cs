using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsClientLibrariesSamples.WorkItemTracking
{
    public class Attachments
    {
        private readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public Attachments(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public void DownloadAttachment(System.Guid id, string saveToFile)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                Stream attachmentStream = workItemTrackingHttpClient.GetAttachmentContentAsync(id).Result;

                int length = 256;
                int bytesRead;
                Byte[] buffer = new Byte[length];

                FileStream writeStream = new FileStream(@saveToFile, FileMode.Create, FileAccess.ReadWrite);
                bytesRead = attachmentStream.Read(buffer, 0, length);

                // read data write stream
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = attachmentStream.Read(buffer, 0, length);
                }

                attachmentStream.Close();
                writeStream.Close();
            }
        }

        public AttachmentReference UploadAttachmentTextFile(string filePath)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                AttachmentReference attachmentReference = workItemTrackingHttpClient.CreateAttachmentAsync(@filePath, "text").Result;
                return attachmentReference;
            }
        }

        public AttachmentReference UploadAttachmentBinaryFile(string filePath)
        {
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(_uri, _credentials))
            {
                AttachmentReference attachmentReference = workItemTrackingHttpClient.CreateAttachmentAsync(@filePath, "binary").Result;
                return attachmentReference;
            }
        }
    }
}
