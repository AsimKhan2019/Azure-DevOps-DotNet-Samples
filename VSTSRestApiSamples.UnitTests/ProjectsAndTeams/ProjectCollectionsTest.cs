using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;

using VstsRestApiSamples.ProjectsAndTeams;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class ProjectCollectionsTest
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
        public void ProjectsAndTeams_ProjectCollections_GetProcess_Success()
        {
            // arrange
            ProjectCollections request = new ProjectCollections(_configuration);

            // act
            var response = request.GetProjectCollection(_configuration.CollectionId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }     
    }
}
