using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;

using VstsRestApiSamples.ProjectsAndTeams;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class ProcessessTest
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
        public void ProjectsAndTeams_Processes_GetListOfProcesses_Success()
        {
            // arrange
            Processes request = new Processes(_configuration);

            // act
            var response = request.GetProcesses();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Processes_GetProcesses_Success()
        {
            // arrange
            Processes request = new Processes(_configuration);

            // act
            var listResponse = request.GetProcesses();                // get list of processes
            IList<ListofProcessesResponse.Value> vm = listResponse.value;   // bind to list
            string processId = vm[0].id;                                    // get a process id so we can look that up

            var response = request.GetProcess(processId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);          

            request = null;
        }
    }
}
