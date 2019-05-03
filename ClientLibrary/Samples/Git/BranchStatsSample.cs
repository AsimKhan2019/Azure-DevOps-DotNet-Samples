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
    [ClientSample(GitWebApiConstants.AreaName, "branchStats")]
    public class BranchStatsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<GitBranchStats> GetBranchStatsForAFewBranches()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);

            // find a handful of branches to compare
            List<GitRef> branches = gitClient.GetRefsAsync(repo.Id, filter: "heads/").Result;
            IEnumerable<string> branchNames = from branch in branches
                                              where branch.Name.StartsWith("refs/heads/")
                                              select branch.Name.Substring("refs/heads/".Length);

            if (branches.Count < 1)
            {
                throw new Exception($"Repo {repo.Name} doesn't have any branches in it.");
            }

            if (string.IsNullOrEmpty(repo.DefaultBranch))
            {
                throw new Exception($"Repo {repo.Name} doesn't have a default branch");
            }

            string defaultBranchName = repo.DefaultBranch.Substring("refs/heads/".Length);

            // list up to 10 branches we're interested in comparing
            GitQueryBranchStatsCriteria criteria = new GitQueryBranchStatsCriteria()
            {
                baseVersionDescriptor = new GitVersionDescriptor
                {
                    VersionType = GitVersionType.Branch,
                    Version = defaultBranchName,
                },
                targetVersionDescriptors = branchNames
                    .Take(10)
                    .Select(branchName => new GitVersionDescriptor()
                    {
                        Version = branchName,
                        VersionType = GitVersionType.Branch,
                    })
                    .ToArray()
            };

            List<GitBranchStats> stats = gitClient.GetBranchStatsBatchAsync(criteria, repo.Id).Result;

            Console.WriteLine("project {0}, repo {1}", project.Name, repo.Name);
            foreach(GitBranchStats stat in stats)
            {
                Console.WriteLine(" branch `{0}` is {1} ahead, {2} behind `{3}`",
                    stat.Name, stat.AheadCount, stat.BehindCount, defaultBranchName);
            }

            return stats;            
        }
    }
}
