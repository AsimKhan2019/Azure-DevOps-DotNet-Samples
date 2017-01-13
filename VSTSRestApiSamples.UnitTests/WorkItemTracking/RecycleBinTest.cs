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
        public void WorkItemTracking_RecycleBin_RestoreItem_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            RecycleBin recyclebinRequest = new RecycleBin(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem deleteResponse = workItemsRequest.DeleteWorkItem(createResponse.id.ToString());
            GetRestoredWorkItemResponse.WorkItem restoreResponse = recyclebinRequest.RestoreItem(createResponse.id.ToString());

            ////get restored item
            GetWorkItemExpandAllResponse.WorkItem getRestoredItemResponse = workItemsRequest.GetWorkItem(createResponse.id.ToString());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);            
            Assert.AreEqual(HttpStatusCode.OK, restoreResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, getRestoredItemResponse.HttpStatusCode);

            workItemsRequest = null;
            recyclebinRequest = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_RecycleBin_RestoreMultipleItems_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            RecycleBin recyclebinRequest = new RecycleBin(_configuration);
            WorkItemPatchResponse.WorkItem createResponse;
            string[] ids = new string[3]; 

            // act
            createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            ids[0] = createResponse.id.ToString();

            createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            ids[1] = createResponse.id.ToString();

            createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            ids[2] = createResponse.id.ToString();

            foreach(var id in ids)
            {
                var deleteResponse = workItemsRequest.DeleteWorkItem(id);
            }

            var respond = recyclebinRequest.RestoreMultipleWorkItems(ids);
                    
            //assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);          
            
            workItemsRequest = null;
            recyclebinRequest = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_RecycleBin_PermenentlyDeletedItem_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            RecycleBin recyclebinRequest = new RecycleBin(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem deleteResponse = workItemsRequest.DeleteWorkItem(createResponse.id.ToString());
            HttpStatusCode permDeleteResponse = recyclebinRequest.PermenentlyDeleteItem(createResponse.id.ToString());
            
         
            ////get delete item
            GetWorkItemExpandAllResponse.WorkItem getDeletedWorkItem = workItemsRequest.GetWorkItem(createResponse.id.ToString());

            //assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, permDeleteResponse);
            Assert.AreEqual(HttpStatusCode.NoContent, getDeletedWorkItem.HttpStatusCode);

            workItemsRequest = null;
            recyclebinRequest = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_RecycleBin_PermenentlyDeleteMultipleItems_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            RecycleBin recyclebinRequest = new RecycleBin(_configuration);
            WorkItemPatchResponse.WorkItem createResponse;
            string[] ids = new string[3];

            // act
            createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            ids[0] = createResponse.id.ToString();

            createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            ids[1] = createResponse.id.ToString();

            createResponse = workItemsRequest.CreateWorkItem(_configuration.Project);
            ids[2] = createResponse.id.ToString();

            foreach (var id in ids)
            {
                var deleteResponse = workItemsRequest.DeleteWorkItem(id);
            }

            GetRestoreMultipleWorkItemsResponse.Items response = recyclebinRequest.PeremenentlyDeleteMultipleItems(ids);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            workItemsRequest = null;
            recyclebinRequest = null;
        }
    }

}
