using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.ViewModels.Work;
using static VstsRestApiSamples.ViewModels.Work.GetTeamSettingsResponse;

namespace VstsRestApiSamples.Work
{
    public class TeamSettings
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public TeamSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public GetTeamSettingsResponse.Settings GetTeamSettings(string project, string team)
        {
            GetTeamSettingsResponse.Settings viewModel = new GetTeamSettingsResponse.Settings();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/" + team + "/_apis/work/teamsettings?api-version=3.0-preview").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetTeamSettingsResponse.Settings>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetTeamSettingsResponse.Settings UpdateTeamSettings(string project)
        {
            GetTeamSettingsResponse.Settings viewModel = new GetTeamSettingsResponse.Settings();
            Object patchDocument = new Object();      

            // change some values on a few fields
            patchDocument = new
            {
                bugsBehavior = "AsRequirements",
                workingDays = new string[4] { "monday", "tuesday", "wednesday", "thursday" },
                backlogVisibilities = new BacklogVisibilities()
                {
                    MicrosoftEpicCategory = false,
                    MicrosoftFeatureCategory = true,
                    MicrosoftRequirementCategory = true
                }
            };              
         
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json"); // mediaType needs to be application/json-patch+json for a patch call

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + project + "/_apis/work/teamsettings?api-version=3.0-preview") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetTeamSettingsResponse.Settings>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
