using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;

using VstsRestApiSamples.ProjectsAndTeams;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class ProjectsTest
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
        public void ProjectsAndTeams_Projects_GetListOfProjects_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);

            // act
            var response = request.GetTeamProjects();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_GetListOfProjectsByState_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);

            // act
            var response = request.GetTeamProjectsByState();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_GetProject_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);

            // act
            var response = request.GetTeamProjectWithCapabilities(_configuration.Project);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_CreateProject_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);
            string projectName = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act
            var createResponse = request.CreateTeamProject(projectName);

            // assert
            Assert.AreEqual(HttpStatusCode.Accepted, createResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_CreateProjectWithOperation_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);
            string projectName = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act
            var createResponse = request.CreateTeamProject(projectName);
            var url = createResponse.url;
            var operationResponse = request.GetOperation(url);
                        
            // assert
            Assert.AreEqual(HttpStatusCode.Accepted, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, operationResponse.HttpStatusCode);
            
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_CreateAndRenameProject_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);
            string projectName = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act
            var createResponse = request.CreateTeamProject(projectName);

            //TODO: Instead of sleep, monitor the status
            System.Threading.Thread.Sleep(5000);

            var getResponse = request.GetTeamProjectWithCapabilities(projectName);
            var projectId = getResponse.id;
           
            var renameResponse = request.RenameTeamProject(projectId, "Art Vandelay Project");

            // assert
            Assert.AreEqual(HttpStatusCode.Accepted, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.Accepted, renameResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_CreatedAndChangeProjectDescription_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);
            string projectName = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act
            var createResponse = request.CreateTeamProject(projectName);

            //TODO: Instead of sleep, monitor the status
            System.Threading.Thread.Sleep(5000);

            var getResponse = request.GetTeamProjectWithCapabilities(projectName);
            var projectId = getResponse.id;

            var renameResponse = request.ChangeTeamProjectDescription(projectId, "New project description");

            // assert
            Assert.AreEqual(HttpStatusCode.Accepted, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.Accepted, renameResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Projects_CreatedAndDeleteProject_Success()
        {
            // arrange
            TeamProjects request = new TeamProjects(_configuration);
            string projectName = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act
            var createResponse = request.CreateTeamProject(projectName);

            //TODO: Instead of sleep, monitor the status ("online")
            System.Threading.Thread.Sleep(5000);

            var getResponse = request.GetTeamProjectWithCapabilities(projectName);
            var projectId = getResponse.id;

            var deleteResponse = request.DeleteTeamProject(projectId);

            // assert
            Assert.AreEqual(HttpStatusCode.Accepted, deleteResponse);          

            request = null;
        }
    }
}
