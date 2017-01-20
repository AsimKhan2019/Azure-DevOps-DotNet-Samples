using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Work;
using VstsRestApiSamples.ViewModels.Work;
using System.Net;

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
        public void WorkItemTracking_WorkItems_GetWorkItemsByIDs_Success()
        {
            // arrange
            TeamSettings request = new TeamSettings(_configuration);

            // act
            GetTeamSettingsResponse.Settings response = request.GetTeamsSettings(_configuration.Project, _configuration.Team);

            // assert           
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
    }
}
