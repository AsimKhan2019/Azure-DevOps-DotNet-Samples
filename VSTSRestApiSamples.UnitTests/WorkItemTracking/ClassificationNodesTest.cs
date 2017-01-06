using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;
using System.Net;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class ClassificationNodesTest
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
        public void WorkItemTracking_Nodes_GetAreas_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);

            // act
            GetNodesResponse.Nodes response = request.GetAreas(_configuration.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
           
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_GetIterations_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);

            // act
            GetNodesResponse.Nodes response = request.GetIterations(_configuration.Project);

            //assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_GetArea_Success()
        {
            // arrange
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes request = new ClassificationNodes(_configuration);

            // act
            GetNodeResponse.Node createResponse = request.CreateArea(_configuration.Project, path);
            GetNodesResponse.Nodes getResponse = request.GetArea(_configuration.Project, path);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_GetIteration_Success()
        {
            // arrange
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes request = new ClassificationNodes(_configuration);

            // act
            GetNodeResponse.Node createResponse = request.CreateIteration(_configuration.Project, path);     
            GetNodesResponse.Nodes getResponse = request.GetIteration(_configuration.Project, path);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, createResponse.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, getResponse.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_CreateIteration_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);

            // act
            GetNodeResponse.Node response = request.CreateIteration(_configuration.Project, path);

            //assert
            if (response.Message.Contains("VS402371: Classification node name " + path))
            {
                Assert.Inconclusive("Iteration '" + path + "' already exists");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.Created, response.HttpStatusCode);
            }
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_CreateArea_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);

            // act
            GetNodeResponse.Node response = request.CreateArea(_configuration.Project, path);

            //assert
            if (response.Message.Contains("VS402371:"))
            {
                Assert.Inconclusive("Area path '" + path + "' already exists");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.Created, response.HttpStatusCode);
            }
            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_UpdateIterationDates_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            DateTime startDate = new DateTime(2016, 11, 29);
            DateTime finishDate = new DateTime(2016, 12, 17);
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);

            // act
            GetNodeResponse.Node responseCreate = request.CreateIteration(_configuration.Project, path);
            GetNodeResponse.Node responseUpdate = request.UpdateIterationDates(_configuration.Project, path, startDate, finishDate);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseCreate.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseUpdate.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_RenameArea_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            string newName = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 10) + "-Rename";

            // act
            GetNodeResponse.Node responseCreate = request.CreateArea(_configuration.Project, path);
            GetNodeResponse.Node responseUpdate = request.RenameArea(_configuration.Project, path, newName);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseCreate.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseUpdate.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_RenameIteration_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            string newName = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 10) + "-Rename";

            // act
            GetNodeResponse.Node responseCreate = request.CreateIteration(_configuration.Project, path);
            GetNodeResponse.Node responseUpdate = request.RenameIteration(_configuration.Project, path, newName);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseCreate.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseUpdate.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_MoveIteration_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string parentIteration = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-PARENT";
            string childIteration = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-child";
            
            // act
            GetNodeResponse.Node responseParent = request.CreateIteration(_configuration.Project, parentIteration);
            GetNodeResponse.Node responseChild = request.CreateIteration(_configuration.Project, childIteration);
            GetNodeResponse.Node responseMove = request.MoveIteration(_configuration.Project, parentIteration, responseChild.id);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseParent.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.Created, responseChild.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseMove.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_MoveArea_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string parent = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-PARENT";
            string child = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-child";

            // act
            GetNodeResponse.Node responseParent = request.CreateArea(_configuration.Project, parent);
            GetNodeResponse.Node responseChild = request.CreateArea(_configuration.Project, child);
            GetNodeResponse.Node responseMove = request.MoveArea(_configuration.Project, parent, responseChild.id);

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseParent.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.Created, responseChild.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, responseMove.HttpStatusCode);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_DeleteArea_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string masterArea = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-MASTER";
            string deleteArea = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-delete";

            // act
            GetNodeResponse.Node responseMaster = request.CreateArea(_configuration.Project, masterArea);
            GetNodeResponse.Node responseDelete = request.CreateArea(_configuration.Project, deleteArea);
            var responseMove = request.DeleteArea(_configuration.Project, deleteArea, responseMaster.id.ToString());

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseMaster.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.Created, responseDelete.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.NoContent, responseMove);

            request = null;
        }

        [TestMethod, TestCategory("REST API")]
        public void WorkItemTracking_Nodes_DeleteIteration_Success()
        {
            // arrange
            ClassificationNodes request = new ClassificationNodes(_configuration);
            string masterIteration = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-MASTER";
            string deleteIteration = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-delete";

            // act
            GetNodeResponse.Node responseMaster = request.CreateIteration(_configuration.Project, masterIteration);
            GetNodeResponse.Node responseDelete = request.CreateIteration(_configuration.Project, deleteIteration);
            var responseMove = request.DeleteIteration(_configuration.Project, deleteIteration, responseMaster.id.ToString());

            //assert
            Assert.AreEqual(HttpStatusCode.Created, responseMaster.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.Created, responseDelete.HttpStatusCode);
            Assert.AreEqual(HttpStatusCode.NoContent, responseMove);

            request = null;
        }
    }
}
