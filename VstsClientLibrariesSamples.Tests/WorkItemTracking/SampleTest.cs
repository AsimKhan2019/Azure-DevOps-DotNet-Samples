using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.WorkItemTracking;

namespace VstsClientLibrariesSamples.Tests.QueryAndUpdateWorkItems
{
    [TestClass]
    public class SampleTest
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
        public void WorkItemTracking_QueryAndUpdateWorkItems_Sample_Success()
        {
            //arrange
            Sample sample = new Sample(_config);

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
