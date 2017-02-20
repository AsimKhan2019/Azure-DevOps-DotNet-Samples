using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace VstsClientLibrariesSamples.ProjectsAndTeams
{
    public class Samples
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public Samples(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public IEnumerable<TeamProjectReference> GetProjects()
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            ProjectHttpClient projectHttpClient = connection.GetClient<ProjectHttpClient>();
            IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;
            return projects;
        }

        public IEnumerable<WebApiTeam> GetTeams()
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            IEnumerable<WebApiTeam> results = teamHttpClient.GetTeamsAsync(_configuration.Project).Result;
            return results;
        }

        public WebApiTeam GetTeam()
        {
            string teamName = "My new team";

            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            WebApiTeam result = teamHttpClient.GetTeamAsync(_configuration.Project, teamName).Result;
            return result;
        }

        public IEnumerable<IdentityRef> GetTeamMembers()
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            IEnumerable<IdentityRef> results = teamHttpClient.GetTeamMembersAsync(_configuration.Project, _configuration.Team).Result;
            return results;
        }

        public WebApiTeam CreateTeam()
        {
            WebApiTeam teamData = new WebApiTeam()
            {
                Name = "My new team"
            };

            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            WebApiTeam result = teamHttpClient.CreateTeamAsync(teamData, _configuration.Project).Result;
            return result;
        }

        public WebApiTeam UpdateTeam()
        {
            string teamName = "My new team";

            WebApiTeam teamData = new WebApiTeam()
            {
                Description = "my awesome team description"
            };

            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            WebApiTeam result = teamHttpClient.UpdateTeamAsync(teamData, _configuration.Project, teamName).Result;
            return result;
        }

        public void DeleteTeam()
        {
            string teamName = "My new team";

            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            teamHttpClient.DeleteTeamAsync(_configuration.Project, teamName).SyncResult();
        }
    }
}
