using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsClientLibrariesSamples.WorkItemTracking;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace VstsClientLibrariesSamples.Tests.WorkItemTracking
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

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_Attachements_UploadAttachmentTextFile_Success()
        {
            // arrange
            Attachments request = new Attachments(_configuration);
            string filePath = @"D:\temp\test.txt";

            if (!System.IO.File.Exists(filePath))
            {
                Assert.Inconclusive("file not found");
            }

            // act
            AttachmentReference attachmentReference = request.UploadAttachmentTextFile(filePath);

            // assert
            Assert.IsNotNull(attachmentReference);

            request = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_Attachements_UploadAttachmentBinaryFile_Success()
        {
            // arrange
            Attachments request = new Attachments(_configuration);
            string filePath = @"D:\temp\test.jpg";

            if (!System.IO.File.Exists(filePath))
            {
                Assert.Inconclusive("file not found");
            }

            // act
            AttachmentReference attachmentReference = request.UploadAttachmentBinaryFile(filePath);

            // assert
            Assert.IsNotNull(attachmentReference);

            request = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_Attachements_DownloadAttachmentTextFile_Success()
        {
            // arrange
            Attachments request = new Attachments(_configuration);
            string filePath = @"D:\temp\test.txt";

            if (! System.IO.File.Exists(filePath))
            {
                Assert.Inconclusive("file not found");
            }

            // act
            AttachmentReference attachmentReference = request.UploadAttachmentTextFile(filePath);
            request.DownloadAttachment(attachmentReference.Id, @"D:\temp\attachment.txt");

            // assert
            Assert.IsTrue(System.IO.File.Exists(@"D:\temp\attachment.txt"));

            request = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_WorkItemTracking_Attachements_DownloadAttachmentBinaryFile_Success()
        {
            // arrange
            Attachments request = new Attachments(_configuration);
            string filePath = @"D:\temp\test.jpg";

            if (!System.IO.File.Exists(filePath))
            {
                Assert.Inconclusive("file not found");
            }

            // act
            AttachmentReference attachmentReference = request.UploadAttachmentBinaryFile(filePath);
            request.DownloadAttachment(attachmentReference.Id, @"D:\temp\attachment.jpg");

            // assert
            Assert.IsTrue(System.IO.File.Exists(@"D:\temp\attachment.txt"));

            request = null;
        }
    }
}
