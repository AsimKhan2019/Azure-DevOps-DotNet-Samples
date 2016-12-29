using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;
using System.IO;

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
        public void WorkItemTracking_WorkItems_CreateWorkItem_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.CreateWorkItem(_configuration.Project);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_CreateWorkItemWithWorkItemLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.CreateWorkItemWithWorkItemLink(_configuration.Project, _configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_CreateWorkItemByPassingRules_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.CreateWorkItemByPassingRules(_configuration.Project);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_WorkItems_UpdateWorkItemUpdateField_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem updateResponse = request.UpdateWorkItemUpdateField(createResponse.id.ToString());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, updateResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemMoveWorkItem_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);
            string areaPath = _configuration.MoveToProject;         // user project name for root area path
            string iterationPath = _configuration.MoveToProject;    // use project name for root iteration path

            // act
            WorkItemPatchResponse.WorkItem response = request.UpdateWorkItemMoveWorkItem(_configuration.WorkItemId, _configuration.MoveToProject, areaPath, iterationPath);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.AreEqual(response.fields.SystemAreaPath, areaPath);
            Assert.AreEqual(response.fields.SystemIterationPath, iterationPath);
                       
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemChangeWorkItemType_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            ///create a task then change it to a user story
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem changeResponse = request.UpdateWorkItemChangeWorkItemType(createResponse.id.ToString());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, changeResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemAddTag_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem result = request.UpdateWorkItemAddTag(_configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemAddLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem updateResponse = request.UpdateWorkItemAddLink(createResponse.id.ToString(), _configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, updateResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemUpdateLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem addLinkResponse = request.UpdateWorkItemAddLink(createResponse.id.ToString(), _configuration.WorkItemId);
            WorkItemPatchResponse.WorkItem updateLinkResponse = request.UpdateWorkItemUpdateLink(createResponse.id.ToString(), _configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, addLinkResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, updateLinkResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemRemoveLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem addLinkResponse = request.UpdateWorkItemAddLink(createResponse.id.ToString(), _configuration.WorkItemId);
            WorkItemPatchResponse.WorkItem removeLinkResponse = request.UpdateWorkItemRemoveLink(createResponse.id.ToString());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, addLinkResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, removeLinkResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemAddAttachment_Success()
        {
            // arrange
            if (! File.Exists(@_configuration.FilePath))
            {
                Assert.Inconclusive("file not found: " + @_configuration.FilePath);
            }
            
            WorkItems request = new WorkItems(_configuration);
            Attachments attachmentsRequest = new Attachments(_configuration);

            // act
            //upload attachment
            var attachmentReference = attachmentsRequest.UploadAttachmentBinaryFile(_configuration.FilePath);

            //create work item then add attachment to that work item
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem attachmentResponse = request.UpdateWorkItemAddAttachment(createResponse.id.ToString(), attachmentReference.url);

            // assert    
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, attachmentResponse.HttpStatusCode);

            request = null;
            attachmentsRequest = null;
        }
        
        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemRemoveAttachment_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);
            Attachments attachmentsRequest = new Attachments(_configuration);

            // act
            //upload attachment
            var attachmentReference = attachmentsRequest.UploadAttachmentBinaryFile(_configuration.FilePath);

            //create work item then add attachment to that work item
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem addAttachmentResponse = request.UpdateWorkItemAddAttachment(createResponse.id.ToString(), attachmentReference.url);
            WorkItemPatchResponse.WorkItem removeAttachmentResponse = request.UpdateWorkItemRemoveAttachment(createResponse.id.ToString());

            // assert    
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, addAttachmentResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, removeAttachmentResponse.HttpStatusCode);

            request = null;
            attachmentsRequest = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemAddHyperLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            WorkItemPatchResponse.WorkItem addHyperLinkResponse = request.UpdateWorkItemAddHyperLink(createResponse.id.ToString());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, addHyperLinkResponse.HttpStatusCode);                    

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_DeleteWorkItem_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem createResponse = request.CreateWorkItem(_configuration.Project);
            var deleteResponse = request.DeleteWorkItem(createResponse.id.ToString());

            // assert
            Assert.AreEqual(HttpStatusCode.OK, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_WorkItems_AddCommitLink_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.UpdateWorkItemAddCommitLink("3045");

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
        public void WorkItemTracking_WorkItems_UpdateWorkItemByPassRules_Success()
        {
            // arrange
            WorkItems request = new WorkItems(_configuration);

            // act
            WorkItemPatchResponse.WorkItem response = request.UpdateWorkItemByPassingRules(_configuration.WorkItemId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
    }
}
