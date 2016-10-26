using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

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
            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                IEnumerable<WebApiTeam> results = teamHttpClient.GetTeamsAsync(project).Result;
                return results;
            }
        }

        public WebApiTeam GetTeam(string project, string team)
        {
            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                WebApiTeam result = teamHttpClient.GetTeamAsync(project, team).Result;
                return result;
            }
        }

        public IEnumerable<IdentityRef> GetTeamMembers(string project, string team)
        {
            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                IEnumerable<IdentityRef> results = teamHttpClient.GetTeamMembersAsync(project, team).Result;
                return results;
            }
        }

        public WebApiTeam CreateTeam(string project, WebApiTeam teamData)
        {
            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                WebApiTeam result = teamHttpClient.CreateTeamAsync(teamData, project).Result;
                return result;
            }
        }

        public WebApiTeam UpdateTeam(string project, string team, WebApiTeam teamData)
        {
            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                WebApiTeam result = teamHttpClient.UpdateTeamAsync(teamData, project, team).Result;
                return result;
            }
        }

        public void DeleteTeam(string project, string team)
        {
            // create team object
            using (TeamHttpClient teamHttpClient = new TeamHttpClient(_uri, _credentials))
            {
                teamHttpClient.DeleteTeamAsync(project, team).SyncResult();                
            }
        }

    }
}
