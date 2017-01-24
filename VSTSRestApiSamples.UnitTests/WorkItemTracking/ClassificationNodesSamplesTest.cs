using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;
using System.Net;
using System.Collections.Generic;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class ClassificationNodesSamplesTest
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
        public void WorkItemTracking_Nodes_Samples_GetAreaTree_Success()
        {
            // arrange
            ClassificationNodesSamples request = new ClassificationNodesSamples(_configuration);

            // act
            List<string> response = request.GetAreaTree(_configuration.Project);

            //assert
            Assert.IsNotNull(response);
           
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_Samples_GetIterationTree_Success()
        {
            // arrange
            ClassificationNodesSamples request = new ClassificationNodesSamples(_configuration);

            // act
            List<string> response = request.GetIterationTree(_configuration.Project);

            //assert
            Assert.IsTrue(response.Count > 0);

            request = null;
        }
    }
}
