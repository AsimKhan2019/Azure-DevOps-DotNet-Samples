using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.ProjectsAndTeams
{
    public class ProjectCollections
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public ProjectCollections(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }
        
        public GetProjectCollectionResponse.ProjectCollection GetProjectCollection(string id)
        {
            GetProjectCollectionResponse.ProjectCollection viewModel = new GetProjectCollectionResponse.ProjectCollection();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/projectCollections/" + id + "?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetProjectCollectionResponse.ProjectCollection>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
