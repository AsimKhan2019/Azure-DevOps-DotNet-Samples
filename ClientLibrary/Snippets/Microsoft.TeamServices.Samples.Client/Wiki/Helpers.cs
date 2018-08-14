using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamServices.Samples.Client.Wiki
{
    public static class Helpers
    {
        public static WikiV2 FindOrCreateProjectWiki(ClientSampleContext context)
        {
            VssConnection connection = context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(context).Id;

            List<WikiV2> wikis = wikiClient.GetAllWikisAsync(projectId).SyncResult();
            WikiV2 wikiToReturn = wikis != null && wikis.Count != 0
                ? wikis.Find(w => w.Type == WikiType.ProjectWiki)
                : null;

            if (wikiToReturn == null)
            {
                // No project wiki existing. Create one.
                var createParameters = new WikiCreateParametersV2()
                {
                    Name = "sampleProjectWiki",
                    ProjectId = projectId,
                    Type = WikiType.ProjectWiki
                };

                wikiToReturn = wikiClient.CreateWikiAsync(createParameters).SyncResult();
            } 
            
            return wikiToReturn;
        }

        public static WikiV2 FindOrCreateCodeWiki(ClientSampleContext context)
        {
            VssConnection connection = context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(context).Id;

            List<WikiV2> wikis = wikiClient.GetAllWikisAsync(projectId).SyncResult();
            WikiV2 wikiToReturn = wikis != null && wikis.Count != 0
                ? wikis.Find(w => w.Type == WikiType.CodeWiki)
                : null;

            if (wikiToReturn == null)
            {
                // No code wiki existing. Create one.
                GitHttpClient gitClient = connection.GetClient<GitHttpClient>();
                List<GitRepository> repositories = gitClient.GetRepositoriesAsync(projectId).Result;
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

                wikiToReturn = wikiClient.CreateWikiAsync(createParameters).SyncResult();
            }

            return wikiToReturn;
        }

        public static string GetAnyWikiPagePath(ClientSampleContext context, WikiV2 wiki)
        {
            VssConnection connection = context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiPage rootPage = wikiClient.GetPageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: "/",
                recursionLevel: VersionControlRecursionType.OneLevel).SyncResult().Page;

            return rootPage.SubPages[0].Path;
        }
    }
}
