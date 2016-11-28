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
              
        public GetNodeResponse.Node UpdateIterationDates(string project, string path, DateTime startDate, DateTime finishDate)
        {
            GetNodeResponse.Attributes attr = new GetNodeResponse.Attributes() { startDate = startDate, finishDate = finishDate };
            GetNodeResponse.Node viewModel = new GetNodeResponse.Node();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent("{ \"attributes\": { \"startDate\": \"2015-01-26T00:00:00Z\", \"finishDate\": \"2015-01-30T00:00:00Z\" } }", Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + project + "/_apis/wit/classificationNodes/iterations/Iteration%20Foo?api-version=1.0") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetNodeResponse.Node>().Result;
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
