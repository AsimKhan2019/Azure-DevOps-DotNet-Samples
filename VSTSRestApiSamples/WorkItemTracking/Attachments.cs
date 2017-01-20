using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        public ViewModels.WorkItemTracking.AttachmentReference UploadAttachmentTextFile(string filePath)
        {
            string text = File.ReadAllText(@filePath);
            String[] breakApart = filePath.Split('\\');
            int length = breakApart.Length;
            string fileName = breakApart[length - 1];

            ViewModels.WorkItemTracking.AttachmentReference viewModel = new ViewModels.WorkItemTracking.AttachmentReference();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string         
                var patchValue = new StringContent(text, Encoding.UTF8, "application/json"); // mediaType needs to be application/json-patch+json for a patch call
                var method = new HttpMethod("POST");

                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/attachments?fileName=" + fileName + "&api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ViewModels.WorkItemTracking.AttachmentReference>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public ViewModels.WorkItemTracking.AttachmentReference UploadAttachmentBinaryFile(string filePath)
        {
            Byte[] bytes = File.ReadAllBytes(@filePath);
            String[] breakApart = filePath.Split('\\');
            int length = breakApart.Length;
            string fileName = breakApart[length - 1];

            ViewModels.WorkItemTracking.AttachmentReference viewModel = new ViewModels.WorkItemTracking.AttachmentReference();

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
                    viewModel = response.Content.ReadAsAsync<ViewModels.WorkItemTracking.AttachmentReference>().Result;
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;                   
                    viewModel.Message = msg.ToString();
                }

                viewModel.HttpStatusCode = response.StatusCode;
                return viewModel;
            }
        }

    }
}
