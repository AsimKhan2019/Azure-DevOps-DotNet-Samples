using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class WIQLTest
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
        public void WIQL_GetListOfWorkItemsByQueryId_Success()
        {
            //arrange
            WIQL request = new WIQL(_configuration);

            //act
            GetWorkItemsResponse.Results response = request.GetListOfWorkItems_ByQueryId(_configuration.Project, _configuration.QueryId);

            //assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("query '" + _configuration.QueryId + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WIQL_GetListOfWorkItemsByWiql_Success()
        {
            //arrange
            WIQL request = new WIQL(_configuration);

            //act
            GetWorkItemsResponse.Results response = request.GetListOfWorkItems_ByWiql(_configuration.Project);

            //assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("no query results found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

    }
}
