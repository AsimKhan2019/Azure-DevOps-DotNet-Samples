using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking.Queries;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class QueriesTest
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
        public void WorkItemTracking_Queries_GetListOfQueries_Success()
        {
            //arrange
            Queries request = new Queries(_configuration);

            //act
            ListofQueriesResponse.Queries response = request.GetListOfQueries(_configuration.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }        

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_Queries_GetListOfQueriesForFolder_Success()
        {
            //arrange
            Queries request = new Queries(_configuration);
            string folderPath = "Shared%20Queries/Product%20Planning";

            //act
            ListofQueriesByFolderPath.Queries response = request.GetListOfQueriesByFolderPath(_configuration.Project, folderPath);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_Queries_GetQueryById_Success()
        {
            //arrange
            Queries request = new Queries(_configuration);                    

            //act
            GetQueryResponse.Queries response = request.GetQueryById(_configuration.Project, _configuration.QueryId);

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
        public void WorkItemTracking_Queries_GetQueryByPath_Success()
        {
            //arrange
            Queries request = new Queries(_configuration);           

            //act
            GetQueryResponse.Queries response = request.GetQueryByPath(_configuration.Project, _configuration.Query);

            //assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("query '" + _configuration.Query + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

    }
}
