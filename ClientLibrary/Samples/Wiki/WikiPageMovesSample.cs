using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.Wiki
{
    [ClientSample(WikiConstants.AreaName, WikiConstants.PageMovesResourceName)]
    public class WikiPageMovesSample : ClientSample
    {
        [ClientSampleMethod]
        public WikiPageMoveResponse ReparentWikiPage()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);

            var pageCreateParameters = new WikiPageCreateOrUpdateParameters()
            {
                Content = "Wiki page content",
            };

            var randomNumber = new Random().Next(1, 999);
            // First page
            string firstPagePath = "SamplePage" + randomNumber;
            WikiPageResponse firstPageResponse = wikiClient.CreateOrUpdatePageAsync(
                pageCreateParameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: firstPagePath,
                Version: null).SyncResult();

            Context.Log("Created page '{0}' in wiki '{1}'", firstPageResponse.Page.Path, wiki.Name);

            // Second page
            string secondPagePath = "SamplePage" + (randomNumber + 1);
            WikiPageResponse secondPageResponse = wikiClient.CreateOrUpdatePageAsync(
                pageCreateParameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id,
                path: secondPagePath,
                Version: null).SyncResult();

            Context.Log("Created page '{0}' in wiki '{1}'", secondPageResponse.Page.Path, wiki.Name);

            var pageMoveParameters = new WikiPageMoveParameters()
            {
                Path = firstPagePath,
                NewPath = secondPagePath + "/" + firstPagePath,
                NewOrder = 0,
            };

            WikiPageMoveResponse pageMoveResponse = wikiClient.CreatePageMoveAsync(
                pageMoveParameters: pageMoveParameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Id).SyncResult();

            Context.Log("Page moved from '{0}' to '{1}'", pageMoveResponse.PageMove.Path, pageMoveResponse.PageMove.NewPath);

            // Cleanup
            ClientSampleHttpLogger.SetSuppressOutput(this.Context, true);
            wikiClient.DeletePageAsync(wiki.ProjectId, wiki.Id, secondPagePath).SyncResult();

            return pageMoveResponse;
        }

        [ClientSampleMethod]
        public WikiPageMoveResponse ReorderWikiPage()
        {
            VssConnection connection = this.Context.Connection;
            WikiHttpClient wikiClient = connection.GetClient<WikiHttpClient>();

            WikiV2 wiki = Helpers.FindOrCreateProjectWiki(this.Context);

            var pageCreateParameters = new WikiPageCreateOrUpdateParameters()
            {
                Content = "Wiki page content",
            };

            var randomNumber = new Random().Next(1, 999);
            // First page
            string firstPagePath = "SamplePage" + randomNumber;
            WikiPageResponse firstPageResponse = wikiClient.CreateOrUpdatePageAsync(
                pageCreateParameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                path: firstPagePath,
                Version: null).SyncResult();

            Context.Log("Created page '{0}' in wiki '{1}'", firstPageResponse.Page.Path, wiki.Name);

            // Second page
            string secondPagePath = "SamplePage" + (randomNumber + 1);
            WikiPageResponse secondPageResponse = wikiClient.CreateOrUpdatePageAsync(
                pageCreateParameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name,
                path: secondPagePath,
                Version: null).SyncResult();

            Context.Log("Created page '{0}' in wiki '{1}'", secondPageResponse.Page.Path, wiki.Name);

            var pageMoveParameters = new WikiPageMoveParameters()
            {
                Path = firstPagePath,
                NewPath = firstPagePath,
                NewOrder = 0
            };

            WikiPageMoveResponse pageMoveResponse = wikiClient.CreatePageMoveAsync(
                pageMoveParameters: pageMoveParameters,
                project: wiki.ProjectId,
                wikiIdentifier: wiki.Name).SyncResult();

            Context.Log("Page '{0}' moved to order '{1}'", pageMoveResponse.PageMove.Path, pageMoveResponse.PageMove.NewOrder);

            // Cleanup
            ClientSampleHttpLogger.SetSuppressOutput(this.Context, true);
            wikiClient.DeletePageAsync(wiki.ProjectId, wiki.Id, firstPagePath).SyncResult();
            wikiClient.DeletePageAsync(wiki.ProjectId, wiki.Id, secondPagePath).SyncResult();

            return pageMoveResponse;
        }
    }
}
