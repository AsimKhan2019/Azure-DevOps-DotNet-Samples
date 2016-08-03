using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Work.ProcessConfiguration;
using System.Net;

namespace VstsRestApiSamples.Tests.Work.ProcessConfiguration
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

        [TestMethod, TestCategory("REST API")]
        public void Work_ProcessCustomization_Fields_CreatePickListField()
        {
            //arrange
            Fields request = new Fields(_configuration);

            //act
            var response = request.CreatePickListField(_configuration.ProcessId, _configuration.PickListId);

            //assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound) {
                Assert.Inconclusive("picklist not found for given processid");
            }
            else {
                Assert.AreEqual(HttpStatusCode.Created, response.HttpStatusCode);
            }
           
            request = null;
        }     
    }
}
