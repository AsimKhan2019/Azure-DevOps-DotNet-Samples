using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.WorkItemTracking;

namespace VstsClientLibrariesSamples.Tests.QueryAndUpdateWorkItems
{
    [TestClass]
    public class SampleTest
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
        public void WorkItemTracking_Sample_QueryAndUpdateWorkItems_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            try
            {
                // act
                var result = sample.QueryAndUpdateWorkItems();

                Assert.AreEqual("success", result);
            }         
            catch (System.NullReferenceException ex)
            {
                Assert.Inconclusive(ex.Message);
            }           
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_CreateBug_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            // act
            var result = sample.CreateBug();

            Assert.AreEqual("success", result);           
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_CreateBugByPassingRules_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            // act
            var result = sample.CreateBugByPassingRules();

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_UpdateBug_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            // act
            var result = sample.UpdateBug();

            Assert.AreEqual("success", result);
        }        

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Samples_AddLinkToBug_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);
            
            // act
            var result = sample.AddLinkToBug();

            if (result.Contains("TF201035:"))
            {
                // assert
                Assert.Inconclusive("Circular relationship between work items. Remove links that are creating the cycle.");
            }
            else
            {
                // assert
                Assert.AreEqual("success", result);
            }            
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Samples_AddAttachmentToBug_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            // act
            var result = sample.AddAttachmentToBug();

            // assert
            Assert.AreEqual("success", result);
        }


        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_AddCommentsToBug_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            // act
            var result = sample.AddCommentsToBug();

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_QueryWorkItems_Query_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);

            // act
            var result = sample.QueryWorkItems_Query();

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_QueryWorkItems_Query_QueryNotFound()
        {
            // arrange
            Sample sample = new Sample(_configuration);
            _configuration.Query = "bad query";

            // act
            var result = sample.QueryWorkItems_Query();

            Assert.IsTrue(result.Contains("TF401243"));
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_Sample_QueryWorkItems_Wiql_Success()
        {
            // arrange
            Sample sample = new Sample(_configuration);                     
           
            // act
            var result = sample.QueryWorkItems_Wiql();

            if (result.Contains("did not find any results"))
            {
                Assert.Inconclusive("no results found for query");
            }
            else
            {
                Assert.AreEqual("success", result);
            }           
        }
    }
}
