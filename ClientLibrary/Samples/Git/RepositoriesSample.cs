using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "repositories")]
    public class RepositoriesSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<GitRepository> ListRepositories()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            List<GitRepository> repos = gitClient.GetRepositoriesAsync(projectId).Result;
            
            foreach(GitRepository repo in repos)
            {
                Console.WriteLine("{0} {1} {2}", repo.Id, repo.Name, repo.RemoteUrl);
            }

            return repos;            
        }

        [ClientSampleMethod]
        public IEnumerable<GitCommitRef> ListCommitsForRepository()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            // Find a sample project to use for listing comments
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            // Get first repo in the project
            Guid repoId = gitClient.GetRepositoriesAsync(projectId).Result[0].Id;

            // Get no more than 10 commits
            GitQueryCommitsCriteria criteria = new GitQueryCommitsCriteria()
            {
                Top = 10
            };

            List<GitCommitRef> commits = gitClient.GetCommitsAsync(repoId, criteria).Result;

            foreach(GitCommitRef commit in commits)
            {
                Console.WriteLine("{0} by {1} ({2})", commit.CommitId, commit.Committer.Email, commit.Comment);
            }

            return commits;
        }
    }
}
