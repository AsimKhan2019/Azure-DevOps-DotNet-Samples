using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using VstsRestApiSamples.Work;

namespace VstsRestApiSamples.Tests.Work
{
    [TestClass]
    public class ListTests
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
        public void Work_ProcessConfiguration_Lists_CreatePickList_Success()
        {
            // arrange
            Lists request = new Lists(_configuration);

            // act
            var response = request.CreatePickList(_configuration.ProcessId);
            
            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("process not found for given processid");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void ProcessDefinitions_Work_Lists_UpdatePickList_Success()
        {
            // arrange
            Lists request = new Lists(_configuration);

            // act
            var response = request.UpdatePickList(_configuration.ProcessId, _configuration.PickListId);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("picklist or process not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }            

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void ProcessDefinitions_Work_Lists_GetListOfPickLists_Success()
        {
            // arrange
            Lists request = new Lists(_configuration);

            // act
            var response = request.GetListOfPickLists(_configuration.ProcessId);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("process not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }

        [TestMethod, TestCategory("REST API")]  
        public void Work_ProcessCustomization_Lists_GetPickList_Success()
        {
            // arrange
            Lists request = new Lists(_configuration);

            // act
            var response = request.GetPickList(_configuration.ProcessId, _configuration.PickListId);

            // assert
            if (response.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("picklist or process not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            }

            request = null;
        }
    }
}
