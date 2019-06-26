using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    public class GitSampleHelpers
    {
        public static GitRepository FindAnyRepositoryOnAnyProject(ClientSampleContext context)
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(context).Id;
            return FindAnyRepository(context, projectId);
        }

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

        public static string ChooseRefsafeName()
        {
            return $"{ChooseNamePart()}-{ChooseNamePart()}-{ChooseNamePart()}";
        }

        public static string ChooseItemsafeName()
        {
            return $"{ChooseNamePart()}.{ChooseNamePart()}.{ChooseNamePart()}";
        }

        public static GitPullRequest CreatePullRequest(ClientSampleContext context, GitRepository repo)
        {
            VssConnection connection = context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                GitPullRequest pullRequest = gitClient.GetPullRequestsAsync(
                    repo.Id,
                    new GitPullRequestSearchCriteria()
                    {
                        Status = PullRequestStatus.Active
                    }).Result.FirstOrDefault();
                
                if (pullRequest == null)
                {
                    pullRequest = CreatePullRequestInternal(context, repo, gitClient);
                }

                return pullRequest;
            }
        }

        public static GitPullRequest AbandonPullRequest(ClientSampleContext context, GitRepository repo, int pullRequestId)
        {
            VssConnection connection = context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                // clean up after ourselves (and in case logging is on, don't log these calls)
                ClientSampleHttpLogger.SetSuppressOutput(context, true);

                // abandon the PR
                GitPullRequest updatedPr = new GitPullRequest()
                {
                    Status = PullRequestStatus.Abandoned,
                };

                var pullRequest = gitClient.UpdatePullRequestAsync(updatedPr, repo.Id, pullRequestId).Result;

                return pullRequest;
            }
        }

        public static GitPullRequestStatus CreatePullRequestStatus(ClientSampleContext context, Guid repositoryId, int pullRequestId, int? iterationId = null)
        {
            VssConnection connection = context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                GitPullRequestStatus status = GenerateSampleStatus(iterationId);
                GitPullRequestStatus createdStatus = gitClient.CreatePullRequestStatusAsync(status, repositoryId, pullRequestId).Result;

                return createdStatus;
            }
        }

        public static string WithoutRefsPrefix(string refName)
        {
            if (!refName.StartsWith("refs/"))
            {
                throw new Exception("The ref name should have started with 'refs/' but it didn't.");
            }
            return refName.Remove(0, "refs/".Length);
        }

        public static GitPullRequestStatus GenerateSampleStatus(int? iterationId = null, bool includeProperties = false)
        {
            var status = new GitPullRequestStatus
            {
                Context = new GitStatusContext
                {
                    Name = $"sample-status-{Rng.Next(1, 5)}",
                    Genre = "vsts-samples"
                },
                TargetUrl = "http://fabrikam-fiber-inc.com/CI/builds/1",
                State = GitStatusState.Succeeded,
                Description = "Sample status succeeded",
                IterationId = iterationId
            };

            if (includeProperties)
            {
                status.Properties = new PropertiesCollection();
                status.Properties["sampleId"] = Rng.Next(1, 10);
                status.Properties["customInfo"] = "Custom status information";
                status.Properties["startedDateTime"] = DateTime.UtcNow;
                status.Properties["weight"] = 1.75;
                status.Properties["bytes"] = Encoding.UTF8.GetBytes("this is sample base64 encoded string");
                status.Properties["globalId"] = Guid.NewGuid();
            }

            return status;
        }

        private static GitPullRequest CreatePullRequestInternal(ClientSampleContext context, GitRepository repo, GitHttpClient gitClient)
        {
            // we need a new branch with changes in order to create a PR
            // first, find the default branch
            string defaultBranchName = WithoutRefsPrefix(repo.DefaultBranch);
            GitRef defaultBranch = gitClient.GetRefsAsync(repo.Id, filter: defaultBranchName).Result.First();

            // next, craft the branch and commit that we'll push
            GitRefUpdate newBranch = new GitRefUpdate()
            {
                Name = $"refs/heads/vsts-api-sample/{ChooseRefsafeName()}",
                OldObjectId = defaultBranch.ObjectId,
            };
            string newFileName = $"{ChooseItemsafeName()}.md";
            GitCommitRef newCommit = new GitCommitRef()
            {
                Comment = "Add a sample file",
                Changes = new GitChange[]
                {
                    new GitChange()
                    {
                        ChangeType = VersionControlChangeType.Add,
                        Item = new GitItem() { Path = $"/vsts-api-sample/{newFileName}" },
                        NewContent = new ItemContent()
                        {
                            Content = "# Thank you for using VSTS!",
                            ContentType = ItemContentType.RawText,
                        },
                    }
                },
            };

            // create the push with the new branch and commit
            GitPush push = gitClient.CreatePushAsync(new GitPush()
            {
                RefUpdates = new GitRefUpdate[] { newBranch },
                Commits = new GitCommitRef[] { newCommit },
            }, repo.Id).Result;

            // finally, create a PR
            var pr = gitClient.CreatePullRequestAsync(new GitPullRequest()
            {
                SourceRefName = newBranch.Name,
                TargetRefName = repo.DefaultBranch,
                Title = $"Add {newFileName} (from VSTS REST samples)",
                Description = "Adding this file from the pull request samples",
            },
            repo.Id).Result;

            return pr;

        }

        private static string ChooseNamePart()
        {
            if (WordList == null)
            {
                LoadWordList();
            }
            return WordList[Rng.Next(WordList.Count)];
        }

        private static void LoadWordList()
        {
            List<string> words = new List<string>();

            string wordListName = "Microsoft.TeamServices.Samples.Client.Git.WordList.txt";
            using (Stream inputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(wordListName))
            using (StreamReader reader = new StreamReader(inputStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        words.Add(line);
                    }
                }
                
            }

            WordList = words;
        }

        private static List<string> WordList;
        private static Random Rng = new Random();
    }
}
