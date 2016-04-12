using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using System.Net;
using VstsRestApiSamples.ViewModels.Wit;

namespace vstsrestapisamples.tests.Client.APIs.Wit
{
    [TestClass]
    public class WorkItemsTest
    {
        private IAuth _auth;

        [TestInitialize]
        public void TestInitialize()
        {
            _auth = new VstsRestApiSamples.Tests.Client.Helpers.Auth();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _auth = null;
        }

        [TestMethod]
        public void Wit_WorkItems_GetListOfWorkItemsByIDs_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            ListofWorkItemsResponse.WorkItems result = request.GetListOfWorkItemsByIDs("2247, 2473, 2456");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_GetListOfWorkItemsByIDsWithSpecificFields_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            ListofWorkItemsResponse.WorkItems result = request.GetListOfWorkItemsByIDsWithSpecificFields("2247, 2473");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_GetBatchOfWorkItemLinksByProjectAndDate_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            BatchOfWorkItemLinksResponse.WorkItemLinks result = request.GetBatchOfWorkItemLinks(_auth.Project, new DateTime(2016, 3, 15));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;            
        }

        [TestMethod]
        public void Wit_WorkItems_GetBatchOfWorkItemLinksForAll_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            BatchOfWorkItemLinksResponse.WorkItemLinks result = request.GetBatchOfWorkItemLinksAll();

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);          
        }

        [TestMethod]
        public void Wit_WorkItems_GetBatchOfWorkItemRevisionsByProjectAndDate_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions result = request.GetBatchOfWorkItemRevisionsByDate(_auth.Project, new DateTime(2016, 4, 7));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
            result = null;   
        }

        [TestMethod]
        public void Wit_WorkItems_GetBatchOfWorkItemRevisionsForAll_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions result = request.GetBatchOfWorkItemRevisionsAll();

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
            result = null;
        }
    }
}
