using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Build2;
using System.Net;

namespace vstsrestapisamples.tests.Client.APIs.Build2
{
    [TestClass]
    public class BuildTest
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
        public void Build_Defintions_GetListOfBuildDefinitions_Success()
        {
            //arrange
            Build request = new Build(_auth);

            //act
            var result = request.GetListOfBuildDefinitions(_auth.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }
    }
}
