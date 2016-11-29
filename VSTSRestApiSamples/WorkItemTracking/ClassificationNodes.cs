using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    /// <summary>
    /// otherwise known as area paths
    /// </summary>
    public class ClassificationNodes
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public ClassificationNodes(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public ListOfNodesResponse.Nodes GetAreas(string project)
        {
            ListOfNodesResponse.Nodes viewModel = new ListOfNodesResponse.Nodes();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/areas?$depth=2&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListOfNodesResponse.Nodes>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public ListOfNodesResponse.Nodes GetArea(string project, string path)
        {
            ListOfNodesResponse.Nodes viewModel = new ListOfNodesResponse.Nodes();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/areas/" + path + "?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListOfNodesResponse.Nodes>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }


        public GetNodeResponse.Node CreateArea(string project, string path)
        {
    CreateUpdateNodeViewModel.Node node = new CreateUpdateNodeViewModel.Node()
    {
        name = path               
    };

    GetNodeResponse.Node viewModel = new GetNodeResponse.Node();

    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

        // serialize the fields array into a json string          
        var postValue = new StringContent(JsonConvert.SerializeObject(node), Encoding.UTF8, "application/json");
        var method = new HttpMethod("POST");

        // send the request
        var request = new HttpRequestMessage(method, _configuration.UriString + project + "/_apis/wit/classificationNodes/areas?api-version=2.2") { Content = postValue };
        var response = client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode)
        {
            viewModel = response.Content.ReadAsAsync<GetNodeResponse.Node>().Result;
            viewModel.Message = "success";
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
        
        public ListOfNodesResponse.Nodes GetIterations(string project)
        {
            ListOfNodesResponse.Nodes viewModel = new ListOfNodesResponse.Nodes();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/iterations?$depth=2&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListOfNodesResponse.Nodes>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetNodeResponse.Node CreateIteration(string project, string path, DateTime startDate, DateTime finishDate)
        {
            CreateUpdateNodeViewModel.Node node = new CreateUpdateNodeViewModel.Node()
            {
                name = path,
                attributes = new CreateUpdateNodeViewModel.Attributes()
                {
                    startDate = startDate,
                    finishDate = finishDate
                }
            };

            GetNodeResponse.Node viewModel = new GetNodeResponse.Node();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var postValue = new StringContent(JsonConvert.SerializeObject(node), Encoding.UTF8, "application/json"); 
                var method = new HttpMethod("POST");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + project + "/_apis/wit/classificationNodes/iterations?api-version=2.2") { Content = postValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetNodeResponse.Node>().Result;
                    viewModel.Message = "success";
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

        public GetNodeResponse.Node UpdateIterationDates(string project, string path, DateTime startDate, DateTime finishDate)
        {
            CreateUpdateNodeViewModel.Node node = new CreateUpdateNodeViewModel.Node()
            {
                //name = path,
                attributes = new CreateUpdateNodeViewModel.Attributes()
                {
                    startDate = startDate,
                    finishDate = finishDate
                }
            };

            GetNodeResponse.Node viewModel = new GetNodeResponse.Node();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(node), Encoding.UTF8, "application/json"); 
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + project + "/_apis/wit/classificationNodes/iterations/" + path + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetNodeResponse.Node>().Result;
                    viewModel.Message = "success";
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
