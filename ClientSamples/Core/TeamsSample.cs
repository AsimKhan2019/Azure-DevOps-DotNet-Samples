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
        [ClientSampleMethod]
        public IEnumerable<WebApiTeam> ListOrderedTeams()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();
            
            IEnumerable<WebApiTeam> teams = teamClient.GetTeamsAsync(project.Id.ToString()).Result;

            teams = teams.OrderBy(team => { return team.Name; });

            Console.WriteLine("Teams for project {0}", project.Name);
            foreach(var team in teams)
            {
                Console.WriteLine(" " + team.Name);
            }

            return teams;
        }

        [ClientSampleMethod]
        public WebApiTeam GetTeam()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            string teamName = ClientSampleHelpers.FindAnyTeam(this.Context, project.Id).Name;

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam team = teamClient.GetTeamAsync(project.Id.ToString(), teamName).Result;

            return team;
        }

        [ClientSampleMethod(resource:CoreConstants.TeamMembersResource)]
        public IEnumerable<IdentityRef> GetTeamMembers()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            Guid teamId = ClientSampleHelpers.FindAnyTeam(this.Context, projectId).Id;

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            IEnumerable<IdentityRef> teamMembers = teamClient.GetTeamMembersAsync(projectId.ToString(), teamId.ToString()).Result;

            Console.WriteLine("Members of team {0}", teamId);
            foreach (var member in teamMembers)
            {
                Console.WriteLine(" " + member.DisplayName);
            }


            return teamMembers;
        }

        [ClientSampleMethod]
        public WebApiTeam CreateTeam()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            string teamName = "Sample team " + DateTime.UtcNow;
            string teamDescription = "Team focused on operations for Fabrikam";

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam newTeamCreateParameters = new WebApiTeam()
            {
                Name = teamName,
                Description = teamDescription
            };

            WebApiTeam newTeam = teamClient.CreateTeamAsync(newTeamCreateParameters, project.Id.ToString()).Result;
        
            Console.WriteLine("Team created: {0}", newTeam.Name);

            // Save the team for use later in the rename/delete samples
            this.Context.SetValue<WebApiTeamRef>("$newTeam", newTeam);

            return newTeam;
        }

        [ClientSampleMethod]
        public WebApiTeam RenameTeam()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Try to use the name for the team created in an earlier sample
            WebApiTeamRef team;
            if (!this.Context.TryGetValue<WebApiTeamRef>("$newTeam", out team))
            {
                throw new Exception("Run the create team sample above first.");
            }

            string currentTeamName = team.Name;

            string newTeamName = currentTeamName + " (renamed)";

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam teamUpdateParameters = new WebApiTeam()
            {
                Name = newTeamName
            };

            WebApiTeam updatedTeam = teamClient.UpdateTeamAsync(teamUpdateParameters, project.Id.ToString(), currentTeamName).Result;

            Console.WriteLine("Team renamed from {0} to {1}.", currentTeamName, newTeamName);

            return updatedTeam;
        }

        [ClientSampleMethod]
        public bool DeleteTeam()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            WebApiTeamRef team;
            if (this.Context.TryGetValue<WebApiTeamRef>("$newTeam", out team))
            {
                throw new Exception("Run the create team sample above first.");
            }

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            try
            {
                teamClient.DeleteTeamAsync(project.Id.ToString(), team.Id.ToString()).SyncResult();

                Console.WriteLine("Deleted team {0} from project {1}", team.Name, project.Name);

                return true;             
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
