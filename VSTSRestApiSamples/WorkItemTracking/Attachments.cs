using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.Work;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class Attachments
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Attachments(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        // / <summary>
        // / download an attachment from the work item
        // / </summary>
        // / <param name="url">url supplied from get work item</param>
        // / <param name="saveToFile">location you want to save the attachment to</param>
        // / <returns>DownloadAttachmentResponse</returns>
        public DownloadAttachmentResponse DownloadAttachment(string url, string saveToFile)
        {             
            DownloadAttachmentResponse viewModel = new DownloadAttachmentResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(url + "?api-version=2.2").Result;
                viewModel.HttpStatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    int length = 256;
                    int bytesRead;
                    Byte[] buffer = new Byte[length];

                    // read to stream
                    Stream readStream = response.Content.ReadAsStreamAsync().Result;

                    // save the file to location
                    FileStream writeStream = new FileStream(@saveToFile, FileMode.Create, FileAccess.ReadWrite);                                                           
                    bytesRead = readStream.Read(buffer, 0, length);

                    // read data write stream
                    while (bytesRead > 0)
                    {
                        writeStream.Write(buffer, 0, bytesRead);
                        bytesRead = readStream.Read(buffer, 0, length);
                    }

                    readStream.Close();
                    writeStream.Close();

                    viewModel.file = saveToFile;
                }               

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / upload binary file into attachement store
        // / </summary>
        // / <param name="filePath">local file path for file</param>
        // / <returns>UploadAttachmentResponse.Attachment</returns>
        public ViewModels.WorkItemTracking.AttachmentReference UploadAttachment(string filePath)
        {
            Byte[] bytes = File.ReadAllBytes(@filePath);
            String[] breakApart = filePath.Split('\\');
            int length = breakApart.Length;
            string fileName = breakApart[length - 1];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                ByteArrayContent content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                HttpResponseMessage response = client.PostAsync("_apis/wit/attachments?fileName=" + fileName + "&api-version=2.2", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<ViewModels.WorkItemTracking.AttachmentReference>().Result;
                    return result;
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    var error = msg.ToString();

                    return null;
                }
            }
        }

    }
}
