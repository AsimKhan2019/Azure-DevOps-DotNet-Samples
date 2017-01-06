using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using VstsClientLibrariesSamples.WorkItemTracking;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
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
                
        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_GetAreas_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.GetAreas(_configuration.Project, 100);

            Assert.AreEqual("success", result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_GetIterations_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.GetIterations(_configuration.Project, 100);

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_GetArea_Success()
        {
            // arrange
            string name = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createResult = nodes.CreateArea(_configuration.Project, name);
            var getResult = nodes.GetArea(_configuration.Project, name);

            //assert           
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(getResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_GetIteration_Success()
        {
            // arrange
            string name = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createResult = nodes.CreateIteration(_configuration.Project, name);
            var getResult = nodes.GetIteration(_configuration.Project, name);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(getResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_CreateArea_Success()
        {
            // arrange
            string name = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var result = nodes.CreateArea(_configuration.Project, name);

            // assert
            Assert.IsNotNull(result);
        }
        
        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_CreateIteration_Success()
        {
            // arrange
            ClassificationNodes nodes = new ClassificationNodes(_configuration);
            string name = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);

            // act
            var result = nodes.CreateIteration(_configuration.Project, name);

            // assert
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_RenameIteration_Success()
        {
            // arrange
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createResult = nodes.CreateIteration(_configuration.Project, path);
            var renameResult = nodes.RenameIteration(_configuration.Project, path, path + "-rename");

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(renameResult);
        }
        
        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_RenameArea_Success()
        {
            // arrange
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createResult = nodes.CreateArea(_configuration.Project, path);
            var renameResult = nodes.RenameArea(_configuration.Project, path, path + "-rename");

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(renameResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void WorkItemTracking_ClassificationNodes_UpdateIterationDates_Success()
        {
            // arrange
            DateTime startDate = new DateTime(2016,12,28);
            DateTime finishDate = new DateTime(2017,1,7);
            string path = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15);

            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act   
            var createResult = nodes.CreateIteration(_configuration.Project, path);
            var updateResult = nodes.UpdateIterationDates(_configuration.Project, path, startDate, finishDate);

            //assert
            Assert.IsNotNull(createResult);
            Assert.IsNotNull(updateResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        [Ignore]
        public void CL_WorkItemTracking_ClassificationNodes_MoveIteration_Success()
        {
            // arrange
            string pathParent = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-Parent";
            string pathChild = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-Child";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createParentResult = nodes.CreateIteration(_configuration.Project, pathParent);
            var createChildResult = nodes.CreateIteration(_configuration.Project, pathChild);
            var moveResult = nodes.MoveIteration(_configuration.Project, pathParent, createChildResult.Id);

            //assert
            Assert.IsNotNull(createParentResult);
            Assert.IsNotNull(createChildResult);
            Assert.IsNotNull(moveResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        [Ignore]
        public void CL_WorkItemTracking_ClassificationNodes_MoveArea_Success()
        {
            // arrange
            string pathParent = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-Parent";
            string pathChild = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-Child";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createParentResult = nodes.CreateArea(_configuration.Project, pathParent);
            var createChildResult = nodes.CreateArea(_configuration.Project, pathChild);
            var moveResult = nodes.MoveArea(_configuration.Project, pathParent, createChildResult.Id);

            //assert
            Assert.IsNotNull(createParentResult);
            Assert.IsNotNull(createChildResult);
            Assert.IsNotNull(moveResult);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_DeleteIteration_Success()
        {
            // arrange
            string pathDelete = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-delete";
            string pathMaster = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-master";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createDelete = nodes.CreateIteration(_configuration.Project, pathDelete);
            var createMaster = nodes.CreateIteration(_configuration.Project, pathMaster);

            nodes.DeleteIteration(_configuration.Project, pathDelete, createMaster.Id);
                       
            //assert
            Assert.IsNotNull(createDelete);
            Assert.IsNotNull(createMaster);            
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_ClassificationNodes_DeleteArea_Success()
        {
            // arrange
            string pathDelete = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-delete";
            string pathMaster = System.Guid.NewGuid().ToString().ToUpper().Substring(0, 15) + "-master";
            ClassificationNodes nodes = new ClassificationNodes(_configuration);

            // act
            var createDelete = nodes.CreateArea(_configuration.Project, pathDelete);
            var createMaster = nodes.CreateArea(_configuration.Project, pathMaster);

            nodes.DeleteArea(_configuration.Project, pathDelete, createMaster.Id);

            //assert
            Assert.IsNotNull(createDelete);
            Assert.IsNotNull(createMaster);
        }
    }
}
