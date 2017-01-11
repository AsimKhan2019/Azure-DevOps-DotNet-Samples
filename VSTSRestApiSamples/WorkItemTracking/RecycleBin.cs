using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class RecycleBin
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public RecycleBin(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public GetItemsFromRecycleBinResponse.WorkItems GetDeletedItems(string project)
        {
            GetItemsFromRecycleBinResponse.WorkItems viewModel = new GetItemsFromRecycleBinResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/recyclebin?api-version=3.0-preview").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetItemsFromRecycleBinResponse.WorkItems>().Result;
                }               

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetItemFromRecycleBinResponse.WorkItem GetDeletedItem(string project, string id)
        {
            GetItemFromRecycleBinResponse.WorkItem viewModel = new GetItemFromRecycleBinResponse.WorkItem();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                              
                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/recyclebin/" + id + "?api-version=3.0-preview").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetItemFromRecycleBinResponse.WorkItem>().Result;
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

        public System.Net.HttpStatusCode RestoreWorkItem(string project, string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                var method = new HttpMethod("DELETE");
                var request = new HttpRequestMessage(method, _configuration.UriString + project + "/_apis/wit/recyclebin/" + id + "?restore=true&api-version=3.0-preview");
                var response = client.SendAsync(request).Result;              

                return response.StatusCode;
            }
        }
    }
}
