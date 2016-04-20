using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Wit;

namespace VstsRestApiSamples.Client.APIs.Wit
{
    public class WIQL
    {
        private string _account;
        private string _login;

        public WIQL(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
        }

        public GetWIQLRunStoredQueryResponse.WIQLResult RunStoredQuery(string project, string id)
        {
            GetWIQLRunStoredQueryResponse.WIQLResult viewModel = new GetWIQLRunStoredQueryResponse.WIQLResult();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

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
