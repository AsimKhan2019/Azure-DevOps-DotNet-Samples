using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.GettingStarted
{
    public class Authentication
    {       

        public Authentication()
        {
                     
        }

        public ListofProjectsResponse.Projects PersonalAccessToken(string url, string personalAccessToken)
        {           
            string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            ListofProjectsResponse.Projects viewModel = new ListofProjectsResponse.Projects();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=1.0").Result;
          
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofProjectsResponse.Projects>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }

        }        

    }
}
