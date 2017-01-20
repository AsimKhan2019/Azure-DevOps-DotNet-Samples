using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.ViewModels;
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

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetRestoredWorkItemResponse.WorkItem RestoreItem(string id)
        {
            GetRestoredWorkItemResponse.WorkItem viewModel = new GetRestoredWorkItemResponse.WorkItem();

            var patchDocument = new {
                IsDeleted = false
            };
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json"); 

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/recyclebin/" + id + "?api-version=3.0-preview") { Content = patchValue };
                var response = client.SendAsync(request).Result;  
                
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetRestoredWorkItemResponse.WorkItem>().Result;
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

        public GetRestoreMultipleWorkItemsResponse.Items RestoreMultipleItems(string[] ids)
        {
            GetRestoreMultipleWorkItemsResponse.Items viewModel = new GetRestoreMultipleWorkItemsResponse.Items();
            WorkItemBatchPost.BatchRequest[] postDocument = new WorkItemBatchPost.BatchRequest[3];
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "Content-Type", "application/json-patch+json" }
            };

            Object[] postBody = new Object[1];
            postBody[0] = new { op = "replace", path = "/IsDeleted", value = "false" };
            var i = 0;

            foreach(var id in ids)
            {
                postDocument[i] = new WorkItemBatchPost.BatchRequest
                {
                    method = "PATCH",
                    uri = "/_apis/wit/recyclebin/" + id + "?api-version=3.0-preview",
                    headers = headers,
                    body = postBody
                };

                i = i + 1;
            };                     

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                var postValue = new StringContent(JsonConvert.SerializeObject(postDocument), Encoding.UTF8, "application/json");

                var method = new HttpMethod("POST");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/$batch?api-version=3.0-preview") { Content = postValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetRestoreMultipleWorkItemsResponse.Items>().Result;
                }               

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public HttpStatusCode PermenentlyDeleteItem(string id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                              
                var method = new HttpMethod("DELETE");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/recyclebin/" + id + "?api-version=3.0-preview");
                var response = client.SendAsync(request).Result;

                if (! response.IsSuccessStatusCode)
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;                 
                }

                return response.StatusCode;
            }
        }

        public GetRestoreMultipleWorkItemsResponse.Items PeremenentlyDeleteMultipleItems(string[] ids)
        {
            GetRestoreMultipleWorkItemsResponse.Items viewModel = new GetRestoreMultipleWorkItemsResponse.Items();
            WorkItemBatchPost.BatchRequest[] postDocument = new WorkItemBatchPost.BatchRequest[3];
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "Content-Type", "application/json-patch+json" }
            };

            var i = 0;           

            foreach (var id in ids)
            {
                postDocument[i] = new WorkItemBatchPost.BatchRequest
                {
                    method = "DELETE",
                    uri = "/_apis/wit/recyclebin/" + id + "?api-version=3.0-preview",
                    headers = headers                  
                };

                i = i + 1;
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                var postValue = new StringContent(JsonConvert.SerializeObject(postDocument), Encoding.UTF8, "application/json");

                var method = new HttpMethod("POST");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/$batch?api-version=3.0-preview") { Content = postValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetRestoreMultipleWorkItemsResponse.Items>().Result;
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
