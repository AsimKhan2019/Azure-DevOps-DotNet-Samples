using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Work.ProcessDefinitions;
using System.Net;

namespace vstsrestapisamples.tests.Client.APIs.Work.ProcessDefinitions
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

        [TestMethod, Ignore]
        public void ProcessDefinitions_Work_Fields_CreatePickListField()
        {
            //arrange
            Fields request = new Fields(_auth);

            //act
            var result = request.CreatePickListField(_auth.ProcessId, _auth.PickListId);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, result.HttpStatusCode);

            request = null;
        }     
    }
}
