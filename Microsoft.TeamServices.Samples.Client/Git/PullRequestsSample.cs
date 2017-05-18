using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Git
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
    }
}
