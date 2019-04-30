using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.Tfvc
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

        [ClientSampleMethod]
        public TfvcChangesetRef CreateChangeMultiFile()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            DateTime time = DateTime.UtcNow;
            string destinationFilePath1 = string.Format("$/{0}/example-file-{1}.1.txt", projectName, time.ToString("yyyy-MM-dd-HH-mm-ss-ff"));
            string destinationFileContents1 = string.Format("File 1 contents as of {0}", time);
            string destinationFilePath2 = string.Format("$/{0}/example-file-{1}.2.txt", projectName, time.ToString("yyyy-MM-dd-HH-mm-ss-ff"));
            string destinationFileContents2 = string.Format("File 2 contents as of {0}", time);

            TfvcChangeset changeset = new TfvcChangeset()
            {
                Changes = new[]
                {
                    new TfvcChange()
                    {
                        ChangeType = VersionControlChangeType.Add,
                        Item = new TfvcItem()
                        {
                            Path = destinationFilePath1,
                            ContentMetadata = new FileContentMetadata()
                            {
                                Encoding = Encoding.UTF8.WindowsCodePage,
                                ContentType = "text/plain",
                            }
                        },
                        NewContent = new ItemContent()
                        {
                            Content = destinationFileContents1,
                            ContentType = ItemContentType.RawText,
                        },
                    },
                    new TfvcChange()
                    {
                        ChangeType = VersionControlChangeType.Add,
                        Item = new TfvcItem()
                        {
                            Path = destinationFilePath2,
                            ContentMetadata = new FileContentMetadata()
                            {
                                Encoding = Encoding.UTF8.WindowsCodePage,
                                ContentType = "text/plain",
                            }
                        },
                        NewContent = new ItemContent()
                        {
                            Content = destinationFileContents2,
                            ContentType = ItemContentType.RawText,
                        },
                    },
                },
                Comment = "(sample) Adding multiple files via API",
            };

            try
            {
                Console.WriteLine("Writing files {0} and {1}", destinationFilePath1, destinationFilePath2);
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

        [ClientSampleMethod]
        public TfvcChangesetRef EditExistingFile()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            // first, create a file we know is safe to edit
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            DateTime time = DateTime.UtcNow;
            string destinationFilePath = string.Format("$/{0}/file-to-edit-{1}.txt", projectName, time.ToString("yyyy-MM-dd-HH-mm-ss-ff"));
            string originalFileContents = string.Format("Initial contents as of {0}", time);

            TfvcChangeset createFile = new TfvcChangeset()
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
                            Content = originalFileContents,
                            ContentType = ItemContentType.RawText,
                        },
                    },
                },
                Comment = "(sample) Adding a file which we'll later edit",
            };

            TfvcChangesetRef createFileRef;
            try
            {
                createFileRef = tfvcClient.CreateChangesetAsync(createFile).Result;
                Console.WriteLine("{0} by {1}: {2}", createFileRef.ChangesetId, createFileRef.Author.DisplayName, createFileRef.Comment ?? "<no comment>");
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
                return null;
            }

            // now edit the file contents
            string editedFileContents = originalFileContents + "\nEdited contents";
            TfvcChangeset changeset = new TfvcChangeset()
            {
                Changes = new[]
                {
                    new TfvcChange()
                    {
                        ChangeType = VersionControlChangeType.Edit,
                        Item = new TfvcItem()
                        {
                            Path = destinationFilePath,
                            ContentMetadata = new FileContentMetadata()
                            {
                                Encoding = Encoding.UTF8.WindowsCodePage,
                                ContentType = "text/plain",
                            },
                            // must tell the API what version we want to change
                            ChangesetVersion = createFileRef.ChangesetId,
                        },
                        NewContent = new ItemContent()
                        {
                            Content = editedFileContents,
                            ContentType = ItemContentType.RawText,
                        },
                    },
                },
                Comment = "(sample) Editing the file via API",
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
