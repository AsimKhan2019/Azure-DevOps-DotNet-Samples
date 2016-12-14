using System;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.WorkItemTracking;
using VstsRestApiSamples.ViewModels.WorkItemTracking.Queries;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class Queries
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Queries(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        // / <summary>
        // / get list of queries by project
        // / </summary>
        // / <param name="project">project name or id</param>
        // / <returns>ListofQueriesResponse.Queries</returns>
        public GetQueriesResponse.Queries GetListOfQueries(string project)
        {
            GetQueriesResponse.Queries viewModel = new GetQueriesResponse.Queries();
          
            using (var client = new HttpClient())
            {                
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // $depth=2 is the maximum level deep you can go
                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries?$depth=2&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetQueriesResponse.Queries>().Result;                    
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        // / <summary>
        // / get list of queries by a specific folder path
        // / </summary>
        // / <param name="project">project name or id</param>
        // / <param name="folderPath">folder path that must be url encoded</param>
        // / <returns>ListofQueriesByFolderPath.Queries</returns>
        public GetQueriesByFolderPath.Queries GetListOfQueriesByFolderPath(string project, string folderPath)
        {
            GetQueriesByFolderPath.Queries viewModel = new GetQueriesByFolderPath.Queries();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                               
                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries/" + folderPath + "?$depth=2&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetQueriesByFolderPath.Queries>().Result;                    
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / get queries for a specific query path
        // / </summary>
        // / <param name="project">project name or id</param>
        // / <param name="path">full query path</param>
        // / <returns>ListofQueriesByFolderPath.Queries</returns>
        public GetQueriesByIdResponse.Queries GetQueryByPath(string project, string path)
        {
            GetQueriesByIdResponse.Queries viewModel = new GetQueriesByIdResponse.Queries();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries/" + path + "?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetQueriesByIdResponse.Queries>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / get query or folder by id
        // / </summary>
        // / <param name="project">project name or id</param>
        // / <param name="id">query id</param>
        // / <returns>GetQueryByIdResponse.Queries</returns>
        public GetQueriesByIdResponse.Queries GetQueryById(string project, string id)
        {
            GetQueriesByIdResponse.Queries viewModel = new GetQueriesByIdResponse.Queries();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries/" + id + "?$depth=2&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetQueriesByIdResponse.Queries>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
