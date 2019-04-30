using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.ProjectsAndTeams
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.TeamsResource)]
    public class TeamsSample : ClientSample
    { 
        [ClientSampleMethod]
        public IEnumerable<WebApiTeam> ListOrderedTeams()
        {
            // Get a random project to load the teams for
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get a client
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();
            
            // Get the teams for a project
            IEnumerable<WebApiTeam> teams = teamClient.GetTeamsAsync(project.Id.ToString()).Result;

            // Order the projects by name
            teams = teams.OrderBy(team => { return team.Name; });

            Console.WriteLine("Project: " + project.Name);
            foreach(var team in teams)
            {
                Console.WriteLine("  " + team.Name);
            }

            return teams;
        }

        /// <summary>
        /// Get details about a specific team
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public WebApiTeam GetTeam()
        {
            // Get any project then get any team from it
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            string teamName = ClientSampleHelpers.FindAnyTeam(this.Context, project.Id).Name;

            // Get a client
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam team = teamClient.GetTeamAsync(project.Id.ToString(), teamName).Result;

            Console.WriteLine("ID         : {0}", team.Id);
            Console.WriteLine("Name       : {0}", team.Name);
            Console.WriteLine("Description: {0}", team.Description);

            return team;
        }

        [ClientSampleMethod(resource:CoreConstants.TeamMembersResource)]
        public IEnumerable<IdentityRef> GetTeamMembers()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            Guid teamId = ClientSampleHelpers.FindAnyTeam(this.Context, projectId).Id;

            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            IEnumerable<IdentityRef> teamMembers = teamClient.GetTeamMembers(projectId.ToString(), teamId.ToString()).Result;

            Console.WriteLine("Members of {0}:", teamId);
            foreach (var member in teamMembers)
            {
                Console.WriteLine("  " + member.DisplayName);
            }

            return teamMembers;
        }

        public IEnumerable<IdentityRef> GetTeamAdmins()
        {
            // Not implemented yet

            return null;
        }

        [ClientSampleMethod]
        public WebApiTeam CreateTeam()
        {
            // Find a project to create the team in
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            string teamName = "Sample team " + Guid.NewGuid();
            string teamDescription = "Short description of my new team";

            // Get a client
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            // Construct team parameters object
            WebApiTeam newTeamCreateParameters = new WebApiTeam()
            {
                Name = teamName,
                Description = teamDescription
            };

            WebApiTeam newTeam = teamClient.CreateTeamAsync(newTeamCreateParameters, project.Id.ToString()).Result;

            Console.WriteLine("Team created: '{0}' (ID: {1})", newTeam.Name, newTeam.Id);

            // Save the team for use later in the rename/delete samples
            this.Context.SetValue<WebApiTeamRef>("$newTeam", newTeam);
            this.Context.SetValue<TeamProjectReference>("$projectOfNewTeam", project);

            return newTeam;
        }

        [ClientSampleMethod]
        public WebApiTeam RenameTeam()
        {
            // Use the previously created team (from the sample above)
            WebApiTeamRef team;
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            if (!this.Context.TryGetValue<WebApiTeamRef>("$newTeam", out team) || !this.Context.TryGetValue<TeamProjectReference>("$projectOfNewTeam", out project))
            {
                throw new Exception("Run the create team sample above first.");
            }

            //Get a client
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            WebApiTeam teamUpdateParameters = new WebApiTeam()
            {
                Name = team.Name + " (renamed)"
            };

            WebApiTeam updatedTeam = teamClient.UpdateTeamAsync(teamUpdateParameters, project.Id.ToString(), team.Id.ToString()).Result;

            Console.WriteLine("Team renamed from '{0}' to '{1}'", team.Name, updatedTeam.Name);

            return updatedTeam;
        }

        [ClientSampleMethod]
        public bool DeleteTeam()
        {
            // Use the previously created team (from the sample above)
            WebApiTeamRef team;
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            if (!this.Context.TryGetValue<WebApiTeamRef>("$newTeam", out team) || !this.Context.TryGetValue<TeamProjectReference>("$projectOfNewTeam", out project))
            {
                throw new Exception("Run the create team sample above first.");
            }

            // Get a client
            VssConnection connection = Context.Connection;
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            try
            {
                teamClient.DeleteTeamAsync(project.Id.ToString(), team.Id.ToString()).SyncResult();

                Console.WriteLine("'{0}' team deleted from project {1}", team.Name, project.Name);

                return true;             
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to delete team: " + ex);
                
                return false;
            }
        }

    }
}
