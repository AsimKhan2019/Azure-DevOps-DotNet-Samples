using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "pullRequestProperties")]
    public class PullRequestPropertiesSample : ClientSample
    {
        [ClientSampleMethod]
        public PropertiesCollection GetPullRequestProperties()
        {
            VssConnection connection = Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                JsonPatchDocument patch = new JsonPatchDocument();
                patch.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/sampleId", Value = 8});
                patch.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/startedDateTime", Value = DateTime.UtcNow });

                gitClient.UpdatePullRequestPropertiesAsync(patch, repo.Id, pullRequest.PullRequestId).SyncResult();
            }

            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            PropertiesCollection properties = gitClient.GetPullRequestPropertiesAsync(repo.Id, pullRequest.PullRequestId).SyncResult();

            Console.WriteLine($"Pull request {pullRequest.PullRequestId} has {properties.Count} properties");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return properties;
        }

        [ClientSampleMethod]
        public PropertiesCollection AddPullRequestProperties()
        {
            VssConnection connection = Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            JsonPatchDocument patch = new JsonPatchDocument();
            patch.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/sampleId", Value = 8 });
            patch.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/startedDateTime", Value = DateTime.UtcNow });
            patch.Add(new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "",
                Value = new Dictionary<string, object>()
                {
                    { "bytes", Encoding.UTF8.GetBytes("this is sample base64 encoded string") },
                    { "globalId", Guid.NewGuid() }
                }
            });

            PropertiesCollection properties = gitClient.UpdatePullRequestPropertiesAsync(patch, repo.Id, pullRequest.PullRequestId).SyncResult();

            Console.WriteLine($"Pull request {pullRequest.PullRequestId} has {properties.Count} properties");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return properties;
        }

        [ClientSampleMethod]
        public PropertiesCollection RemoveAndReplacePullRequestProperties()
        {
            VssConnection connection = Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);
            GitPullRequest pullRequest = GitSampleHelpers.CreatePullRequest(this.Context, repo);

            Console.WriteLine("project {0}, repo {1}, pullRequestId {2}", project.Name, repo.Name, pullRequest.PullRequestId);

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                JsonPatchDocument init = new JsonPatchDocument();
                init.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/sampleId", Value = 8 });
                init.Add(new JsonPatchOperation() { Operation = Operation.Add, Path = "/startedDateTime", Value = DateTime.UtcNow });

                gitClient.UpdatePullRequestPropertiesAsync(init, repo.Id, pullRequest.PullRequestId).SyncResult();
            }

            JsonPatchDocument patch = new JsonPatchDocument();
            patch.Add(new JsonPatchOperation() { Operation = Operation.Replace, Path = "/sampleId", Value = 12 });
            patch.Add(new JsonPatchOperation() { Operation = Operation.Remove, Path = "/startedDateTime", Value = null });

            PropertiesCollection properties = gitClient.UpdatePullRequestPropertiesAsync(patch, repo.Id, pullRequest.PullRequestId).SyncResult();

            Console.WriteLine($"Pull request {pullRequest.PullRequestId} has {properties.Count} properties");

            GitSampleHelpers.AbandonPullRequest(this.Context, repo, pullRequest.PullRequestId);

            return properties;
        }
    }
}
