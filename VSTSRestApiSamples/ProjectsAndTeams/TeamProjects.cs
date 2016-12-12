using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.ProjectsAndTeams
{
    public class TeamProjects
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public TeamProjects(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public ListofProjectsResponse.Projects GetTeamProjects()
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
                HttpResponseMessage response = client.GetAsync("_apis/projects?api-version=2.2").Result;

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

        public ListofProjectsResponse.Projects GetTeamProjectsByState()
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
        
        public GetProjectResponse.Project GetTeamProjectWithCapabilities(string name)
        {
            // create a viewmodel that is a class that represents the returned json response
            GetProjectResponse.Project viewModel = new GetProjectResponse.Project();

            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects/" + name + "?includeCapabilities=true&api-version=2.2").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    // set the viewmodel from the content in the response
                    viewModel = response.Content.ReadAsAsync<GetProjectResponse.Project>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }

        }

        public GetOperationResponse.Operation CreateTeamProject(string name)
        {
            GetOperationResponse.Operation operation = new GetOperationResponse.Operation();
            
            Object projectData = new
            {
                name = name,
                description = "VanDelay Industries travel app",
                capabilities = new
                {
                    versioncontrol = new
                    {
                        sourceControlType = "Git"
                    },
                    processTemplate = new
                    {
                        templateTypeId = "6b724908-ef14-45cf-84f8-768b5384da45"
                    }
                }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string         
                var patchValue = new StringContent(JsonConvert.SerializeObject(projectData), Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");

                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/projects?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    operation = response.Content.ReadAsAsync<GetOperationResponse.Operation>().Result;
                }
                 else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    operation.Message = msg.ToString();
                }   

                operation.HttpStatusCode = response.StatusCode;

                return operation;
            }
        }

        public GetOperationResponse.Operation GetOperation(string url)
        {
            // create a viewmodel that is a class that represents the returned json response
            GetOperationResponse.Operation viewModel = new GetOperationResponse.Operation();

            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync(url).Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    // set the viewmodel from the content in the response
                    viewModel = response.Content.ReadAsAsync<GetOperationResponse.Operation>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }

        }

        public GetOperationResponse.Operation RenameTeamProject(string projectId, string newProjectName)
        {
            GetOperationResponse.Operation operation = new GetOperationResponse.Operation();

            Object projectData = new
            {
                name = newProjectName,                
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string         
                var patchValue = new StringContent(JsonConvert.SerializeObject(projectData), Encoding.UTF8, "application/json");
                var method = new HttpMethod("PATCH");

                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/projects/" + projectId + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    operation = response.Content.ReadAsAsync<GetOperationResponse.Operation>().Result;
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    operation.Message = msg.ToString();
                }

                operation.HttpStatusCode = response.StatusCode;

                return operation;
            }
        }

        public GetOperationResponse.Operation ChangeTeamProjectDescription(string projectId, string projectDescription)
        {
            GetOperationResponse.Operation opertion = new GetOperationResponse.Operation();

            Object projectData = new
            {
                description = projectDescription
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string         
                var patchValue = new StringContent(JsonConvert.SerializeObject(projectData), Encoding.UTF8, "application/json");
                var method = new HttpMethod("PATCH");

                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/projects/" + projectId + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    opertion = response.Content.ReadAsAsync<GetOperationResponse.Operation>().Result;
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    opertion.Message = msg.ToString();
                }

                opertion.HttpStatusCode = response.StatusCode;

                return opertion;
            }
        }

        public HttpStatusCode DeleteTeamProject(string projectId)
        {          
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                var method = new HttpMethod("DELETE");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/projects/" + projectId + "?api-version=2.2");
                var response = client.SendAsync(request).Result;

                return response.StatusCode;
            }
        }
    }
}
