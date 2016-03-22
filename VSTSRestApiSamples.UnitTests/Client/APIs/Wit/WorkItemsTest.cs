using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using System.Net;

namespace vstsrestapisamples.tests.Client.APIs.Wit
{
    [TestClass]
    public class WorkItemsTest
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
        public void Wit_WorkItems_GetListOfWorkItemsByIDs_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            var statusCode = request.GetListOfWorkItemsByIDs("2247, 2473, 2456");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, statusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_WorkItems_GetListOfWorkItemsByIDsWithSpecificFields_Success()
        {
            //arrange
            WorkItems request = new WorkItems(_auth);

            //act
            var statusCode = request.GetListOfWorkItemsByIDsWithSpecificFields("2247, 2473");

            //assert
            Assert.AreEqual(HttpStatusCode.OK, statusCode);

            request = null;
        }
    }
}
