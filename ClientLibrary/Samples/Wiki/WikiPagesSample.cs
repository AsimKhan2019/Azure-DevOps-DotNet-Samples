using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.Wiki
{
    [ClientSample(WikiConstants.AreaName, WikiConstants.PagesResourceName)]
    public class WikiPagesSample : ClientSample
    {
        [ClientSampleMethod]
        public WikiPageResponse CreateWikiPage()
        {
            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);

            WikiPageResponse wikiPageResponse = Helpers.CreatePage(this.Context, wiki, "SamplePage" + new Random().Next(1, 999));
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
        public WikiPageResponse GetWikiPageByIdMetadata()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            int somePageId = Helpers.GetAnyWikiPageId(this.Context, wiki);

            WikiPageResponse somePageResponse = wikiClient.GetPageByIdAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                id: somePageId).SyncResult();

            Context.Log("Retrieved page with id : '{0}' metadata in wiki '{1}'", somePageId, wiki.Name);

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
        public WikiPageResponse GetWikiPageByIdMetadataWithContent()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            int somePageId = Helpers.GetAnyWikiPageId(this.Context, wiki);

            WikiPageResponse WikiPageResponse = wikiClient.GetPageByIdAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                id: somePageId,
                includeContent: true).SyncResult();

            Context.Log("Retrieved page with id : '{0}' metadata in wiki '{1}' with content '{2}'", WikiPageResponse.Page.Path, wiki.Name, WikiPageResponse.Page.Content);

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
        public WikiPageResponse GetWikiPageByIdAndSubPages()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            int somePageId = Helpers.GetAnyWikiPageId(this.Context, wiki);

            WikiPageResponse rootPageResponse = wikiClient.GetPageByIdAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                id: somePageId,
                recursionLevel: VersionControlRecursionType.OneLevel).SyncResult();

            if(rootPageResponse.Page.SubPages.Count() == 0)
            {
                WikiPageResponse wikiPageResponse1 = Helpers.CreatePage(this.Context, wiki, rootPageResponse.Page.Path + "/SubPage" + new Random().Next(1, 999));
                WikiPageResponse wikiPageResponse2 = Helpers.CreatePage(this.Context, wiki, rootPageResponse.Page.Path + "/SubPage" + new Random().Next(1, 999));

                rootPageResponse = wikiClient.GetPageByIdAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                id: somePageId,
                recursionLevel: VersionControlRecursionType.OneLevel).SyncResult();
            }
            
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
        public string GetWikiPageByIdText()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            int somePageId = Helpers.GetAnyWikiPageId(this.Context, wiki);

            using (var reader = new StreamReader(wikiClient.GetPageByIdTextAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                id: somePageId).SyncResult()))
            {
                string pageContent = reader.ReadToEnd();
                Context.Log("Retrieved page with id : '{0}' in wiki '{1}' with content '{2}'", somePageId, wiki.Name, pageContent);

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
        public WikiPageResponse EditWikiPageById()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            int somePageId = Helpers.GetAnyWikiPageId(this.Context, wiki);

            WikiPageResponse pageResponse = wikiClient.GetPageByIdAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                id: somePageId,
                includeContent: true).SyncResult();

            WikiPage somePage = pageResponse.Page;

            Context.Log("Retrieved page with Id '{0}' as JSON in wiki '{1}' with content '{2}'", somePage.Id, wiki.Name, somePage.Content);

            var originalContent = somePage.Content;
            var originalVersion = pageResponse.ETag.ToList()[0];

            WikiPageCreateOrUpdateParameters parameters = new WikiPageCreateOrUpdateParameters()
            {
                Content = "New content for page"
            };

            WikiPageResponse editedPageResponse = wikiClient.UpdatePageByIdAsync(
                parameters: parameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                id: somePageId,
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

        [ClientSampleMethod]
        public WikiPageResponse DeleteWikiPageById()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);
            int somePageId = Helpers.GetAnyWikiPageId(this.Context, wiki);

            WikiPageResponse somePageResponse = wikiClient.DeletePageByIdAsync(
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                id: somePageId).SyncResult();

            Context.Log("Deleted page with Id : '{0}' from wiki '{1}'", somePageId, wiki.Name);

            return somePageResponse;
        }
    }
}
