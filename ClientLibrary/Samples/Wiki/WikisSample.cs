using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.Wiki
{
    [ClientSample(WikiConstants.AreaName, WikiConstants.WikisResourceName)]
    public class WikisSample : ClientSample
    {
        [ClientSampleMethod]
        public WikiV2 CreateProjectWikiIfNotExisting()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            List<WikiV2> wikis = wikiClient.GetAllWikisAsync(projectId).SyncResult();

            WikiV2 createdWiki = null;
            var isProjectWikiExisting = false;
            if (wikis != null && wikis.Count > 0)
            {
                isProjectWikiExisting = wikis.Where(wiki => wiki.Type.Equals(WikiType.ProjectWiki)).Any();
            }

            if (isProjectWikiExisting == false)
            {
                // No project wiki existing. Create one.
                var createParameters = new WikiCreateParametersV2()
                {
                    Name = "sampleProjectWiki",
                    ProjectId = projectId,
                    Type = WikiType.ProjectWiki
                };

                createdWiki = wikiClient.CreateWikiAsync(createParameters).SyncResult();

                Context.Log("Created wiki with name '{0}' in project '{1}'", createdWiki.Name, createdWiki.ProjectId);
            }
            else
            {
                Context.Log("Project wiki already exists for this project.");
            }

            return createdWiki;            
        }

        [ClientSampleMethod]
        public WikiV2 CreateCodeWiki()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            List<GitRepository> repositories = gitClient.GetRepositoriesAsync(projectId).Result;

            WikiV2 createdWiki = null;
            Guid repositoryId = repositories[0].Id;
            
            var createParameters = new WikiCreateParametersV2()
            {
                Name = "sampleCodeWiki",
                ProjectId = projectId,
                RepositoryId = repositoryId,
                Type = WikiType.CodeWiki,
                MappedPath = "/",      // any folder path in the repository
                Version = new GitVersionDescriptor()
                {
                    Version = "master"
                }
            };

            createdWiki = wikiClient.CreateWikiAsync(createParameters).SyncResult();

            Context.Log("Created wiki with name '{0}' in project '{1}'", createdWiki.Name, createdWiki.ProjectId);

            // Cleanup
            ClientSampleHttpLogger.SetSuppressOutput(this.Context, true);
            wikiClient.DeleteWikiAsync(createdWiki.Id).SyncResult();

            return createdWiki;
        }

        [ClientSampleMethod]
        public WikiV2 GetWikiByName()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 existingWiki = Helpers.FindOrCreateCodeWiki(this.Context);

            WikiV2 wiki = wikiClient.GetWikiAsync(existingWiki.ProjectId, existingWiki.Name).SyncResult();

            Context.Log("Retrieved wiki with name '{0}' in project '{1}'", wiki.Name, wiki.ProjectId);

            return wiki;
        }

        [ClientSampleMethod]
        public WikiV2 GetWikiById()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 existingWiki = Helpers.FindOrCreateCodeWiki(this.Context);

            WikiV2 wiki = wikiClient.GetWikiAsync(existingWiki.Id).SyncResult();

            Context.Log("Retrieved wiki with name '{0}' in project '{1}'", wiki.Name, wiki.ProjectId);

            return wiki;
        }

        [ClientSampleMethod]
        public IEnumerable<WikiV2> GetAllWikisInAProject()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            List<WikiV2> wikis = wikiClient.GetAllWikisAsync(projectId).SyncResult();

            foreach (WikiV2 wiki in wikis)
            {
                Context.Log("Retrieved wiki with name '{0}' in project '{1}'", wiki.Name, wiki.ProjectId);
            }

            return wikis;
        }

        [ClientSampleMethod]
        public IEnumerable<WikiV2> GetAllWikisInACollection()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            List<WikiV2> wikis = wikiClient.GetAllWikisAsync().SyncResult();

            foreach (WikiV2 wiki in wikis)
            {
                Context.Log("Retrieved wiki with name '{0}' in project '{1}'", wiki.Name, wiki.ProjectId);
            }

            return wikis;
        }

        [ClientSampleMethod]
        public WikiV2 UpdateWiki()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 codeWiki = Helpers.FindOrCreateCodeWiki(this.Context);

            // Get the versions in that wiki
            List<GitVersionDescriptor> versions = codeWiki.Versions.ToList();

            // Append the new version
            List<GitBranchStats> branches = gitClient.GetBranchesAsync(codeWiki.ProjectId, codeWiki.RepositoryId).SyncResult();
            foreach(var branch in branches)
            {
                versions.Add(new GitVersionDescriptor()
                {
                    Version = branch.Name
                });
            }

            WikiUpdateParameters updateParams = new WikiUpdateParameters()
            {
                Versions = versions
            };

            WikiV2 updatedCodeWiki = wikiClient.UpdateWikiAsync(updateParams, codeWiki.ProjectId, codeWiki.Name).SyncResult();

            Context.Log("Updated wiki with name '{0}' in project '{1}'", updatedCodeWiki.Name, updatedCodeWiki.ProjectId);
            Context.Log("Updated versions are : {0}", string.Join(",", updatedCodeWiki.Versions.Select(v => v.Version)));
            
            return updatedCodeWiki;
        }

        [ClientSampleMethod]
        public WikiV2 DeleteCodeWiki()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 codeWiki = Helpers.FindOrCreateCodeWiki(this.Context);

            WikiV2 deletedWiki = wikiClient.DeleteWikiAsync(codeWiki.ProjectId, codeWiki.Name).SyncResult();

            Context.Log("Deleted wiki with name '{0}' in project '{1}'", deletedWiki.Name, deletedWiki.ProjectId);

            return deletedWiki;
        }
    }
}
