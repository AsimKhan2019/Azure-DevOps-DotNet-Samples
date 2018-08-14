using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.Wiki
{
    [ClientSample(WikiConstants.AreaName, WikiConstants.PagesResourceName)]
    public class WikiPagesSample : ClientSample
    {
        [ClientSampleMethod]
        public WikiPageResponse CreateWikiPage()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
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

            Context.Log("Create page '{0}' in wiki '{1}'", wikiPageResponse.Page.Path, wiki.Name);

            return wikiPageResponse;
        }

        [ClientSampleMethod]
        public WikiPageResponse GetWikiPageMetadata()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            string somePagePath = Helpers.GetAnyWikiPagePath(this.Context, wiki);

            WikiPageResponse somePageResponse = wikiClient.GetPageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                path: somePagePath).SyncResult();

            Context.Log("Retrieved page '{0}' metadata in wiki '{1}'", somePagePath, wiki.Name);

            return somePageResponse;
        }

        [ClientSampleMethod]
        public WikiPageResponse GetWikiPageMetadataWithContent()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            string somePagePath = Helpers.GetAnyWikiPagePath(this.Context, wiki);

            WikiPageResponse WikiPageResponse = wikiClient.GetPageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: somePagePath,
                includeContent: true).SyncResult();

            Context.Log("Retrieved page '{0}' metadata in wiki '{1}' with content '{2}'", WikiPageResponse.Page.Path, wiki.Name, WikiPageResponse.Page.Content);

            return WikiPageResponse;
        }

        [ClientSampleMethod]
        public WikiPageResponse GetWikiPageAndSubPages()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            WikiPageResponse rootPageResponse = wikiClient.GetPageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                path: "/",
                recursionLevel: VersionControlRecursionType.OneLevel).SyncResult();

            Context.Log("Retrieved the following subpages for the root page:");
            foreach (WikiPage subPage in rootPageResponse.Page.SubPages)
            {
                Context.Log("Sub-page : '{0}'", subPage.Path);
            }

            return rootPageResponse;
        }

        [ClientSampleMethod]
        public string GetWikiPageText()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            string somePagePath = Helpers.GetAnyWikiPagePath(this.Context, wiki);

            using (var reader = new StreamReader(wikiClient.GetPageTextAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: somePagePath).SyncResult()))
            {
                string pageContent = reader.ReadToEnd();
                Context.Log("Retrieved page '{0}' in wiki '{1}' with content '{2}'", somePagePath, wiki.Name, pageContent);

                return pageContent;
            }
        }

        [ClientSampleMethod]
        public WikiPageResponse EditWikiPage()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            string somePagePath = Helpers.GetAnyWikiPagePath(this.Context, wiki);

            WikiPageResponse pageResponse = wikiClient.GetPageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                path: somePagePath,
                includeContent: true).SyncResult();

            WikiPage somePage = pageResponse.Page;

            Context.Log("Retrieved page '{0}' as JSON in wiki '{1}' with content '{2}'", somePage.Path, wiki.Name, somePage.Content);

            var originalContent = somePage.Content;
            var originalVersion = pageResponse.ETag.ToList()[0];

            WikiPageCreateOrUpdateParameters parameters = new WikiPageCreateOrUpdateParameters()
            {
                Content = "New content for page"
            };

            WikiPageResponse editedPageResponse = wikiClient.CreateOrUpdatePageAsync(
                parameters: parameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                path: somePagePath,
                Version: originalVersion).SyncResult();

            var updatedContent = editedPageResponse.Page.Content;
            var updatedVersion = editedPageResponse.ETag.ToList()[0];

            Context.Log("Before editing --> Page path: {0}, version: {1}, content: {2}", somePage.Path, originalVersion, originalContent);
            Context.Log("After editing --> Page path: {0}, version: {1}, content: {2}", somePage.Path, updatedVersion, updatedContent);

            return editedPageResponse;
        }

        [ClientSampleMethod]
        public WikiPageResponse DeleteWikiPage()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            string somePagePath = Helpers.GetAnyWikiPagePath(this.Context, wiki);

            WikiPageResponse somePageResponse = wikiClient.DeletePageAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: somePagePath).SyncResult();

            Context.Log("Deleted page '{0}' from wiki '{1}'", somePagePath, wiki.Name);

            return somePageResponse;
        }
    }
}
