using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VstsClientLibrariesSamples.ProjectsAndTeams;

namespace VstsClientLibrariesSamples.Tests.ProjectsAndTeams
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

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_ProjectCollections_GetProjectCollections_Success()
        {
            // arrange
            ProjectCollections projectCollections = new ProjectCollections(_configuration);

            // act            
            IEnumerable<TeamProjectCollectionReference> results = projectCollections.GetProjectCollections();

            // assert
            Assert.IsNotNull(results);                    
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_ProjectCollections_GetProjectCollection_Success()
        {
            // arrange
            ProjectCollections projectCollections = new ProjectCollections(_configuration);

            // act
            try
            {
                TeamProjectCollectionReference result = projectCollections.GetProjectCollection(_configuration.CollectionId);

                // assert
                Assert.AreEqual(result.Id, new System.Guid(_configuration.CollectionId));
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("project collection'" + _configuration.Project + "' not found");
            }
        }
    }
}
