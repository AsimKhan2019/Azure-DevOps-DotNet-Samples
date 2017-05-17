using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.Git
{
    public class GitSampleHelpers
    {
        public static GitRepository FindAnyRepository(ClientSampleContext context, Guid projectId)
        {
            GitRepository repo;
            if (!FindAnyRepository(context, projectId, out repo))
            {
                throw new Exception("No repositories available. Create a repo in this project and run the sample again.");
            }

            return repo;
        }

        private static bool FindAnyRepository(ClientSampleContext context, Guid projectId, out GitRepository repo)
        {
            // Check if we already have a repo loaded
            if (!context.TryGetValue<GitRepository>("$someRepo", out repo))
            {
                VssConnection connection = context.Connection;
                GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

                using (new ClientSampleHttpLoggerOutputSuppression())
                {
                    // Check if an ID was already set (this could have been provided by the caller)
                    Guid repoId;
                    if (!context.TryGetValue<Guid>("repositoryId", out repoId))
                    {
                        // Get the first repo
                        repo = gitClient.GetRepositoriesAsync(projectId).Result.FirstOrDefault();
                    }
                    else
                    {
                        // Get the details for this repo
                        repo = gitClient.GetRepositoryAsync(repoId.ToString()).Result;
                    }
                }

                if (repo != null)
                {
                    context.SetValue<GitRepository>("$someRepo", repo);
                }
                else
                {
                    // create a project here?
                    throw new Exception("No repos available for running the sample.");
                }
            }

            return repo != null;
        }
    }
}
