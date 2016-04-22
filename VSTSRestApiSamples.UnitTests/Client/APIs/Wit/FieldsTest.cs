using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using System.Net;

namespace vstsrestapisamples.tests.Client.APIs.Wit
{
    [TestClass]
    public class FieldsTest
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
            Fields request = new Fields(_auth);

            //act
            var result = request.GetListOfWorkItemFields();

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }       
    }
}
