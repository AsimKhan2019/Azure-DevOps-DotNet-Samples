using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vsts.ClientSamples.Core
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.TeamsResource)]
    public class TeamsSample : ClientSample
    {
        public TeamsSample(ClientSampleContext context) : base(context)
        {
        }

        [ClientSampleMethod]
        public IEnumerable<WebApiTeam> GetOrderedTeamsList(string projectName = "Fabrikam")
        {
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();
            
            IEnumerable<WebApiTeam> teams = teamClient.GetTeamsAsync(projectName).Result;

            teams = teams.OrderBy(team => { return team.Name; });

            return teams;
        }

        [ClientSampleMethod]
        public WebApiTeam GetTeam(string projectName, string teamName)
        {
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam team = teamClient.GetTeamAsync(projectName, teamName).Result;

            return team;
        }

        [ClientSampleMethod(resource:CoreConstants.TeamMembersResource)]
        public IEnumerable<IdentityRef> GetTeamMembers(string projectName, string teamName)
        {
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            IEnumerable<IdentityRef> results = teamClient.GetTeamMembersAsync(projectName, teamName).Result;

            return results;
        }

        [ClientSampleMethod]
        public WebApiTeam CreateTeam(string projectName = "Fabikam", string name = "Fabrikam Ops Team", string description = "Team focused on operations for Fabrikam")
        {
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam newTeamCreateParameters = new WebApiTeam()
            {
                Name = name,
                Description = description
            };

            WebApiTeam newTeam = teamClient.CreateTeamAsync(newTeamCreateParameters, projectName).Result;

            return newTeam;
        }

        [ClientSampleMethod]
        public WebApiTeam RenameTeam(string projectName = "Fabrikam", string currentTeamName = "Fabrikam Ops Team", string newTeamName = "Fabrikam Ops Team (renamed)")
        {
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam teamUpdateParameters = new WebApiTeam()
            {
                Name = newTeamName
            };

            WebApiTeam updatedTeam = teamClient.UpdateTeamAsync(teamUpdateParameters, projectName, currentTeamName).Result;

            return updatedTeam;
        }

        [ClientSampleMethod]
        public bool DeleteTeam(string projectName = "Fabrikam", string teamName = "Fabrikam Ops Team")
        {
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            try
            {
                teamClient.DeleteTeamAsync(projectName, teamName).SyncResult();
                return true;             
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
