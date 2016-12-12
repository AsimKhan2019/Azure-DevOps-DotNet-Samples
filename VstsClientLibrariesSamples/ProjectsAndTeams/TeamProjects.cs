using System;
using System.Collections.Generic;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;

namespace VstsClientLibrariesSamples.ProjectsAndTeams
{
    public class TeamProjects
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public TeamProjects(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);            
        }

        public IEnumerable<TeamProjectReference> GetTeamProjects()
        {
            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;
                return projects;
            }
        }

        public IEnumerable<TeamProjectReference> GetTeamProjectsByState()
        {
            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects(ProjectState.All).Result;
                return projects;
            }
        }

        public TeamProjectReference GetTeamProjectWithCapabilities(string name)
        {
            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                TeamProject project = projectHttpClient.GetProject(name, true).Result;
                return project;               
            }                
        }

        public OperationReference CreateTeamProject(string name)
        {
            Dictionary<string, Dictionary<string, string>> capabilities = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> versionControl = new Dictionary<string, string>();
            Dictionary<string, string> processTemplate = new Dictionary<string, string>();

            versionControl.Add("sourceControlType", "Git");
            processTemplate.Add("templateTypeId", "6b724908-ef14-45cf-84f8-768b5384da45");

            capabilities.Add("versioncontrol", versionControl);
            capabilities.Add("processTemplate", processTemplate);

            TeamProject teamProject = new TeamProject()
            {
                Name = name,
                Description = "VanDelay Industries travel app",
                Capabilities = capabilities
            };

            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                var operationReferencee = projectHttpClient.QueueCreateProject(teamProject).Result;
                return operationReferencee;
            }
        }

        public OperationReference GetOperation(System.Guid Id)
        {  
            using (OperationsHttpClient operationsHttpClient = new OperationsHttpClient(_uri, _credentials))
            {
                var operationReferencee = operationsHttpClient.GetOperation(Id).Result;
                return operationReferencee;
            }
        }

        public OperationReference RenameTeamProject(Guid projectToUpdateId, string name)
        {
            TeamProject teamProject = new TeamProject()
            {
                Name = name                               
            };
     
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                var operationReference = projectHttpClient.UpdateProject(projectToUpdateId, teamProject).Result;
                return operationReference;
            }
        }

        public OperationReference ChangeTeamProjectDescription(Guid projectToUpdateId, string description)
        {
            TeamProject teamProject = new TeamProject()
            {
                Description = description
            };

            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                var operationReferencee = projectHttpClient.UpdateProject(projectToUpdateId, teamProject).Result;
                return operationReferencee;
            }
        }

        public OperationReference DeleteTeamProject(Guid projectId)
        {
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                var operationReferencee = projectHttpClient.QueueDeleteProject(projectId).Result;
                return operationReferencee;
            }
        }

    }
}
