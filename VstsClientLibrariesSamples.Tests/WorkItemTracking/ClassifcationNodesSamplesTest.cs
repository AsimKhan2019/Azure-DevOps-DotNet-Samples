using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using VstsClientLibrariesSamples.WorkItemTracking;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
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
        
        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodesSamples_GetAreasTree_Success()
        {
            // arrange
            ClassificationNodesSamples nodes = new ClassificationNodesSamples(_configuration);

            // act
            var list = nodes.GetFullTree(_configuration.Project, TreeStructureGroup.Areas);

            //assert
            Assert.IsNotNull(list);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodesSamples_GetIterationsTree_Success()
        {
            // arrange
            ClassificationNodesSamples nodes = new ClassificationNodesSamples(_configuration);

            // act
            var list = nodes.GetFullTree(_configuration.Project, TreeStructureGroup.Iterations);

            //assert
            Assert.IsNotNull(list);
        }
    }
}
