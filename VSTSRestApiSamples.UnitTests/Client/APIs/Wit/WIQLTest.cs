using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using VstsRestApiSamples.ViewModels.Wit.Queries;
using VstsRestApiSamples.ViewModels.Wit;

namespace VSTSRestApiSamples.UnitTests.Client.APIs.Wit
{
    [TestClass]
    public class WIQLTest
    {
        private IAuth _auth;

        [TestInitialize]
        public void TestInitialize()
        {
            _auth = new VstsRestApiSamples.Tests.Client.Helpers.Auth();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _auth = null;
        }

        [TestMethod]
        public void Wit_WIQL_RunStoredQuery_Success()
        {
            //arrange
            WIQL request = new WIQL(_auth);

            //act
            GetWIQLRunStoredQueryResponse.WIQLResult response = request.RunStoredQuery(_auth.Project, _auth.QueryId);
            
            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }      
    }
}
