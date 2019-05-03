using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "commits")]
    public class CommitsSample : ClientSample
    {
        [ClientSampleMethod]
        public List<GitCommitRef> GetAllCommits()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsByAuthor()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    Author = "Norman Paulk"
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsByCommitter()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    Committer = "Fabrikamfiber16@hotmail.com"
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsOnABranch()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    ItemVersion = new GitVersionDescriptor()
                    {
                        VersionType = GitVersionType.Branch,
                        VersionOptions = GitVersionOptions.None,
                        Version = "master"
                    }
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsOnABranchAndInAPath()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    ItemVersion = new GitVersionDescriptor()
                    {
                        VersionType = GitVersionType.Branch,
                        VersionOptions = GitVersionOptions.None,
                        Version = "master"
                    },
                    ItemPath = "/debug.log"
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsInDateRange()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    FromDate = new DateTime(2018, 6, 14).ToString(),
                    ToDate = new DateTime(2018, 6, 16).ToString()
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsReachableFromACommit()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    // Earliest commit in the graph to search.
                    CompareVersion = m_oldestDescriptor
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsReachableFromACommitAndInPath()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria()
                {
                    CompareVersion = m_tipCommitDescriptor,
                    ItemVersion = m_oldestDescriptor,
                    ItemPath = "/README.md",
                }).Result;
        }

        [ClientSampleMethod]
        public List<GitCommitRef> GetCommitsPaging()
        {
            GitRepository repo = GitSampleHelpers.FindAnyRepositoryOnAnyProject(this.Context);

            return this.Context.Connection.GetClient<GitHttpClient>()
                .GetCommitsAsync(repo.Id, new GitQueryCommitsCriteria() { }, skip: 1, top: 2).Result;
        }

        private GitVersionDescriptor m_oldestDescriptor = new GitVersionDescriptor()
        {
            VersionType = GitVersionType.Commit,
            VersionOptions = GitVersionOptions.None,
            Version = "4fa42e1a7b0215cc70cd4e927cb70c422123af84"
        };

        private GitVersionDescriptor m_tipCommitDescriptor = new GitVersionDescriptor()
        {
            VersionType = GitVersionType.Branch,
            VersionOptions = GitVersionOptions.None,
            Version = "master"
        };
    }
}
