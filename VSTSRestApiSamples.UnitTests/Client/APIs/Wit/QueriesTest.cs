using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using VstsRestApiSamples.ViewModels.Wit.Queries;
using VstsRestApiSamples.ViewModels.Wit;

namespace VSTSRestApiSamples.UnitTests.Client.APIs.Wit
{
    [TestClass]
    public class QueriesTest
    {
        private IAuth _auth;

        [TestInitialize]
        public void TestInitialize()
        {
            _auth = new VstsRestApiSamples.Tests.Client.Helpers.Auth();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _auth = null;
        }

        [TestMethod]
        public void Wit_Queries_GetListOfQueries_Success()
        {
            //arrange
            Queries request = new Queries(_auth);

            //act
            ListofQueriesResponse.Queries response = request.GetListOfQueries(_auth.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_Queries_GetListOfQueriesForFolder_Success()
        {
            //arrange
            Queries request = new Queries(_auth);
            string folderPath = "Shared%20Queries/Product%20Planning";

            //act
            ListofQueriesByFolderPath.Queries response = request.GetListOfQueriesByFolderPath(_auth.Project, folderPath);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod]
        public void Wit_Queries_GetQueryById_Success()
        {
            //arrange
            Queries request = new Queries(_auth);                    

            //act
            GetQueryByIdResponse.Queries response = request.GetQueryById(_auth.Project, _auth.QueryId);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }       
    }
}
