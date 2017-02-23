using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.Work;

namespace VstsClientLibrariesSamples.Tests.Work
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

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Work_TeamSettings_GetTeamSettings_Success()
        {
            // arrange
            TeamSettings teamSettings = new TeamSettings(_configuration);

            // act
            var result = teamSettings.GetTeamSettings(_configuration.Project);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_Work_TeamSettings_UpdateTeamSettings_Success()
        {
            // arrange
            TeamSettings teamSettings = new TeamSettings(_configuration);

            // act
            var result = teamSettings.UpdateTeamSettings(_configuration.Project);

            //assert
            Assert.IsNotNull(result);
        }
    }
}
