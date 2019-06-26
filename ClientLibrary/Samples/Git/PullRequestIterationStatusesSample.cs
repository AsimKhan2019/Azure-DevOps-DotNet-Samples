using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "pullRequestIterationStatuses")]
    public class PullRequestIterationStatusesSample : ClientSample
    {
        [ClientSampleMethod]
        public GitPullRequestStatus CreatePullRequestIterationStatus()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            GitPullRequestStatus status = GitSampleHelpers.GenerateSampleStatus();

            GitPullRequestStatus createdStatus = gitClient.CreatePullRequestIterationStatusAsync(status, repo.Id, pullRequest.PullRequestId, 1).Result;

            Console.WriteLine($"{createdStatus.Description}({createdStatus.Context.Genre}/{createdStatus.Context.Name}) with id {createdStatus.Id} created" +
                $" on iteration {createdStatus.IterationId}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return createdStatus;
        }

        [ClientSampleMethod]
        public List<GitPullRequestStatus> GetPullRequestIterationStatuses()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId, 1);
            GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId, 1);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            List<GitPullRequestStatus> iterationStatuses = gitClient.GetPullRequestIterationStatusesAsync(repo.Id, pullRequest.PullRequestId, 1).Result;

            Console.WriteLine($"{iterationStatuses.Count} statuses found for pull request {pullRequest.PullRequestId} iteration {1}");
            foreach (var status in iterationStatuses)
            {
                Console.WriteLine($"{status.Description}({status.Context.Genre}/{status.Context.Name}) with id {status.Id}");
            }

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return iterationStatuses;
        }

        [ClientSampleMethod]
        public GitPullRequestStatus GetPullRequestIterationStatus()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);
            GitPullRequestStatus generatedStatus = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId, 1);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            GitPullRequestStatus status = gitClient.GetPullRequestIterationStatusAsync(repo.Id, pullRequest.PullRequestId, 1, generatedStatus.Id).Result;

            Console.WriteLine($"{status.Description}({status.Context.Genre}/{status.Context.Name}) with id {status.Id}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return status;
        }

        [ClientSampleMethod]
        public void DeletePullRequestIterationStatus()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            GitPullRequestStatus status = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId, 1);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            gitClient.DeletePullRequestIterationStatusAsync(repo.Id, pullRequest.PullRequestId, 1, status.Id).SyncResult();

            Console.WriteLine($"Status {status.Id} deleted from pull request {pullRequest.PullRequestId}, iteration {1}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);
        }

        [ClientSampleMethod]
        public void UpdatePullRequestIterationStatuses()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            GitPullRequestStatus status1 = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId, 1);
            GitPullRequestStatus status2 = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId, 1);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            var patch = new JsonPatchDocument();
            patch.Add(new JsonPatchOperation() { Operation = VisualStudio.Services.WebApi.Patch.Operation.Remove, Path = $"/{status1.Id}" });
            patch.Add(new JsonPatchOperation() { Operation = VisualStudio.Services.WebApi.Patch.Operation.Remove, Path = $"/{status2.Id}" });

            gitClient.UpdatePullRequestIterationStatusesAsync(patch, repo.Id, pullRequest.PullRequestId, 1).SyncResult();

            Console.WriteLine($"Statuses {status1.Id}, and {status2.Id} deleted from the pull request {pullRequest.PullRequestId}, iteration {1}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);
        }
    }
}
