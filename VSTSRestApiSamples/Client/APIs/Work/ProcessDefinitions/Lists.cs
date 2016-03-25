using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Work;

namespace VstsRestApiSamples.Client.APIs.Work.ProcessDefinitions
{
    public class Lists
    {
        private string _account;
        private string _login;

        public Lists(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
        }

        public HttpStatusCode CreatePickList(string processId)
        {
            PickListPost.PickList data = new PickListPost.PickList();
            PickListPost.Item[] items = new PickListPost.Item[4];

            items[0] = new PickListPost.Item() { value = "Red" };
            items[1] = new PickListPost.Item() { value = "Blue" };
            items[2] = new PickListPost.Item() { value = "Yellow" };
            items[3] = new PickListPost.Item() { value = "Purple" };

            data.Name = "Sample Picklist";
            data.Type = "string";
            data.Items = items;
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);
                    
                HttpResponseMessage response = client.PostAsJsonAsync("_apis/work/processdefinitions/" + processId + "/lists?api-version=3.0-preview", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<PickListPostResponse.PickList>().Result;
                }

                return response.StatusCode;
            }
        }

        public HttpStatusCode UpdatePickList(string processId, string picklistId)
        {
            PickListPost.PickList data = new PickListPost.PickList();
            PickListPost.Item[] items = new PickListPost.Item[5];

            //build picklist itms
            items[0] = new PickListPost.Item() { value = "Red" };
            items[1] = new PickListPost.Item() { value = "Blue" };
            items[2] = new PickListPost.Item() { value = "Yellow" };
            items[3] = new PickListPost.Item() { value = "Purple" };
            items[4] = new PickListPost.Item() { value = "Black" };

            //set post picklist object values
            data.Name = "Sample Picklist";
            data.Type = "string";
            data.Items = items;

            using (var client = new HttpClient())
            {           
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.PutAsJsonAsync("_apis/work/processdefinitions/" + processId + "/lists/" + picklistId + "?api-version=3.0-preview", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<PickListPostResponse.PickList>().Result;
                }

                return response.StatusCode;
            }
        }

        public HttpStatusCode GetListOfPickLists(string processId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/work/processDefinitions/" + processId + "/lists?api-version=3.0-preview ").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<ListPickListResponse.PickList>().Result;               
                }              
               
                return response.StatusCode;
            }
        }

        public HttpStatusCode GetPickList(string processId, string picklistId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/work/processDefinitions/" + processId + "/lists/" + picklistId + "?api-version=3.0-preview ").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<PickListResponse.PickList>().Result;
                }

                return response.StatusCode;
            }
        }
    }
}
