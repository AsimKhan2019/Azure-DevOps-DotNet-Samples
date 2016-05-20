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
        public void Wit_WorkItems_GetWorkItemExpandAll_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            var result = request.GetWorkItem("2583");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_Reporting_GetBatchOfWorkItemLinksForAll_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            BatchOfWorkItemLinksResponse.WorkItemLinks result = request.GetBatchOfWorkItemLinksAll();

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);          
        }

        [TestMethod]
        public void Wit_WorkItems_Reporting_GetBatchOfWorkItemRevisionsByProjectAndDate_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions result = request.GetBatchOfWorkItemRevisionsByDate(_auth.Project, new DateTime(2016, 4, 17));

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
            result = null;   
        }

        [TestMethod]
        public void Wit_WorkItems_Reporting_GetBatchOfWorkItemRevisionsForAll_Success()
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

        [TestMethod]
        public void Wit_WorkItems_UpdateWorkItem_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            WorkItemPatchResponse.WorkItem result = request.UpdateWorkItemFields(_auth.WorkItemId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_AddAndUpdateWorkItemTags_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            WorkItemPatchResponse.WorkItem result = request.AddWorkItemTags(_auth.WorkItemId, "Technical Debt; Spike Needed");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_MoveWorkItem_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);
            string areaPath = "Temp Agile";
            string iterationPath = "Temp Agile";

            //act
            WorkItemPatchResponse.WorkItem result = request.MoveWorkItem("2776", "Temp Agile", areaPath, iterationPath);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.AreEqual(result.fields.SystemAreaPath, areaPath);
            Assert.AreEqual(result.fields.SystemIterationPath, iterationPath);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_MoveWorkItemAndChangeType_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            WorkItemPatchResponse.WorkItem result = request.MoveWorkItemAndChangeType("2776", "Temp Scrum", "Temp Scrum", "Temp Scrum");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }
    }
}
