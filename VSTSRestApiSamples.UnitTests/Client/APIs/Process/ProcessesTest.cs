using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using VstsRestApiSamples.Client.APIs.Process;

namespace VSTSRestApiSamples.UnitTests.Client.APIs.Process
{
    [TestClass]
    public class ProcessessTest
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
        public void Processes_GetListOfProcesses_Success()
        {
            //arrange
            Processes request = new Processes(_auth);

            //act
            var result = request.GetListOfProcesses();

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Processes_GetProcesses_Success()
        {
            //arrange
            Processes request = new Processes(_auth);
            string processId = "adcc42ab-9882-485e-a3ed-7678f01f66bc";

            //act
            var result = request.GetProcess(processId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);

            request = null;
        }
    }
}
