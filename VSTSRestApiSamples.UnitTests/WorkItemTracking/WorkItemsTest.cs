using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class WorkItemsTest
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
        public void WorkItemTracking_WorkItems_GetListOfWorkItemsByIDs_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            ListofWorkItemsResponse.WorkItems response = request.GetListOfWorkItems_ByIDs(_configuration.WorkItemIds);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("work items '" + _configuration.WorkItemIds + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_GetListOfWorkItemsByIDs_WithSpecificFields_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            ListofWorkItemsResponse.WorkItems response = request.GetListOfWorkItems_ByIDsWithSpecificFields("2247, 2473");

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
        
        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_GetBatchOfWorkItemLinksByProjectAndDate_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            BatchOfWorkItemLinksResponse.WorkItemLinks response = request.GetBatchOfWorkItemLinks(_configuration.Project, new DateTime(2016, 3, 15));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;            
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_GetWorkItemExpandAll_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            var response = request.GetWorkItem("2583");

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_Reporting_GetBatchOfWorkItemLinksForAll_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            BatchOfWorkItemLinksResponse.WorkItemLinks response = request.GetBatchOfWorkItemLinksAll();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);          
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_Reporting_GetBatchOfWorkItemRevisions_ByProjectAndDate_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions response = request.GetBatchOfWorkItemRevisionsByDate(_configuration.Project, new DateTime(2016, 4, 17));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
            response = null;   
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_Reporting_GetBatchOfWorkItemRevisions_ForAll_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions response = request.GetBatchOfWorkItemRevisionsAll();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
            response = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_CreateWorkItemWithByPassRules_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.CreateWorkItemUsingByPassRules(_configuration.Project);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_CreateBug_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.CreateBug(_configuration.Project);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_UpdateWorkItem_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.UpdateWorkItemFields(_configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_UpdateWorkItemWithByPassRules_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.UpdateWorkItemFieldsWithByPassRules(_configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_AddLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            string[] arr = _configuration.WorkItemIds.Split(',');
                        
            // act
            WorkItemPatchResponse.WorkItem response = request.AddLink(arr[0].ToString(), arr[1].ToString());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UploadAttachment_Success()
        {
            // arrange
            WorkItems workItemsRequest = new WorkItems(_configuration);
            Attachments attachmentsRequest = new Attachments(_configuration);

            // act
            var attachmentReference = attachmentsRequest.UploadAttachment(_configuration.FilePath);

            var response = workItemsRequest.AddAttachment(_configuration.WorkItemId, attachmentReference.url);

            // assert    
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            workItemsRequest = null;
            attachmentsRequest = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_AddAndUpdateWorkItemTags_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem result = request.AddWorkItemTags(_configuration.WorkItemId, "Technical Debt; Spike Needed");

            // assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API"), Ignore]  
        public void WorkItemTracking_WorkItems_MoveWorkItem_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);
            string areaPath = _configuration.MoveToProject;         // user project name for root area path
            string iterationPath = _configuration.MoveToProject;    // use project name for root iteration path

            // act
            WorkItemPatchResponse.WorkItem response = request.MoveWorkItem(_configuration.WorkItemId, _configuration.MoveToProject, areaPath, iterationPath);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.AreEqual(response.fields.SystemAreaPath, areaPath);
            Assert.AreEqual(response.fields.SystemIterationPath, iterationPath);

            // move back
            WorkItemPatchResponse.WorkItem movebackResponse = request.MoveWorkItem(_configuration.WorkItemId, _configuration.Project, _configuration.Project, _configuration.Project);
            Assert.AreEqual(HttpStatusCode.OK, movebackResponse.HttpStatusCode);
 
            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_ChangeType_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.ChangeType(_configuration.WorkItemId, "User Story");
            var someme = response.ToString();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
    }
}
