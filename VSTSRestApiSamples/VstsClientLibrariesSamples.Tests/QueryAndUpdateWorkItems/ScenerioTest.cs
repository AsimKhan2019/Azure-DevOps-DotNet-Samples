using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.QueryAndUpdateWorkItems;

namespace VstsClientLibrariesSamples.Tests.QueryAndUpdateWorkItems
{
    [TestClass]
    public class ScenerioTest
    {
        private IConfiguration _config;

        [TestInitialize]
        public void TestInitialize()
        {
            _config = new Configuration();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _config = null;
        }

        [TestMethod]
        public void QueryAndUpdateWorkItems_ExecuteFullScenerio_Success()
        {
            //arrange
            Scenerio scenerio = new Scenerio(_config);

            //act
            var result = scenerio.ExecuteFullScenerio();

            //assert
            Assert.AreEqual(result, true);
        }
    }
}
