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

        [TestMethod, TestCategory("REST API")]
        public void GettingStarted_Authentication_PersonalAccessToken_Success()
        {
            //arrange
            Authentication request = new Authentication();

            //act
            var response = request.PersonalAccessToken(_configuration.UriString, _configuration.PersonalAccessToken);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }
    }
}
