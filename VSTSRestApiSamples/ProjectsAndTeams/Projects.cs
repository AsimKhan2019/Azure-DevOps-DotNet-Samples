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
    public class Projects
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Projects(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public ListofProjectsResponse.Projects ListOfProjects()
        {           
            // create a viewmodel that is a class that represents the returned json response
            ListofProjectsResponse.Projects viewModel = new ListofProjectsResponse.Projects();

            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    // set the viewmodel from the content in the response
                    viewModel = response.Content.ReadAsAsync<ListofProjectsResponse.Projects>().Result;                   
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }

        }
    }
}
