using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.ProjectsAndTeams;

namespace VstsClientLibrariesSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class SamplesTest
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

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Samples_ProjectsAndTeams_GetTeams_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            try
            {
                var result = request.GetTeams();

                // assert
                Assert.IsNotNull(result);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("project '" + _configuration.Project + "' not found");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Samples_ProjectsAndTeams_Samples_GetTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            try
            {
                var result = request.GetTeam();

                // assert
                Assert.AreEqual("My new team", result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Samples_ProjectsAndTeams_GetTeamMembers_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            try
            {
                var result = request.GetTeamMembers();

                // assert
                Assert.IsNotNull(result);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("team '" + _configuration.Team + "' not found");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Samples_ProjectsAndTeams_CreateTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            try
            {
                var result = request.CreateTeam();

                // assert
                Assert.AreEqual("My new team", result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("'My new team' already exists");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void ProjectsAndTeams_Samples_UpdateTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            try
            {
                var result = request.UpdateTeam();

                // assert
                Assert.AreEqual("My new team", result.Name);
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("'My new team' does not exist");
            }
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Samples_ProjectsAndTeams_DeleteTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            try
            {
                request.DeleteTeam();

                var result = request.GetTeam();              

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
