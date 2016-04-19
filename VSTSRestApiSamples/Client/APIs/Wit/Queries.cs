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
        
        public ListofQueriesResponse.Queries GetListOfQueries(string project)
        {
            ListofQueriesResponse.Queries viewModel = new ListofQueriesResponse.Queries();
          
            using (var client = new HttpClient())
            {                
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries?$depth=2&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofQueriesResponse.Queries>().Result;                    
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

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
