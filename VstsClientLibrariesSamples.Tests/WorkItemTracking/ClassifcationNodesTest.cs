using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using VstsClientLibrariesSamples.WorkItemTracking;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class ClassificationNodesTest
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
        public void WorkItemTracking_ClassificationNodes_GetAreas_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.GetAreas(_configuration.Project, 100);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_GetArea_Success()
        {
            // arrange
            string path = "Area Path Test 1A";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.GetArea(_configuration.Project, path);

            //assert
            if (result.Contains("VS402485:"))
            {
                Assert.Inconclusive("path '" + path + "' not found");
            }
            
            Assert.AreEqual("success", result);                        
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_CreateArea_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.CreateArea(_configuration.Project, "", "Foo");

            // assert
            if (result.Contains("VS402371:"))
            {
                Assert.Inconclusive("area path already exists");
            }
            else
            {
                Assert.AreEqual("success", result);
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_UpdateArea_Success()
        {
            // arrange
            string path = "Area Path Test 1A";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.UpdateArea(_configuration.Project, path, "Area Path Test 1A-Foo");

            //assert
            if (result.Contains("VS402485:"))
            {
                Assert.Inconclusive("path '" + path + "' not found");
            }

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_GetIterations_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.GetIterations(_configuration.Project, 100);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_GetIteration_Success()
        {
            // arrange
            string path = "Iteration Foo";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.GetIteration(_configuration.Project, path);

            //assert
            if (result.Contains("VS402485:"))
            {
                Assert.Inconclusive("path '" + path + "' not found");
            }

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_CreateIteration_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);
            string startDate = "11/28/2016";
            string finishDate = "12/16/2016";

            // act
            var result = nodes.CreateIteration(_configuration.Project, "Iteration Foo", startDate, finishDate);

            // assert
            if (result.Contains("VS402371:"))
            {
                Assert.Inconclusive("area path already exists");
            }
            else
            {
                Assert.AreEqual("success", result);
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_UpdateIteration_Success()
        {
            // arrange
            string name = "Iteration Foo";
            DateTime startDate = new DateTime(2016,12,28);
            DateTime finishDate = new DateTime(2017,1,7); 

            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.UpdateIteration(_configuration.Project, name, startDate, finishDate);

            //assert
            if (result.Contains("VS402485:"))
            {
                Assert.Inconclusive("name '" + name + "' not found");
            }

            Assert.AreEqual("success", result);
        }
    }
}
