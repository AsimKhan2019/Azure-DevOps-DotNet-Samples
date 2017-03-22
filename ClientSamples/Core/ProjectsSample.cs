using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Vsts.ClientSamples.Core
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.ProjectsRouteName)]
    public class ProjectsSample: ClientSample
    {

        /// <summary>
        /// Returns all team projects.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public void ListAllProjectsAndTeams()
        {
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            IEnumerable<TeamProjectReference> projects = projectClient.GetProjects().Result;

            foreach(var project in projects)
            {
                Context.Log("Teams for project {0}:", project.Name);

                IEnumerable<WebApiTeam> teams = teamClient.GetTeamsAsync(project.Name).Result;
                foreach (var team in teams)
                {
                    Context.Log(" {0}: {1}", team.Name, team.Description);
                }
            }
        }

        /// <summary>
        /// Returns only the first page of projects
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<TeamProjectReference> ListProjectsByPage()
        {
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            List<TeamProjectReference> projects;
            int page = 0;
            int pageSize = 3;
            do
            {
                projects = new List<TeamProjectReference>(projectClient.GetProjects(top: pageSize, skip: (page * pageSize)).Result);                

                Context.Log("Page {0}", (page + 1));
                foreach(var project in projects)
                {
                    Context.Log(" " + project.Name);
                }

                page++;
            }
            while (projects.Count == pageSize);

            return projects;
        }

        /// <summary>
        /// Returns only team projects that have the specified state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<TeamProjectReference> ListProjectsByState()
        {
            ProjectState state = ProjectState.Deleted;

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            IEnumerable<TeamProjectReference> projects = projectClient.GetProjects(state).Result;

            return projects;
        }

        [ClientSampleMethod]
        public TeamProjectReference GetProjectDetails()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            TeamProject project = projectClient.GetProject(projectName, includeCapabilities: true, includeHistory: true).Result;

            return project;                    
        }

        [ClientSampleMethod]
        public OperationReference CreateProject()
        {
            string name = "Fabrikam";
            string processName = "Agile";

            // Setup version control properties
            Dictionary<string, string> versionControlProperties = new Dictionary<string, string>();

            versionControlProperties[TeamProjectCapabilitiesConstants.VersionControlCapabilityAttributeName] = 
                SourceControlTypes.Git.ToString();

            // Setup process properties       
            ProcessHttpClient processClient = Context.Connection.GetClient<ProcessHttpClient>();
            Guid processId = processClient.GetProcessesAsync().Result.Find(process => { return process.Name.Equals(processName, StringComparison.InvariantCultureIgnoreCase); }).Id;

            Dictionary<string, string> processProperaties = new Dictionary<string, string>();

            processProperaties[TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateTypeIdAttributeName] =
                processId.ToString();

            // Construct capabilities dictionary
            Dictionary<string, Dictionary<string, string>> capabilities = new Dictionary<string, Dictionary<string, string>>();

            capabilities[TeamProjectCapabilitiesConstants.VersionControlCapabilityName] = 
                versionControlProperties;
            capabilities[TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityName] = 
                processProperaties;

            TeamProject projectCreateParameters = new TeamProject()
            {
                Name = name,
                Description = "My project description",
                Capabilities = capabilities
            };

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            OperationReference createProjectOperationStatus = projectClient.QueueCreateProject(projectCreateParameters).Result;

            // TODO: check operation status and wait for it to complete before returning the new project

            return createProjectOperationStatus;
        }

        public OperationReference GetOperationStatus(Guid operationId)
        {
            VssConnection connection = Context.Connection;
            OperationsHttpClient operationsClient = connection.GetClient<OperationsHttpClient>();

            OperationReference operationStatus = operationsClient.GetOperation(operationId).Result;

            return operationStatus;
        }


        [ClientSampleMethod]
        public OperationReference ChangeProjectDescription()
        {
            string projectName = "Fabrikam";
            string newDescription = "New description for Fabrikam";

            TeamProject updatedTeamProject = new TeamProject()
            {
                Description = newDescription
            };

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            Guid projectId = projectClient.GetProject(projectName).Result.Id;

            OperationReference operationStatus = projectClient.UpdateProject(projectId, updatedTeamProject).Result;

            return operationStatus;
        }

        [ClientSampleMethod]
        public OperationReference RenameProject()
        {
            String currentName = "Fabrikam";
            string newName = "Fabrikam (renamed)";

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            Guid projectId = projectClient.GetProject(currentName).Result.Id;

            TeamProject updatedTeamProject = new TeamProject()
            {
                Name = newName
            };

            OperationReference operationStatus = projectClient.UpdateProject(projectId, updatedTeamProject).Result;

            return operationStatus;
        }


        public OperationReference DeleteTeamProject()
        {
            Guid projectId = Guid.Empty; // TODO

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
           
            OperationReference operationStatus = projectClient.QueueDeleteProject(projectId).Result;

            return operationStatus;
        }
    }
}
