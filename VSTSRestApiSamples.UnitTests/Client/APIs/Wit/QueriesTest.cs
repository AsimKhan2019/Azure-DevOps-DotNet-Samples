using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;

namespace VSTSRestApiSamples.UnitTests.Client.APIs.Wit
{
    [TestClass]
    public class QueriesTest
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
        public void Wit_Queries_GetListOfQueries_Success()
        {
            //arrange
            Queries request = new Queries(_auth);

            //act
            var statusCode = request.GetListOfQueries(_auth.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, statusCode);

            request = null;
        }
    }
}
