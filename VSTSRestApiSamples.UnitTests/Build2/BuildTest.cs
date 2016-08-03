using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Build2;
using System.Net;

namespace VstsRestApiSamples.Tests.Build2
{
    [TestClass]
    public class BuildTest
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
        public void Build_Defintions_GetListOfBuildDefinitions_Success()
        {
            //arrange
            Build request = new Build(_configuration);

            //act
            var response = request.GetListOfBuildDefinitions(_configuration.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
    }
}
