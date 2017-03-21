using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.WorkItemTracking;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class RecyleBinTest
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
        public void CL_WorkItemTracking_RecycleBin_GetDeletedItems_Success()
        {
            // arrange
            RecycleBin recycleBin = new RecycleBin(_configuration);
            WorkItems workItems = new WorkItems(_configuration);
            WorkItem item = null;
            int[] ids = new int[2];

            // act
            ////create workitems, delete them, get from bin
            item = workItems.CreateWorkItem(_configuration.Project);           
            workItems.DeleteWorkItem(Convert.ToInt32(item.Id));
            ids[0] = Convert.ToInt32(item.Id);

            item = workItems.CreateWorkItem(_configuration.Project);
            workItems.DeleteWorkItem(Convert.ToInt32(item.Id));
            ids[1] = Convert.ToInt32(item.Id);

            var list = recycleBin.GetDeletedItems(_configuration.Project);
            
            //assert
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count >= 2);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_RecycleBin_GetDeletedItem_Success()
        {
            // arrange
            RecycleBin recycleBin = new RecycleBin(_configuration);
            WorkItems workItems = new WorkItems(_configuration);
           
            // act
            ////create workitem, delete them, get from bin by id
            var item = workItems.CreateWorkItem(_configuration.Project);
            workItems.DeleteWorkItem(Convert.ToInt32(item.Id));                    

            var result = recycleBin.GetDeletedItem(Convert.ToInt32(item.Id));

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, item.Id);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_RecycleBin_RestoreItem_Success()
        {
            // arrange
            RecycleBin recycleBin = new RecycleBin(_configuration);
            WorkItems workItems = new WorkItems(_configuration);

            // act
            ////create workitem, delete it, restore it, get it
            var item = workItems.CreateWorkItem(_configuration.Project);
            workItems.DeleteWorkItem(Convert.ToInt32(item.Id));

            var restoreResult = recycleBin.RestoreItem(Convert.ToInt32(item.Id));
            var getResult = workItems.GetWorkItem(Convert.ToInt32(item.Id));

            //assert
            Assert.IsNotNull(getResult);
            Assert.AreEqual(getResult.Id, item.Id);
        }
             
        [TestMethod, TestCategory("Client Libraries")]        
        public void CL_WorkItemTracking_RecycleBin_PermenentlyDeleteItem_Success()
        {
            // arrange
            RecycleBin recycleBin = new RecycleBin(_configuration);
            WorkItems workItems = new WorkItems(_configuration);

            // act
            ////create workitem, delete it, perm deleted it, try and get it
            var item = workItems.CreateWorkItem(_configuration.Project);
            workItems.DeleteWorkItem(Convert.ToInt32(item.Id));

            recycleBin.PermenentlyDeleteItem(Convert.ToInt32(item.Id));                              
        }
    }
}
