using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.GettingStarted;
using System.Net;

namespace VstsRestApiSamples.Tests.GettingStarted
{
    [TestClass]
    public class AuthenticationTest
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

        [TestMethod, TestCategory("REST API - Authentication")]
        public void GettingStarted_InteractiveADAL_Success()
        {
            // arrange
            Authentication request = new Authentication();

            // act
            var response = request.InteractiveADAL(_configuration.AccountName, _configuration.ApplicationId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API - Authentication")]
        public void GettingStarted_Authentication_InteractiveADALExchangeGraphTokenForVSTSToken_Success()
        {
            // arrange
            Authentication request = new Authentication();

            // act
            var response = request.InteractiveADALExchangeGraphTokenForVSTSToken(_configuration.AccountName, _configuration.ApplicationId);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API - Authentication")]
        public void GettingStarted_Authentication_NonInteractivePersonalAccessToken_Success()
        {
            // arrange
            Authentication request = new Authentication();

            // act
            var response = request.NonInteractivePersonalAccessToken(_configuration.AccountName, _configuration.PersonalAccessToken);

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
    }
}
