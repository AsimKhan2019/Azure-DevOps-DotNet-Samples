using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.Client.Helpers;
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

        public HttpStatusCode GetListOfQueries(string project)
        {
            using (var client = new HttpClient())
            {                
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/queries?$depth=1&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<ListofQueriesResponse.Queries>().Result;
                }

                return response.StatusCode;
            }
        }
    }
}
