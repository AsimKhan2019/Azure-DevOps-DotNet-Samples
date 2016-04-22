using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Wit;
using VstsRestApiSamples.ViewModels.Wit.Queries;

namespace VstsRestApiSamples.Client.APIs.Wit
{
    public class Queries
    {
        private string _account;
        private string _login;

        public Queries(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
        }

        /// <summary>
        /// get list of queries by project
        /// </summary>
        /// <param name="project">project name or id</param>
        /// <returns>ListofQueriesResponse.Queries</returns>
        public ListofQueriesResponse.Queries GetListOfQueries(string project)
        {
            ListofQueriesResponse.Queries viewModel = new ListofQueriesResponse.Queries();
          
            using (var client = new HttpClient())
            {                
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                //$depth=2 is the maximum level deep you can go
                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries?$depth=2&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofQueriesResponse.Queries>().Result;                    
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get list of queries by a specific folder path
        /// </summary>
        /// <param name="project">project name or id</param>
        /// <param name="folderPath">folder path that must be url encoded</param>
        /// <returns>ListofQueriesByFolderPath.Queries</returns>
        public ListofQueriesByFolderPath.Queries GetListOfQueriesByFolderPath(string project, string folderPath)
        {
            ListofQueriesByFolderPath.Queries viewModel = new ListofQueriesByFolderPath.Queries();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);
                               
                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries/" + folderPath + "?$depth=2&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofQueriesByFolderPath.Queries>().Result;                    
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get query or folder by id
        /// </summary>
        /// <param name="project">project name or id</param>
        /// <param name="id">query id</param>
        /// <returns>GetQueryByIdResponse.Queries</returns>
        public GetQueryByIdResponse.Queries GetQueryById(string project, string id)
        {
            GetQueryByIdResponse.Queries viewModel = new GetQueryByIdResponse.Queries();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries/" + id + "?$depth=2&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetQueryByIdResponse.Queries>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
