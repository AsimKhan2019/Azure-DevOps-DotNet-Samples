using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using System.Net;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class SamplesTest
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
        public void WorkItemTracking_Samples_GetWorkItemsByQuery()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.GetWorkItemsByQuery();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Samples_GetWorkItemsByWiql()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.GetWorkItemsByWiql();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_Samples_CreateBug_Success()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.CreateBug();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Samples_CreateBugByPassingRules_Success()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.CreateBugByPassingRules();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Samples_UpdateBug_Success()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.UpdateBug();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Samples_AddLinkToBug_Success()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.AddLinkToBug();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Samples_AddAttachmentToBug_Success()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.AddAttachmentToBug();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Samples_AddCommentToBug_Success()
        {
            // arrange
            Samples samples = new Samples(_configuration);

            // act
            var response = samples.AddCommentToBug();

            // assert
            Assert.AreEqual("success", response);

            samples = null;
        }
    }
}
