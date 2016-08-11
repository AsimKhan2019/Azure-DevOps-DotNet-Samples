using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
            //encode our personal access token                   
            string credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            //create a viewmodel that is a class that represents the returned json response
            ListofProjectsResponse.Projects viewModel = new ListofProjectsResponse.Projects();

            //use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);  //url of our account (https://accountname.visualstudio.com)
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials); 

                //connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=1.0").Result;
          
                //check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    //set the viewmodel from the content in the response
                    viewModel = response.Content.ReadAsAsync<ListofProjectsResponse.Projects>().Result;
                    //var value = response.Content.ReadAsStringAsync().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }

        }        

    }
}


            

