using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.Client.APIs.Wit;
using VstsRestApiSamples.ViewModels.Wit;
using System.Net;

namespace vstsrestapisamples.tests.Client.APIs.Wit
{
    [TestClass]
    public class AttachmentsTest
    {
        private IAuth _auth;

        [TestInitialize]
        public void TestInitialize()
        {
            _auth = new VstsRestApiSamples.Tests.Client.Helpers.Auth();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _auth = null;
        }

        [TestMethod]
        public void Wit_WorkItems_DownloadAttachment_Success()
        {
            //arrange
            string url = "";
            string saveTo = @"D:\Temp\";
            Attachments requestAttachments = new Attachments(_auth);
            WorkItems requestWorkItems = new WorkItems(_auth);

            //act
            var workItems = requestWorkItems.GetWorkItem("2583");

            Assert.AreEqual(HttpStatusCode.OK, workItems.HttpStatusCode);

            foreach (GetWorkItemExpandAllResponse.Relation item in workItems.relations)
            {
                if (item.rel == "AttachedFile")
                {
                    saveTo = saveTo + item.attributes.name;
                    url = item.url;

                    var result = requestAttachments.DownloadAttachment(url, saveTo);

                    Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
                }
            }   
        }
    }
}
