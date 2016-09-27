using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.Tests.WorkItemTracking
{
    [TestClass]
    public class AttachmentsTest
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
        public void WorkItemTracking_WorkItems_DownloadAttachment_Success()
        {
            // arrange
            string url = "";
            string saveTo = @"D:\Temp\";
            Attachments requestAttachments = new Attachments(_configuration);
            WorkItems requestWorkItems = new WorkItems(_configuration);

            // act
            var wiResponse = requestWorkItems.GetWorkItem(_configuration.WorkItemId);

            if (wiResponse.HttpStatusCode == HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("work item '" + _configuration.WorkItemId + "' not found");
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.OK, wiResponse.HttpStatusCode);

                foreach (GetWorkItemExpandAllResponse.Relation item in wiResponse.relations)
                {
                    if (item.rel == "AttachedFile")
                    {
                        saveTo = saveTo + item.attributes.name;
                        url = item.url;

                        var response = requestAttachments.DownloadAttachment(url, saveTo);

                        Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
                    }
                }
            }
        }
    }
}
