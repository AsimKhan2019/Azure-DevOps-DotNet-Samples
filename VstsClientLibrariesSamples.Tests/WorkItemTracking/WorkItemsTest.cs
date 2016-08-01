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
        private IConfiguration _config = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            InitHelper.GetConfiguration(_config);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _config = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_WorkItems_UpdateWorkItemsByQueryResults_Success()
        {
            //arrange
            IList<WorkItemReference> workItemsList = new List<WorkItemReference>();
            string[] workItemsArr = _config.WorkItemIds.Split(','); //get the list of ids from our app.config

            //build a list of work item references for ids we know exist
            foreach (string item in workItemsArr)
            {
                workItemsList.Add(new WorkItemReference() { Id = Convert.ToInt32(item) });
            }   

            WorkItemQueryResult workItemQueryResult = new WorkItemQueryResult();
            workItemQueryResult.WorkItems = workItemsList;

            //act
            WorkItems workItems = new WorkItems(_config);
            var result = workItems.UpdateWorkItemsByQueryResults(workItemQueryResult, _config.Identity);

            //assert
            Assert.AreEqual("success", result);
        }
    }
}
