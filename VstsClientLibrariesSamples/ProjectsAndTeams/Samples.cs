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
            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;
                return projects;
            }
        }

        public IEnumerable<WebApiTeam> GetTeams()
        {
            string project = _configuration.Project;

            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                IEnumerable<WebApiTeam> results = teamHttpClient.GetTeamsAsync(project).Result;
                return results;
            }
        }

        public WebApiTeam GetTeam()
        {
            string project = _configuration.Project;
            string team = "My new team";

            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                WebApiTeam result = teamHttpClient.GetTeamAsync(project, team).Result;
                return result;
            }
        }

        public IEnumerable<IdentityRef> GetTeamMembers()
        {
            string project = _configuration.Project;
            string team = _configuration.Team;

            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                IEnumerable<IdentityRef> results = teamHttpClient.GetTeamMembersAsync(project, team).Result;
                return results;
            }
        }

        public WebApiTeam CreateTeam()
        {
            string project = _configuration.Project;

            WebApiTeam teamData = new WebApiTeam()
            {
                Name = "My new team"
            };

            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                WebApiTeam result = teamHttpClient.CreateTeamAsync(teamData, project).Result;
                return result;
            }
        }

        public WebApiTeam UpdateTeam()
        {
            string project = _configuration.Project;
            string team = "My new team";

            WebApiTeam teamData = new WebApiTeam()
            {
                Description = "my awesome team description"
            };

            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                WebApiTeam result = teamHttpClient.UpdateTeamAsync(teamData, project, team).Result;
                return result;
            }
        }

        public void DeleteTeam()
        {
            string project = _configuration.Project;
            string team = "My new team";

            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                teamHttpClient.DeleteTeamAsync(project, team).SyncResult();
            }
        }
    }
}
