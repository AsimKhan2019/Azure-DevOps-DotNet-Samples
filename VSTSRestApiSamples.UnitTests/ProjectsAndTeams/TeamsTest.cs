using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using VstsRestApiSamples.ProjectsAndTeams;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class TeamsTest
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
        public void ProjectsAndTeams_Teams_GetListOfTeams_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            ListofTeamsResponse.Teams response = request.GetListOfTeams(_configuration.Project);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("teams not found for project '" + _configuration.Project + "'");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Teams_GetTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            GetTeamResponse.Team response = request.GetTeam(_configuration.Project, _configuration.Team);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Teams_GetTeamMembers_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            GetTeamMembersResponse.Members response = request.GetTeamMembers(_configuration.Project, _configuration.Team);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Teams_CreateTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            GetTeamResponse.Team response = request.CreateTeam(_configuration.Project, "My Awesome Team");

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.Created, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void ProjectsAndTeams_Teams_UpdateTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            GetTeamResponse.Team response = request.UpdateTeam(_configuration.Project, "My Awesome Team");

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }
    }
}
