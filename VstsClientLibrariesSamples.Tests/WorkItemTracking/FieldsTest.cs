using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.WorkItemTracking;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class FieldsTest
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

        [TestMethod]
        public void Fields_GetListOfWorkItemFields()
        {
            // arrange
            Fields fields = new Fields(_configuration);

            // act
            var result = fields.GetListOfWorkItemFields("Title");

            //assert
            Assert.AreEqual("System.Title", result);
        }

        public void Fields_GetField()
        {
        }
    }
}
