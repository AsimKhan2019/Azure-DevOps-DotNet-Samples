using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace VstsClientLibrariesSamples.ProjectsAndTeams
{
    public class Teams
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public Teams(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public IEnumerable<WebApiTeam> GetTeams(string project)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            IEnumerable<WebApiTeam> results = teamHttpClient.GetTeamsAsync(project).Result;
            return results;
        }

        public WebApiTeam GetTeam(string project, string team)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            WebApiTeam result = teamHttpClient.GetTeamAsync(project, team).Result;
            return result;
        }

        public IEnumerable<IdentityRef> GetTeamMembers(string project, string team)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            IEnumerable<IdentityRef> results = teamHttpClient.GetTeamMembersAsync(project, team).Result;
            return results;
        }

        public WebApiTeam CreateTeam(string project, WebApiTeam teamData)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            WebApiTeam result = teamHttpClient.CreateTeamAsync(teamData, project).Result;
            return result;
        }

        public WebApiTeam UpdateTeam(string project, string team, WebApiTeam teamData)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            WebApiTeam result = teamHttpClient.UpdateTeamAsync(teamData, project, team).Result;
            return result;
        }

        public void DeleteTeam(string project, string team)
        {
            VssConnection connection = new VssConnection(_uri, _credentials);
            TeamHttpClient teamHttpClient = connection.GetClient<TeamHttpClient>();
            teamHttpClient.DeleteTeamAsync(project, team).SyncResult();                
        }
    }
}
