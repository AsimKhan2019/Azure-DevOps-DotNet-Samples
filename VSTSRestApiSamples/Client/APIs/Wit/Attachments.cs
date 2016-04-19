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

                    Stream readStream = response.Content.ReadAsStreamAsync().Result;
                    FileStream writeStream = new FileStream(@saveToFile, FileMode.Create, FileAccess.ReadWrite);                                                           
                    bytesRead = readStream.Read(buffer, 0, length);

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

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}
