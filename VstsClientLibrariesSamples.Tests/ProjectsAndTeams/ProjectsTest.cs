using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VstsClientLibrariesSamples.ProjectsAndTeams;
using Microsoft.TeamFoundation.Core.WebApi;

namespace VstsClientLibrariesSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class ProjectsTest
    {
        private IConfiguration _config = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            InitHelper.GetConfiguration(_config);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _config = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ProjectsAndTeams_GetProjectByName_Success()
        {
            //arrange
            Projects projects = new Projects(_config);

            //act
            try
            {
                TeamProjectReference result = projects.GetProjectByName(_config.Project);

                //assert
                Assert.AreEqual(_config.Project, result.Name); 
            }
            catch (System.AggregateException ex)
            {
                Assert.Inconclusive("project '" + _config.Project + "' not found");
            }           
        }
    }
}
