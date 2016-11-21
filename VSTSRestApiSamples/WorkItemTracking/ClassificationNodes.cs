using System;
using System.Net.Http;
using System.Net.Http.Headers;

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

        public void GetListOfAreaPaths(string project)
        {
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/areas?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    var viewModel = response.Content.ReadAsStringAsync().Result;
                }

                //viewModel.HttpStatusCode = response.StatusCode;

                //return viewModel;
            }
        }
    }
}
