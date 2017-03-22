using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vsts.ClientSamples
{
    public static class ClientSampleHelpers
    {
        public static TeamProjectReference GetDefaultProject(ClientSampleContext context)
        {
            TeamProjectReference project;

            // Check if we already have a default project loaded
            if (!context.TryGetValue<TeamProjectReference>("project", out project))
            {
                VssConnection connection = context.Connection;
                ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

                // Check if an ID was already set (this could have been provided by the caller)
                Guid projectId;
                if (!context.TryGetValue<Guid>("projectId", out projectId))
                {
                    // Get the first project
                    project = projectClient.GetProjects(null, top: 1).Result.FirstOrDefault();
                }
                else
                {
                    // Get the details for this project
                    project = projectClient.GetProject(projectId.ToString()).Result;
                }

                if (project != null)
                {
                    context.SetValue<TeamProjectReference>("project", project);
                }
                else
                {
                    // create a project here?
                    throw new Exception("No projects available for running the sample.");
                }
            }

            return project;
        }

        public static WebApiTeamRef GetDefaultTeam(ClientSampleContext context)
        {
            WebApiTeamRef team;
           
            if (!context.TryGetValue<WebApiTeamRef>("team", out team))
            {
                TeamProjectReference project = GetDefaultProject(context);
                if (project != null)
                {
                    TeamHttpClient teamClient = context.Connection.GetClient<TeamHttpClient>();

                    team = teamClient.GetTeamsAsync(project.Name, top: 1).Result.FirstOrDefault();

                    if (team != null)
                    {
                        context.SetValue<WebApiTeamRef>("team", team);
                    }
                    else
                    {
                        // create a team?
                        throw new Exception("No team available for running this sample.");
                    }
                }
            }

            return team;
        }
    }
}

