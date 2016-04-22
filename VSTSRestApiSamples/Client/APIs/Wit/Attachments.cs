using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Work;

namespace VstsRestApiSamples.Client.APIs.Wit
{
    public class Attachments
    {
        private string _account;
        private string _login;

        public Attachments(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
        }

        /// <summary>
        /// download an attachment from the work item
        /// </summary>
        /// <param name="url">url supplied from get work item</param>
        /// <param name="saveToFile">location you want to save the attachment to</param>
        /// <returns>DownloadAttachmentResponse</returns>
        public DownloadAttachmentResponse DownloadAttachment(string url, string saveToFile)
        {             
            DownloadAttachmentResponse viewModel = new DownloadAttachmentResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync(url + "?api-version=1.0").Result;
                viewModel.HttpStatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    int length = 256;
                    int bytesRead;
                    Byte[] buffer = new Byte[length];

                    //read to stream
                    Stream readStream = response.Content.ReadAsStreamAsync().Result;

                    //save the file to location
                    FileStream writeStream = new FileStream(@saveToFile, FileMode.Create, FileAccess.ReadWrite);                                                           
                    bytesRead = readStream.Read(buffer, 0, length);

                    //read data write stream
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
    }
}
