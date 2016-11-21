using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using VstsClientLibrariesSamples.WorkItemTracking;

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
        public void WorkItemTracking_WorkItems_UpdateWorkItemsByQueryResults_Success()
        {
            // arrange
            IList<WorkItemReference> workItemsList = new List<WorkItemReference>();
            string[] workItemsArr = _configuration.WorkItemIds.Split(','); // get the list of ids from our app.config

            // build a list of work item references for ids we know exist
            foreach (string item in workItemsArr)
            {
                workItemsList.Add(new WorkItemReference() { Id = Convert.ToInt32(item) });
            }

            WorkItemQueryResult workItemQueryResult = new WorkItemQueryResult();
            workItemQueryResult.WorkItems = workItemsList;

            // act
            WorkItems workItems = new WorkItems(_configuration);
            var result = workItems.UpdateWorkItemsByQueryResults(workItemQueryResult, _configuration.Identity);

            // assert
            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_CreatWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.CreateWorkItem(_configuration.Project);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_UpdateWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.UpdateWorkItem(_configuration.WorkItemId);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_GetWorkItem_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.GetWorkItem(_configuration.WorkItemId);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_GetWorkItemHistory_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.GetWorkItemHistory(_configuration.WorkItemId);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_AddLink_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            string[] arr = _configuration.WorkItemIds.Split(',');

            // act
            var result = workItems.AddLink(Convert.ToInt32(arr[0]), Convert.ToInt32(arr[1]));

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_ChangeType_Success()
        {
            // arrange
            WorkItems workItems = new WorkItems(_configuration);

            // act
            var result = workItems.ChangeType(_configuration.WorkItemId);

            Assert.AreEqual("success", result);
        }
    }
}
