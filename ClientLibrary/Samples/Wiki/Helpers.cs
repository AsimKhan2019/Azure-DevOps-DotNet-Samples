using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Wiki
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

        public static int GetAnyWikiPageId(ClientSampleContext context, WikiV2 wiki)
        {
            string path = GetAnyWikiPagePath(context, wiki);
            VssConnection connection = context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiPage anyPage = wikiClient.GetPageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: path,
                recursionLevel: VersionControlRecursionType.OneLevel).SyncResult().Page;

            if (!anyPage.Id.HasValue)
            {
                WikiPageCreateOrUpdateParameters parameters = new WikiPageCreateOrUpdateParameters()
                {
                    Content = "Wiki page content"
                };

                WikiPageResponse wikiPageResponse = wikiClient.CreateOrUpdatePageAsync(
                parameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: "SamplePage" + new Random().Next(1, 999),
                Version: null).SyncResult();

                context.Log("Create page '{0}' in wiki '{1}'", wikiPageResponse.Page.Path, wiki.Name);

                anyPage = wikiPageResponse.Page;
            }

            return anyPage.Id.Value;
        }

        public static WikiPageResponse CreatePage(ClientSampleContext context, WikiV2 wiki, string path)
        {
            VssConnection connection = context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiPageCreateOrUpdateParameters parameters = new WikiPageCreateOrUpdateParameters()
            {
                Content = "Wiki page content"
            };

            WikiPageResponse wikiPageResponse = wikiClient.CreateOrUpdatePageAsync(
                parameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: path,
                Version: null).SyncResult();

            return wikiPageResponse;
        }
    }
}
