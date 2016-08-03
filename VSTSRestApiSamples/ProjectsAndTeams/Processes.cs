using System;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.ProjectsAndTeams
{
    public class Processes
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Processes(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        /// <summary>
        /// get list of all processes
        /// </summary>
        /// <returns>ListofProcessesResponse.Processes</returns>
        public ListofProcessesResponse.Projects GetListOfProcesses()
        {
            ListofProcessesResponse.Projects viewModel = new ListofProcessesResponse.Projects();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/process/processes?api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofProcessesResponse.Projects>().Result;
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
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

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
