using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.DevOps.ClientSamples.Wiki
{
    [ClientSample(WikiConstants.AreaName, WikiConstants.AttachmentsResourceName)]
    public class WikiAttachmentsSample : ClientSample
    {
        [ClientSampleMethod]
        public WikiAttachmentResponse AddAttachment()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            Stream attachmentStream = File.OpenRead(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Content\Logo.png"));

            WikiAttachmentResponse attachmentResponse = wikiClient.CreateAttachmentAsync(
                uploadStream: attachmentStream.ConvertToBase64(),
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                name: "Attachment" + new Random().Next(0, 999) + ".png").SyncResult();

            Context.Log("Attachment '{0}' added to wiki '{1}'", attachmentResponse.Attachment.Name, wiki.Name);

            return attachmentResponse;
        }
    }
}
