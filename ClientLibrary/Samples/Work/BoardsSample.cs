using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Work
{
    [ClientSample(WorkWebConstants.RestArea, "boards")]
    public class BoardsSample : ClientSample
    {
        [ClientSampleMethod]
        public Board GetTeamStoriesBoard()
        {
            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            WebApiTeamRef team = ClientSampleHelpers.FindAnyTeam(this.Context, project.Id);

            TeamContext context = new TeamContext(project.Id, team.Id);
            context.Team = team.Name;
            context.Project = project.Name;          

            Board board = workClient.GetBoardAsync(context, "Stories").Result;

            Context.Log("Columns for 'Stories' Board for Project '{0}' and Team '{1}'", context.Project, context.Team);
            Context.Log(""); 
            
            return board;
        }

        [ClientSampleMethod]
        public List<BacklogLevelConfiguration> GetBacklogs()
        {
            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            WebApiTeamRef team = ClientSampleHelpers.FindAnyTeam(this.Context, project.Id);

            TeamContext teamContext = new TeamContext(project.Id, team.Id)
            {
                Team = team.Name,
                Project = project.Name
            };

            List<BacklogLevelConfiguration> backlogs = workClient.GetBacklogsAsync(teamContext).Result;

            foreach(var backlog in backlogs)
            {
                Context.Log("Backlog: '{0}' Type: '{1}'", backlog.Name, backlog.Type);                
            }

            return backlogs;            
        }
    }
}
