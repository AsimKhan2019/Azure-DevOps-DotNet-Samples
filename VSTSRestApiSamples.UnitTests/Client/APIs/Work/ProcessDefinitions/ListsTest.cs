using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using VstsRestApiSamples.Client.APIs.Process;
using VstsRestApiSamples.Client.APIs.Work.ProcessDefinitions;

namespace VSTSRestApiSamples.UnitTests.Client.APIs.Work.ProcessDefinitions
{
    [TestClass]
    public class ListTests
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
        public void ProcessDefinitions_Work_Lists_CreatePickList_Success()
        {
            //arrange
            Lists request = new Lists(_auth);

            //act
            var result = request.CreatePickList(_auth.ProcessId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void ProcessDefinitions_Work_Lists_UpdatePickList_Success()
        {
            //arrange
            Lists request = new Lists(_auth);

            //act
            var result = request.UpdatePickList(_auth.ProcessId, _auth.PickListId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void ProcessDefinitions_Work_Lists_GetListOfPickLists_Success()
        {
            //arrange
            Lists request = new Lists(_auth);

            //act
            var result = request.GetListOfPickLists(_auth.ProcessId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void ProcessDefinitions_Work_Lists_GetPickList_Success()
        {
            //arrange
            Lists request = new Lists(_auth);

            //act
            var result = request.GetPickList(_auth.ProcessId, _auth.PickListId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }
    }
}
