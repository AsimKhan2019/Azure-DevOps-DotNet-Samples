using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Tfvc
{
    [ClientSample(TfvcConstants.AreaName, "changesets")]
    public class ChangesetsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<TfvcChangesetRef> ListChangesets()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            IEnumerable<TfvcChangesetRef> changesets = tfvcClient.GetChangesetsAsync(top: 10).Result;

            foreach (TfvcChangesetRef changeset in changesets)
            {
                Console.WriteLine("{0} by {1}: {2}", changeset.ChangesetId, changeset.Author.DisplayName, changeset.Comment ?? "<no comment>");
            }

            return changesets;
        }

        [ClientSampleMethod]
        public TfvcChangesetRef CreateChange()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            DateTime time = DateTime.UtcNow;
            string destinationFilePath = string.Format("$/{0}/example-file-{1}.txt", projectName, time.ToString("yyyy-MM-dd-HH-mm-ss-ff"));
            string destinationFileContents = string.Format("File contents as of {0}", time);

            TfvcChangeset changeset = new TfvcChangeset()
            {
                Changes = new[]
                {
                    new TfvcChange()
                    {
                        ChangeType = VersionControlChangeType.Add,
                        Item = new TfvcItem()
                        {
                            Path = destinationFilePath,
                            ContentMetadata = new FileContentMetadata()
                            {
                                Encoding = Encoding.UTF8.WindowsCodePage,
                                ContentType = "text/plain",
                            }
                        },
                        NewContent = new ItemContent()
                        {
                            Content = destinationFileContents,
                            ContentType = ItemContentType.RawText,
                        },
                    },
                },
                Comment = "(sample) Adding a new changeset via API",
            };

            try
            {
                TfvcChangesetRef changesetRef = tfvcClient.CreateChangesetAsync(changeset).Result;
                Console.WriteLine("{0} by {1}: {2}", changesetRef.ChangesetId, changesetRef.Author.DisplayName, changesetRef.Comment ?? "<no comment>");
                return changesetRef;
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Something went wrong, could not create TFVC changeset.");
                if (e.InnerException.Message.Contains(projectName))
                {
                    Console.WriteLine("This may mean project \"{0}\" isn't configured for TFVC.", projectName);
                    Console.WriteLine("Add a TFVC repo to the project, then try this sample again.");
                }
                else
                {
                    Console.WriteLine(e.InnerException.Message);
                }
            }

            return null;
        }
    }
}
