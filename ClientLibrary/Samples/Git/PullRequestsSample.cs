using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "pullRequests")]
    public class PullRequestsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<GitPullRequest> ListPullRequestsIntoMaster()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);

            List<GitPullRequest> prs = gitClient.GetPullRequestsAsync(
                repo.Id,
                new GitPullRequestSearchCriteria()
                {
                    TargetRefName = "refs/heads/master",
                }).Result;

            Console.WriteLine("project {0}, repo {1}", project.Name, repo.Name);
            foreach (GitPullRequest pr in prs)
            {
                Console.WriteLine("{0} #{1} {2} -> {3}",
                    pr.Title.Substring(0, Math.Min(40, pr.Title.Length)),
                    pr.PullRequestId,
                    pr.SourceRefName,
                    pr.TargetRefName);
            }

            return prs;            
        }

        [ClientSampleMethod]
        public IEnumerable<GitPullRequest> ListPullRequestsForProject()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            List<GitPullRequest> prs = gitClient.GetPullRequestsByProjectAsync(project.Id, null).Result;

            Console.WriteLine("project {0}", project.Name);
            foreach (GitPullRequest pr in prs)
            {
                Console.WriteLine("{0} #{1} {2} -> {3}",
                    pr.Title.Substring(0, Math.Min(40, pr.Title.Length)),
                    pr.PullRequestId,
                    pr.SourceRefName,
                    pr.TargetRefName);
            }

            return prs;
        }

        [ClientSampleMethod]
        public GitPullRequest CreatePullRequest()
        {
            return CreatePullRequestInner(cleanUp: true);
        }

        public GitPullRequest CreatePullRequestInner(bool cleanUp)
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);

            // we need a new branch with changes in order to create a PR
            // first, find the default branch
            string defaultBranchName = GitSampleHelpers.WithoutRefsPrefix(repo.DefaultBranch);
            GitRef defaultBranch = gitClient.GetRefsAsync(repo.Id, filter: defaultBranchName).Result.First();

            // next, craft the branch and commit that we'll push
            GitRefUpdate newBranch = new GitRefUpdate()
            {
                Name = $"refs/heads/vsts-api-sample/{GitSampleHelpers.ChooseRefsafeName()}",
                OldObjectId = defaultBranch.ObjectId,
            };
            string newFileName = $"{GitSampleHelpers.ChooseItemsafeName()}.md";
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

            Console.WriteLine("project {0}, repo {1}", project.Name, repo.Name);
            Console.WriteLine("{0} (#{1}) {2} -> {3}",
                pr.Title.Substring(0, Math.Min(40, pr.Title.Length)),
                pr.PullRequestId,
                pr.SourceRefName,
                pr.TargetRefName);

            if (cleanUp)
            {
                // clean up after ourselves (and in case logging is on, don't log these calls)
                ClientSampleHttpLogger.SetSuppressOutput(this.Context, true);

                // abandon the PR
                GitPullRequest updatedPr = new GitPullRequest()
                {
                    Status = PullRequestStatus.Abandoned,
                };
                pr = gitClient.UpdatePullRequestAsync(updatedPr, repo.Id, pr.PullRequestId).Result;

                // delete the branch
                GitRefUpdateResult refDeleteResult = gitClient.UpdateRefsAsync(
                    new GitRefUpdate[]
                    {
                        new GitRefUpdate()
                        {
                            OldObjectId = push.RefUpdates.First().NewObjectId,
                            NewObjectId = new string('0', 40),
                            Name = push.RefUpdates.First().Name,
                        }
                    },
                    repositoryId: repo.Id).Result.First();
            }

            return pr;
        }

        [ClientSampleMethod]
        public GitPullRequest AbandonPullRequest()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);

            // first we need to create a pull request
            GitPullRequest pr = CreatePullRequestInner(cleanUp: false);

            // now abandon the PR
            GitPullRequest updatedPr = new GitPullRequest()
            {
                Status = PullRequestStatus.Abandoned,
            };
            GitPullRequest abandonedPr = gitClient.UpdatePullRequestAsync(updatedPr, repo.Id, pr.PullRequestId).Result;

            Console.WriteLine("{0} (#{1}) {2}",
                abandonedPr.Title.Substring(0, Math.Min(40, pr.Title.Length)),
                abandonedPr.PullRequestId,
                abandonedPr.Status);

            // delete the branch that was associated with the PR (and do not log)
            ClientSampleHttpLogger.SetSuppressOutput(this.Context, true);
            GitRefUpdateResult refDeleteResult = gitClient.UpdateRefsAsync(
                new GitRefUpdate[]
                {
                    new GitRefUpdate()
                    {
                        OldObjectId = gitClient.GetRefsAsync(repo.Id, filter: GitSampleHelpers.WithoutRefsPrefix(pr.SourceRefName)).Result.First().ObjectId,
                        NewObjectId = new string('0', 40),
                        Name = pr.SourceRefName,
                    }
                },
                repositoryId: repo.Id).Result.First();

            return abandonedPr;
        }

    }
}
