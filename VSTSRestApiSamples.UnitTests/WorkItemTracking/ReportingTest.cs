using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class ReportingTest
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
        public void WorkItemTracking_Reporting_GetBatchOfWorkItemLinksByProjectAndDate_Success()
        {
            // arrange
            Reporting request = new Reporting(_configuration);

            // act
            BatchOfWorkItemLinksResponse.WorkItemLinks response = request.GetBatchOfWorkItemLinks(_configuration.Project, new DateTime(2016, 3, 15));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;            
        }
        
        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_Reporting_GetBatchOfWorkItemLinksForAll_Success()
        {
            // arrange
            Reporting request = new Reporting(_configuration);

            // act
            BatchOfWorkItemLinksResponse.WorkItemLinks response = request.GetBatchOfWorkItemLinksAll();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);          
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_Reporting_GetBatchOfWorkItemRevisions_ByProjectAndDate_Success()
        {
            // arrange
            Reporting request = new Reporting(_configuration);

            // act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions response = request.GetBatchOfWorkItemRevisionsByDate(_configuration.Project, new DateTime(2016, 4, 17));

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
            response = null;   
        }

        [TestMethod, TestCategory("REST API")]  
        public void WorkItemTracking_Reporting_GetBatchOfWorkItemRevisions_ForAll_Success()
        {
            // arrange
            Reporting request = new Reporting(_configuration);

            // act
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions response = request.GetBatchOfWorkItemRevisionsAll();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);

            request = null;
            response = null;
        }        
    }
}
