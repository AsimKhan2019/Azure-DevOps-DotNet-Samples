using System;
using System.Net;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class BatchTest
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
        public void WorkItemTracking_WorkItems_CreateAndLinkMultipleWorkItems_Success()
        {
            // arrange
            Batch request = new Batch(_configuration);

            // act
            WorkItemBatchPostResponse response = request.CreateAndLinkMultipleWorkItems(_configuration.Project);

            // assert
            foreach (WorkItemBatchPostResponse.Value value in response.values)
            {
                Assert.AreEqual(200, value.code);

                WorkItemPatchResponse.WorkItem workitem = JsonConvert.DeserializeObject<WorkItemPatchResponse.WorkItem>(value.body);
                Assert.IsTrue(workitem.relations.Length == 1);
            }

            request = null;
        }
    }
}
