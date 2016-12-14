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
        public void WorkItemTracking_WorkItems_GetWorkItemsByIDs_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            GetWorkItemsResponse.WorkItems response = request.GetWorkItemsByIDs(_configuration.WorkItemIds);

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
        public void WorkItemTracking_WorkItems_GetWorkItemsWithSpecificFields_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            GetWorkItemsResponse.WorkItems response = request.GetWorkItemsWithSpecificFields(_configuration.WorkItemIds);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_GetWorkItemsAsOfDate_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);
            DateTime asOfDate = DateTime.Now.AddDays(-90);

            // act
            GetWorkItemsResponse.WorkItems response = request.GetWorkItemsAsOfDate(_configuration.WorkItemIds, asOfDate);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_GetWorkItemsWithLinksAndAttachments_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            GetWorkItemsWithLinksAndAttachmentsResponse.WorkItems response = request.GetWorkItemsWithLinksAndAttachments(_configuration.WorkItemIds);

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
        public void WorkItemTracking_WorkItems_GetWorkItem_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            var response = request.GetWorkItem(_configuration.WorkItemId);
                    
            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("work item '" + _configuration.WorkItemId + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_GetWorkItemWithLinksAndAttachments_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            var response = request.GetWorkItemWithLinksAndAttachments(_configuration.WorkItemId);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("work item '" + _configuration.WorkItemId + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_GetWorkItemFullyExpanded_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            var response = request.GetWorkItemFullyExpanded(_configuration.WorkItemId);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("work item '" + _configuration.WorkItemId + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_GetDefaultValues_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            var response = request.GetDefaultValues("Task", _configuration.Project);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("work item type not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
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
        public void WorkItemTracking_WorkItems_AddHyperLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.AddHyperLink(_configuration.WorkItemId);

            // assert
            if (response.Message.ToLower().Contains("relation already exists"))
            {
                Assert.Inconclusive("Link already exists on bug");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }          

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_AddCommitLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.AddCommitLink("3045");

            // assert
            if (response.Message.ToLower().Contains("relation already exists"))
            {
                Assert.Inconclusive("Commit link already exists on bug");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

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
