using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples;
using VstsClientLibrariesSamples.ProjectsAndTeams;
using Microsoft.TeamFoundation.Core.WebApi;

namespace VstsClientLibrariesTeams.Tests.ProjectsAndTeams
{
    [TestClass]
    public class TeamsTest
    {
        private IConfiguration _configuration = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            VstsClientLibrariesSamples.Tests.InitHelper.GetConfiguration(_configuration);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _configuration = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Teams_GetTeams_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            try
            {
                var result = request.GetTeams(_configuration.Project);

                // assert
                Assert.IsNotNull(result);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("project '" + _configuration.Project + "' not found");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Teams_GetTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            try
            {
                var result = request.GetTeam(_configuration.Project, _configuration.Team);

                // assert
                Assert.AreEqual(_configuration.Team, result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Teams_GetTeamMembers_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            try
            {
                var result = request.GetTeamMembers(_configuration.Project, _configuration.Team);

                // assert
                Assert.IsNotNull(result);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Teams_CreateTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            WebApiTeam teamData = new WebApiTeam() { Name = "My new team" };

            // act
            try
            {
                var result = request.CreateTeam(_configuration.Project, teamData);

                // assert
                Assert.AreEqual("My new team", result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("'My new team' already exists");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Teams_UpdateTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            WebApiTeam teamData = new WebApiTeam() { Name = "My new team", Description = "my awesome team description" };

            // act
            try
            {
                var result = request.UpdateTeam(_configuration.Project, "My new team", teamData);

                // assert
                Assert.AreEqual("My new team", result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("'My new team' does not exist");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Teams_DeleteTeam_Success()
        {
            // arrange
            Teams request = new Teams(_configuration);

            // act
            try
            {
                request.DeleteTeam(_configuration.Project, "My new team");

                var result = request.GetTeam(_configuration.Project, "My new team");

                // assert
                Assert.AreEqual("My new team", result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("'My new team' does not exist");
            }
        }
    }
}
