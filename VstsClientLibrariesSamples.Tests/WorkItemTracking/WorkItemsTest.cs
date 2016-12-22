using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using VstsClientLibrariesSamples.WorkItemTracking;
using System.IO;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
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

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_GetWorkItemsByIDs_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);
            var split = _configuration.WorkItemIds.Split(',');
            var ids = new List<int>();

            foreach(string item in split)
            {               
                ids.Add(Convert.ToInt32(item));
            }

            // act
            var result = workItems.GetWorkItemsByIDs(ids);

            //assert
            Assert.IsNotNull(result);
        }

        public void CL_WorkItemTracking_WorkItems_GetWorkItemsWithSpecificFields_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);
            var split = _configuration.WorkItemIds.Split(',');
            var ids = new List<int>();

            foreach (string item in split)
            {
                ids.Add(Convert.ToInt32(item));
            }

            // act
            var result = workItems.GetWorkItemsWithSpecificFields(ids);

            //assert
            Assert.IsNotNull(result);
        }

        public void CL_WorkItemTracking_WorkItems_GetWorkItemsAsOfDate_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);
            var asOfDate = new DateTime().AddDays(-30);
            var split = _configuration.WorkItemIds.Split(',');
            var ids = new List<int>();

            foreach (string item in split)
            {
                ids.Add(Convert.ToInt32(item));
            }

            // act
            var result = workItems.GetWorkItemsAsOfDate(ids, asOfDate);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_GetWorkItemsWithLinksAndAttachments_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);
            var split = _configuration.WorkItemIds.Split(',');
            var ids = new List<int>();

            foreach (string item in split)
            {
                ids.Add(Convert.ToInt32(item));
            }

            // act
            var result = workItems.GetWorkItemsWithLinksAndAttachments(ids);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_GetWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.GetWorkItem(_configuration.WorkItemId);
                        
            Assert.IsNotNull(result);                       
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_GetWorkItemWithLinksAndAttachments_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.GetWorkItemWithLinksAndAttachments(_configuration.WorkItemId);

            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_GetWorkItemFullyExpanded_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.GetWorkItemFullyExpanded(_configuration.WorkItemId);

            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_CreateWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.CreateWorkItem(_configuration.Project);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_CreateWorkItemWithWorkItemLink_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var updateResult = workItems.CreateWorkItemWithWorkItemLink(_configuration.Project, createResult.Url);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_CreateWorkItemByPassingRules_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.CreateWorkItemByPassingRules(_configuration.Project);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemUpdateField_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var updateResult = workItems.UpdateWorkItemUpdateField(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemMoveWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);
            string project = _configuration.MoveToProject;
            string areaPath = _configuration.MoveToProject;         // use project name for root area path
            string iterationPath = _configuration.MoveToProject;    // use project name for root iteration path

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var moveResult = workItems.UpdateWorkItemMoveWorkItem(id, project, areaPath, iterationPath);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(moveResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemChangeWorkItemType_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);       
           
            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var changeResult = workItems.UpdateWorkItemChangeWorkItemType(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(changeResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemAddTag_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var updateResult = workItems.UpdateWorkItemAddTag(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemUpdateLink_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createOneResult = workItems.CreateWorkItem(_configuration.Project);
            var createTwoResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createOneResult.Id ?? default(int);
           
            var updateResult = workItems.UpdateWorkItemUpdateLink(id, createTwoResult.Url);

            //assert
            Assert.IsNotNull(createOneResult);
            Assert.IsNotNull(createTwoResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemRemoveLink_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createOneResult = workItems.CreateWorkItem(_configuration.Project); //create wi 1
            var createTwoResult = workItems.CreateWorkItem(_configuration.Project); //creaet wi 2
            var id = createOneResult.Id ?? default(int);

            var updateResult = workItems.UpdateWorkItemUpdateLink(id, createTwoResult.Url); //link on wi #1 to wi #2
            var removeResult = workItems.UpdateWorkItemRemoveLink(id); //remove link from wi #1

            //assert
            Assert.IsNotNull(createOneResult);
            Assert.IsNotNull(createTwoResult);
            Assert.IsNotNull(updateResult);
            Assert.IsNotNull(removeResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemAddAttachment_Success()
        {
            string filePath = @"D:\Temp\Test.txt";

            if (! File.Exists(filePath))
            {
                Assert.Inconclusive("File path '" + filePath + "' not found");
            }

            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createOneResult = workItems.CreateWorkItem(_configuration.Project);           
            var id = createOneResult.Id ?? default(int);

            var updateResult = workItems.UpdateWorkItemAddAttachment(id, @"D:\Temp\Test.txt");

            //assert
            Assert.IsNotNull(createOneResult);           
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemRemoveAttachment_Success()
        {
            string filePath = @"D:\Temp\Test.txt";

            if (!File.Exists(filePath))
            {
                Assert.Inconclusive("File path '" + filePath + "' not found");
            }

            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createOneResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createOneResult.Id ?? default(int);

            var addAttachmentResult = workItems.UpdateWorkItemAddAttachment(id, @"D:\Temp\Test.txt");
            var removeAttachmentResult = workItems.UpdateWorkItemRemoveAttachment(id, "0");

            //assert
            Assert.IsNotNull(createOneResult);
            Assert.IsNotNull(addAttachmentResult);
            Assert.IsNotNull(removeAttachmentResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemAddHyperLink_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var updateResult = workItems.UpdateWorkItemAddHyperLink(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemAddCommitLink_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var updateResult = workItems.UpdateWorkItemAddCommitLink(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_UpdateWorkItemUsingByPassRules_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var updateResult = workItems.UpdateWorkItemUsingByPassRules(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_WorkItems_DeleteWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var createResult = workItems.CreateWorkItem(_configuration.Project);
            var id = createResult.Id ?? default(int);
            var deleteResult = workItems.DeleteWorkItem(id);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(deleteResult);
        }

    }
}
