using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "pullRequestStatuses")]
    public class PullRequestStatusesSample : ClientSample
    {
        [ClientSampleMethod]
        public GitPullRequestStatus CreatePullRequestStatus()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);
            
            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            GitPullRequestStatus status = GitSampleHelpers.GenerateSampleStatus();

            GitPullRequestStatus createdStatus = gitClient.CreatePullRequestStatusAsync(status, repo.Id, pullRequest.PullRequestId).Result;

            Console.WriteLine($"{createdStatus.Description}({createdStatus.Context.Genre}/{createdStatus.Context.Name}) with id {createdStatus.Id} created");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return createdStatus;
        }

        [ClientSampleMethod]
        public GitPullRequestStatus CreatePullRequestStatusWithIterationInBody()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            GitPullRequestStatus status = GitSampleHelpers.GenerateSampleStatus(1);

            GitPullRequestStatus createdStatus = gitClient.CreatePullRequestStatusAsync(status, repo.Id, pullRequest.PullRequestId).Result;

            Console.WriteLine($"{createdStatus.Description}({createdStatus.Context.Genre}/{createdStatus.Context.Name}) with id {createdStatus.Id} created"+
                $" on iteration {createdStatus.IterationId}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return createdStatus;
        }

        [ClientSampleMethod]
        public GitPullRequestStatus CreatePullRequestStatusWithCustomProperties()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            GitPullRequestStatus status = GitSampleHelpers.GenerateSampleStatus(includeProperties: true);

            GitPullRequestStatus createdStatus = gitClient.CreatePullRequestStatusAsync(status, repo.Id, pullRequest.PullRequestId).Result;

            Console.WriteLine($"{createdStatus.Description}({createdStatus.Context.Genre}/{createdStatus.Context.Name}) with id {createdStatus.Id} created");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return createdStatus;
        }

        [ClientSampleMethod]
        public List<GitPullRequestStatus> GetPullRequestStatuses()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId);
            GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            List<GitPullRequestStatus> statuses = gitClient.GetPullRequestStatusesAsync(repo.Id, pullRequest.PullRequestId).Result;

            Console.WriteLine($"{statuses.Count} statuses found for pull request {pullRequest.PullRequestId}");
            foreach (var status in statuses)
            {
                Console.WriteLine($"{status.Description}({status.Context.Genre}/{status.Context.Name}) with id {status.Id}");
            }

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return statuses;
        }

        [ClientSampleMethod]
        public GitPullRequestStatus GetPullRequestStatus()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);
            GitPullRequestStatus generatedStatus = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            GitPullRequestStatus status = gitClient.GetPullRequestStatusAsync(repo.Id, pullRequest.PullRequestId, generatedStatus.Id).Result;

            Console.WriteLine($"{status.Description}({status.Context.Genre}/{status.Context.Name}) with id {status.Id}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return status;
        }

        [ClientSampleMethod]
        public void DeletePullRequestStatus()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            GitPullRequestStatus status = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            gitClient.DeletePullRequestStatusAsync(repo.Id, pullRequest.PullRequestId, status.Id).SyncResult();

            Console.WriteLine($"Status {status.Id} deleted from pull request {pullRequest.PullRequestId}");
            
            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);
        }

        [ClientSampleMethod]
        public void UpdatePullRequestStatuses()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            GitPullRequestStatus status1 = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId);
            GitPullRequestStatus status2 = GitSampleHelpers.CreatePullRequestStatus(this.Context, repo.Id, pullRequest.PullRequestId);

            Console.WriteLine($"project {project.Name}, repo {repo.Name}, pullRequestId {pullRequest.PullRequestId}");

            var patch = new JsonPatchDocument();
            patch.Add(new JsonPatchOperation() { Operation = VisualStudio.Services.WebApi.Patch.Operation.Remove, Path = $"/{status1.Id}" });
            patch.Add(new JsonPatchOperation() { Operation = VisualStudio.Services.WebApi.Patch.Operation.Remove, Path = $"/{status2.Id}" });

            gitClient.UpdatePullRequestStatusesAsync(patch, repo.Id, pullRequest.PullRequestId).SyncResult();

            Console.WriteLine($"Statuses {status1.Id}, and {status2.Id} deleted from the pull request {pullRequest.PullRequestId}");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);
        }
    }
}
