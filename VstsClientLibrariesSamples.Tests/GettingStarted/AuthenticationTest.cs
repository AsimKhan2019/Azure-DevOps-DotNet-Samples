using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.GettingStarted;

namespace VstsClientLibrariesSamples.Tests.GettingStarted
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

        [TestMethod, TestCategory("Client Libraries")]
        public void GettingStarted_Authentication_PersonalAccessToken_Success()
        {
            // arrange
            Authentication authentication = new Authentication(_configuration);

            // act
            var result = authentication.PersonalAccessToken(_configuration.UriString, _configuration.PersonalAccessToken);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
