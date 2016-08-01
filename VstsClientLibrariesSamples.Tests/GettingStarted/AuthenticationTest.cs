using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.GettingStarted;

namespace VstsClientLibrariesSamples.Tests.GettingStarted
{
    [TestClass]
    public class AuthenticationTest
    {
        private IConfiguration _config = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            InitHelper.GetConfiguration(_config);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _config = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void GettingStarted_Authentication_PersonalAccessToken_Success()
        {
            //arrange
            Authentication authentication = new Authentication(_config);

            //act
            var result = authentication.PersonalAccessToken(_config.UriString, _config.PersonalAccessToken);

            //assert
            Assert.IsNotNull(result.Name);
        }
    }
}
