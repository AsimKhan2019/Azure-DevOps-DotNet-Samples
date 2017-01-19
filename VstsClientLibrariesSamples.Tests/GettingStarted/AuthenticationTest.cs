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

        [TestMethod, TestCategory("Client Libraries - Authentication")]
        public void GettingStarted_Authentication_InteractiveADAL_Success()
        {
            // arrange
            Authentication authentication = new Authentication();

            // act
            var result = authentication.InteractiveADAL(_configuration.AccountName, _configuration.ApplicationId);

            // assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries - Authentication")]
        public void GettingStarted_Authentication_InteractiveADALExchangeGraphTokenForVSTSToken_Success()
        {
            // arrange
            Authentication authentication = new Authentication();

            // act
            var result = authentication.InteractiveADALExchangeGraphTokenForVSTSToken(_configuration.AccountName, _configuration.ApplicationId);

            // assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries - Authentication")]
        public void GettingStarted_Authentication_NonInteractivePersonalAccessToken_Success()
        {
            // arrange
            Authentication authentication = new Authentication();

            // act
            var result = authentication.NonInteractivePersonalAccessToken(_configuration.AccountName, _configuration.PersonalAccessToken);

            // assert
            Assert.IsNotNull(result);
        }


    }
}
