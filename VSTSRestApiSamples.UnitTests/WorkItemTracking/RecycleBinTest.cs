using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;
using System.Net;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class RecycleBinTest
    {
        private IConfiguration _configuration = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            InitHelper.GetConfiguration(_configuration);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _configuration = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_RecycleBin_GetDeletedItems_Success()
        {
            // arrange          
            RecycleBin request = new RecycleBin(_configuration);

            // act
            GetItemsFromRecycleBinResponse.WorkItems response = request.GetDeletedItems(_configuration.Project);
          
            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);        
         
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_RecycleBin_GetDeletedItem_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            RecycleBin recyclebinRequest = new RecycleBin(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem deleteResponse = workItemsRequest.DeleteWorkItem(createResponse.id.ToString());
            GetItemFromRecycleBinResponse.WorkItem getDeletedItemResponse = recyclebinRequest.GetDeletedItem(_configuration.Project, createResponse.id.ToString());
         
            //assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, getDeletedItemResponse.HttpStatusCode);           

            workItemsRequest = null;
            recyclebinRequest = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_RecycleBin_RestoreDeletedItem_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            RecycleBin recyclebinRequest = new RecycleBin(_configuration);

            // act
            //WorkItemPatchResponse.WorkItem createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            //WorkItemPatchResponse.WorkItem deleteResponse = workItemsRequest.DeleteWorkItem(createResponse.id.ToString());
            //GetItemFromRecycleBinResponse.WorkItem getDeletedItemResponse = recyclebinRequest.GetDeletedItem(_configuration.Project, createResponse.id.ToString());
            System.Net.HttpStatusCode restoreResponse = recyclebinRequest.RestoreWorkItem(_configuration.Project, "3212");

            //assert
            //Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            //Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);
            //Assert.AreEqual(HttpStatusCode.OK, getDeletedItemResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, restoreResponse);

            workItemsRequest = null;
        }
    }

}
