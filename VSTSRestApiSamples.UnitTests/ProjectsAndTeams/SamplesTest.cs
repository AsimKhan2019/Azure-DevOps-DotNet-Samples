using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.ProjectsAndTeams;

namespace VstsRestApiSamples.Tests.ProjectsAndTeams
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

        [TestMethod, TestCategory("REST API")]
        public void Samples_Teams_GetTeams_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            string response = request.GetTeams();

            // assert
            if (response == "not found")
            {
                Assert.Inconclusive("project not found");
            }
            else
            {
                Assert.AreEqual("success", response);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void Samples_Teams_GetTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            string response = request.GetTeam();

            // assert
            if (response == "not found")
            {
                Assert.Inconclusive("team not found");
            }
            else
            {
                Assert.AreEqual("success", response);
            }

            request = null;
        }


        [TestMethod, TestCategory("REST API")]
        public void Samples_Teams_GetTeamMembers_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            string response = request.GetTeamMembers();

            // assert
            if (response == "not found")
            {
                Assert.Inconclusive("team not found");
            }
            else
            {
                Assert.AreEqual("success", response);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void Samples_Teams_CreateTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            string response = request.CreateTeam();

            // assert
            Assert.AreEqual("success", response);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void Samples_Teams_UpdateTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            string response = request.UpdateTeam();

            // assert
            if (response == "not found")
            {
                Assert.Inconclusive("team not found for update");
            }
            else
            {
                Assert.AreEqual("success", response);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void Samples_Teams_DeleteTeam_Success()
        {
            // arrange
            Samples request = new Samples(_configuration);

            // act
            string response = request.DeleteTeam();

            // assert
            Assert.AreEqual("success", response);

            request = null;
        }
    }
}
