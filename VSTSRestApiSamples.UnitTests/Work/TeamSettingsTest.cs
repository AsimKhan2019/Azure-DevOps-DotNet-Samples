using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Work;
using VstsRestApiSamples.ViewModels.Work;
using System.Net;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.Work
{
    [TestClass]
    public class TeamSettingsTest
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
        public void Work_TeamSettings_GetTeamSettings_Success()
        {
            // arrange
            TeamSettings request = new TeamSettings(_configuration);

            // act
            GetTeamSettingsResponse.Settings response = request.GetTeamSettings(_configuration.Project, _configuration.Team);

            // assert           
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void Work_TeamSettings_UpdateTeamSettings_Success()
        {
            // arrange
            TeamSettings request = new TeamSettings(_configuration);   

            // act
            GetTeamSettingsResponse.Settings settingsResponse = request.UpdateTeamSettings(_configuration.Project);

            // assert           
            Assert.AreEqual(HttpStatusCode.OK, settingsResponse.HttpStatusCode);

            request = null;
        }
    }
}
