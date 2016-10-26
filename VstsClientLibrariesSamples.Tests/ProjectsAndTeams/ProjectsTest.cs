using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using VstsClientLibrariesSamples.ProjectsAndTeams;
using Microsoft.TeamFoundation.Core.WebApi;

namespace VstsClientLibrariesSamples.Tests.ProjectsAndTeams
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

        [TestMethod, TestCategory("Client Libraries")]
        public void ProjectsAndTeams_Projects_GetProjectByName_Success()
        {
            // arrange
            Projects projects = new Projects(_configuration);

            // act
            try
            {
                TeamProjectReference result = projects.GetProject(_configuration.Project);

                // assert
                Assert.AreEqual(_configuration.Project, result.Name); 
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("project '" + _configuration.Project + "' not found");
            }           
        }
    }
}
