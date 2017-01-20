using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace VstsRestApiSamples.ProjectsAndTeams
{
    public class Samples
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Samples(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public string GetTeams()
        {
            var project = _configuration.Project;
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/projects/" + project + "/teams?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    var teams = response.Content.ReadAsAsync<WebApiTeams>().Result;
                    return "success";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "not found";
                }

                return "failed";
            }
        }

        public string GetTeam()
        {
            var project = _configuration.Project;
            var team = _configuration.Team;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/projects/" + project + "/teams/" + team + "?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    var teams = response.Content.ReadAsAsync<WebApiTeams>().Result;
                    return "success";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "not found";
                }

                return "failed";
            }
        }

        public string GetTeamMembers()
        {
            var project = _configuration.Project;
            var team = _configuration.Team;
            Members members;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/projects/" + project + "/teams/" + team + "/members?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    members = response.Content.ReadAsAsync<Members>().Result;
                    return "success";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "not found";
                }

                return "failed";
            }
        }

        public string CreateTeam()
        {
            var project = _configuration.Project;
            Object teamData = new { name = "My new team" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string         
                var patchValue = new StringContent(JsonConvert.SerializeObject(teamData), Encoding.UTF8, "application/json"); // mediaType needs to be application/json-patch+json for a patch call
                var method = new HttpMethod("POST");

                var request = new HttpRequestMessage(method, _configuration.UriString + "/_apis/projects/" + project + "/teams?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    WebApiTeam teamResponse = response.Content.ReadAsAsync<WebApiTeam>().Result;
                    return "success";
                }

                return "failed";
            }

        }

        public string UpdateTeam()
        {
            var project = _configuration.Project;
            Object team = new { name = "My new team", description = "my teams awesome description" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string         
                var patchValue = new StringContent(JsonConvert.SerializeObject(team), Encoding.UTF8, "application/json"); // mediaType needs to be application/json-patch+json for a patch call
                var method = new HttpMethod("PATCH");

                var request = new HttpRequestMessage(method, _configuration.UriString + "/_apis/projects/" + project + "/teams/My%20new%20team?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    WebApiTeam teamResponse = response.Content.ReadAsAsync<WebApiTeam>().Result;
                    return "success";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "not found";
                }
                
                return "failed";
            }
        }

        public string DeleteTeam()
        {
            var project = _configuration.Project;
            var team = "My new team";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                var method = new HttpMethod("DELETE");

                var request = new HttpRequestMessage(method, _configuration.UriString + "/_apis/projects/" + project + "/teams/" + team + "?api-version=2.2");
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    return "failed";
                }
            }
        }

        public string CreateTeamsByAreaPath()
        {
            return "failed";
        }
    }

    public class WebApiTeams 
    {
        public WebApiTeam[] value { get; set; }
        public int count { get; set; }
    }

    public class WebApiTeam
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string identityUrl { get; set; }
    }

    public class Members
    {
        public Member[] value { get; set; }
        public int count { get; set; }
    }

    public class Member
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string uniqueName { get; set; }
        public string url { get; set; }
        public string imageUrl { get; set; }
    }
}
