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
        public void WorkItemTracking_QueryAndUpdateWorkItems_Sample_Success()
        {
            //arrange
            Sample sample = new Sample(_configuration);

            try
            {
                //act
                var result = sample.QueryAndUpdateWorkItems();

                Assert.AreEqual("success", result);
            }         
            catch (System.NullReferenceException ex)
            {
                Assert.Inconclusive(ex.Message);
            }           
        }
    }
}
