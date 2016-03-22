using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Process;

namespace VstsRestApiSamples.Client.APIs.Process
{
    public class Processes
    {
        private string _account;
        private string _login;

        public Processes(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
        }

        public HttpStatusCode GetListOfProcesses()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/process/processes?api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<ListofProcessesResponse.Processes>().Result;
                }

                return response.StatusCode;
            }
        }

        public HttpStatusCode GetProcess(string processId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/process/processes/" + processId + "?api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<GetProcessResponse.Process>().Result;
                }

                return response.StatusCode;
            }
        }
    }
}
