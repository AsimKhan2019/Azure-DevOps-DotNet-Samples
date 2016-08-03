using System;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.Work;

namespace VstsRestApiSamples.Work.ProcessConfiguration
{
    public class Lists
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Lists(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        /// <summary>
        /// create a picklist to the process
        /// </summary>
        /// <param name="processId">process id</param>
        /// <returns></returns>
        public PickListPostResponse.PickList CreatePickList(string processId)
        {
            PickListPostResponse.PickList viewModel = new PickListPostResponse.PickList();

            PickListPost.PickList data = new PickListPost.PickList();
            PickListPost.Item[] items = new PickListPost.Item[4];

            //create a bunch of values
            items[0] = new PickListPost.Item() { value = "Red" };
            items[1] = new PickListPost.Item() { value = "Blue" };
            items[2] = new PickListPost.Item() { value = "Yellow" };
            items[3] = new PickListPost.Item() { value = "Purple" };

            data.Name = "Sample Picklist";
            data.Type = "string";
            data.Items = items;
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    
                HttpResponseMessage response = client.PostAsJsonAsync("_apis/work/processdefinitions/" + processId + "/lists?api-version=3.0-preview", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<PickListPostResponse.PickList>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// update picklist values
        /// </summary>
        /// <param name="processId">process id</param>
        /// <param name="picklistId">picklist id</param>
        /// <returns>PickListPostResponse.PickList</returns>
        public PickListPostResponse.PickList UpdatePickList(string processId, string picklistId)
        {
            PickListPostResponse.PickList viewModel = new PickListPostResponse.PickList();

            PickListPost.PickList data = new PickListPost.PickList();
            PickListPost.Item[] items = new PickListPost.Item[5];

            //build picklist items and add a few new ones
            items[0] = new PickListPost.Item() { value = "Red" };
            items[1] = new PickListPost.Item() { value = "Blue" };
            items[2] = new PickListPost.Item() { value = "Yellow" };
            items[3] = new PickListPost.Item() { value = "Purple" };
            items[4] = new PickListPost.Item() { value = "Black" };

            //set post picklist object values
            data.Name = "Sample Picklist";  //name
            data.Type = "string";           //type
            data.Items = items;             //all the item values

            using (var client = new HttpClient())
            {           
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.PutAsJsonAsync("_apis/work/processdefinitions/" + processId + "/lists/" + picklistId + "?api-version=3.0-preview", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<PickListPostResponse.PickList>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get list of picklists we have in a process
        /// </summary>
        /// <param name="processId">process id</param>
        /// <returns>ListPickListResponse.PickList</returns>
        public ListPickListResponse.PickList GetListOfPickLists(string processId)
        {
            ListPickListResponse.PickList viewModel = new ListPickListResponse.PickList();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/work/processDefinitions/" + processId + "/lists?api-version=3.0-preview ").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListPickListResponse.PickList>().Result;               
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get a specific picklist
        /// </summary>
        /// <param name="processId">process id</param>
        /// <param name="picklistId">picklist id</param>
        /// <returns>PickListResponse.PickList</returns>

        public PickListResponse.PickList GetPickList(string processId, string picklistId)
        {
            PickListResponse.PickList viewModel = new PickListResponse.PickList();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/work/processDefinitions/" + processId + "/lists/" + picklistId + "?api-version=3.0-preview ").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<PickListResponse.PickList>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
