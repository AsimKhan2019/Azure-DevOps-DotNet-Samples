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

        /// <summary>
        /// get list of all processes
        /// </summary>
        /// <returns>ListofProcessesResponse.Processes</returns>
        public ListofProcessesResponse.Processes GetListOfProcesses()
        {
            ListofProcessesResponse.Processes viewModel = new ListofProcessesResponse.Processes();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/process/processes?api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofProcessesResponse.Processes>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get process by id
        /// </summary>
        /// <param name="processId"></param>
        /// <returns>GetProcessResponse.Process</returns>
        public GetProcessResponse.Process GetProcess(string processId)
        {
            GetProcessResponse.Process viewModel = new GetProcessResponse.Process();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/process/processes/" + processId + "?api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetProcessResponse.Process>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
