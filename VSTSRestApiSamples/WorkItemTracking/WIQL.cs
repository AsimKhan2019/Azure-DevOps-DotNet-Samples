using System;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class WIQL
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public WIQL(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        /// <summary>
        /// execute a query that already exists (see queries code to get query id)
        /// </summary>
        /// <param name="project">project name or id</param>
        /// <param name="id">query id</param>
        /// <returns>GetWIQLRunStoredQueryResponse.WIQLResult</returns>
        public GetWIQLRunStoredQueryResponse.WIQLResult RunStoredQuery(string project, string id)
        {
            GetWIQLRunStoredQueryResponse.WIQLResult viewModel = new GetWIQLRunStoredQueryResponse.WIQLResult();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/wiql/" + id + "?api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWIQLRunStoredQueryResponse.WIQLResult>().Result;                    
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
