using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.ProjectsAndTeams
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.ProjectsRouteName)]
    public class ProjectsSample: ClientSample
    {

        /// <summary>
        /// Returns all team projects and the teams for each.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public Dictionary<TeamProjectReference,IEnumerable<WebApiTeam>> ListAllProjectsAndTeams()
        {
            // Get the project and team clients
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();
            TeamHttpClient teamClient = connection.GetClient<TeamHttpClient>();

            // Call to get the list of projects
            IEnumerable<TeamProjectReference> projects = projectClient.GetProjects().Result;

            Dictionary<TeamProjectReference, IEnumerable<WebApiTeam>> results = new Dictionary<TeamProjectReference, IEnumerable<WebApiTeam>>();
            
            // Iterate over the returned projects
            foreach (var project in projects)
            {
                // Get the teams for the project
                IEnumerable<WebApiTeam> teams = teamClient.GetTeamsAsync(project.Name).Result;

                // Add the project/teams item to the results dictionary
                results.Add(project, teams);

                Console.WriteLine(" " + project.Name);
                // Iterate over the teams and show the name
                foreach (var team in teams)
                {
                    Console.WriteLine("    " + team.Name);
                }
            }

            if (projects.Count() == 0)
            {
                Console.WriteLine("No projects found.");
            }

            return results;
        }

        /// <summary>
        /// Returns only the first page of projects
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<TeamProjectReference> ListProjectsByPage()
        {
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            
            int currentPage = 0;
            int pageSize = 3;
            int lastPageSize = -1;
            List<TeamProjectReference> allProjects = new List<TeamProjectReference>();
            do
            {
                // Get a single page of projects
                IEnumerable<TeamProjectReference> projects = projectClient.GetProjects(top: pageSize, skip: (currentPage * pageSize)).Result;

                // Add the set to the full list
                allProjects.AddRange(projects);

                lastPageSize = projects.Count();
                currentPage++;
                Console.WriteLine(currentPage);

                // Iterate and show the name of each project
                foreach (var project in projects)
                {
                    Console.WriteLine(" " + project.Name);
                }
            }
            while (lastPageSize == pageSize);

            return allProjects;
        }

        [ClientSampleMethod]
        public TeamProjectReference GetProjectDetails()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            // Get the details for the specified project
            TeamProject project = projectClient.GetProject(projectName, includeCapabilities: true, includeHistory: true).Result;

            // Get the "web" URL for this project
            ReferenceLink webLink = project.Links.Links["web"] as ReferenceLink;

            Console.WriteLine("Details for project {0}:", projectName);
            Console.WriteLine();
            Console.WriteLine("  ID          : {0}", project.Id);
            Console.WriteLine("  Description : {0}", project.Description);
            Console.WriteLine("  Web URL     : {0}", (webLink != null ? webLink.Href : "(not available)"));

            return project;                    
        }

        [ClientSampleMethod]
        public TeamProject CreateProject()
        {
            string projectName = "Sample project " + Guid.NewGuid();
            string projectDescription = "Short description for my new project";
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

            // Construct object containing properties needed for creating the project
            TeamProject projectCreateParameters = new TeamProject()
            {
                Name = projectName,
                Description = projectDescription,
                Capabilities = capabilities
            };

            // Get a client
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            TeamProject project = null;
            try
            {
                Console.WriteLine("Queuing project creation...");

                // Queue the project creation operation 
                // This returns an operation object that can be used to check the status of the creation
                OperationReference operation = projectClient.QueueCreateProject(projectCreateParameters).Result;

                ClientSampleHttpLogger.SetSuppressOutput(Context, true);

                // Check the operation status every 5 seconds (for up to 30 seconds)
                Operation completedOperation = WaitForLongRunningOperation(operation.Id, 5, 30).Result;

                // Check if the operation succeeded (the project was created) or failed
                if (completedOperation.Status == OperationStatus.Succeeded)
                {
                    // Get the full details about the newly created project
                    project = projectClient.GetProject(
                        projectCreateParameters.Name,
                        includeCapabilities: true,
                        includeHistory: true).Result;

                    Console.WriteLine();
                    Console.WriteLine("Project created (ID: {0})", project.Id);

                    // Save the newly created project (other sample methods will use it)
                    Context.SetValue<TeamProject>("$newProject", project);
                }
                else
                {
                    Console.WriteLine("Project creation operation failed: " + completedOperation.ResultMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during create project: ", ex.Message);
            }

            return project;
        }

        private async Task<Operation> WaitForLongRunningOperation(Guid operationId, int interavalInSec = 5, int maxTimeInSeconds = 60, CancellationToken cancellationToken = default(CancellationToken))
        {
            OperationsHttpClient operationsClient = this.Context.Connection.GetClient<OperationsHttpClient>();
            DateTime expiration = DateTime.Now.AddSeconds(maxTimeInSeconds);
            int checkCount = 0;

            while (true)
            {
                Console.WriteLine(" Checking status ({0})... ", (checkCount++));

                Operation operation = await operationsClient.GetOperation(operationId, cancellationToken);

                if (!operation.Completed)
                {
                    Console.WriteLine("   Pausing {0} seconds", interavalInSec);

                    await Task.Delay(interavalInSec * 1000);

                    if (DateTime.Now > expiration)
                    {
                        throw new Exception(String.Format("Operation did not complete in {0} seconds.", maxTimeInSeconds));
                    }
                }
                else
                {
                    return operation;   
                }
            }
        }

        [ClientSampleMethod]
        public bool ChangeProjectDescription()
        {
            // Use the project created in the earlier "create project" sample method
            TeamProject project;
            if (!Context.TryGetValue<TeamProject>("$newProject", out project))
            {
                Console.WriteLine("No previously created project to change the description of.");
                return false;
            }

            TeamProject updatedProject = new TeamProject()
            {
                Description = "An event better description for my project!"
            };

            // Get a client
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            Console.WriteLine("Queuing project update...");

            // Queue the update operation
            Guid updateOperationId = projectClient.UpdateProject(project.Id, updatedProject).Result.Id;

            ClientSampleHttpLogger.SetSuppressOutput(Context, true);

            // Check the operation status every 2 seconds (for up to 30 seconds)
            Operation detailedUpdateOperation = WaitForLongRunningOperation(updateOperationId, 2, 30).Result;

            // Check if the operation succeeded (the project was created) or failed
            if (detailedUpdateOperation.Status == OperationStatus.Succeeded)
            {
                Console.WriteLine();
                Console.WriteLine("Project description change from:\n {1}\nto\n {2}", project.Name, project.Description, updatedProject.Description);
                return true;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Unable to change the description for project {0}", project.Name);
                return false;
            } 
        }

        [ClientSampleMethod]
        public bool RenameProject()
        {
            // Use the project created in the earlier "create project" sample method
            TeamProject project;
            if (!Context.TryGetValue<TeamProject>("$newProject", out project))
            {
                Console.WriteLine("No previously created project to change the name of.");

                return false;
            }

            // Get a client
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            TeamProject updatedProject = new TeamProject()
            {
                Name = project.Name + " (renamed)"
            };

            Console.WriteLine("Queuing project update...");

            // Queue the update operation
            Guid updateOperationId = projectClient.UpdateProject(project.Id, updatedProject).Result.Id;

            ClientSampleHttpLogger.SetSuppressOutput(Context, true);

            // Check the operation status every 2 seconds (for up to 30 seconds)
            Operation detailedUpdateOperation = WaitForLongRunningOperation(updateOperationId, 2, 30).Result;

            if (detailedUpdateOperation.Status == OperationStatus.Succeeded)
            {
                Console.WriteLine();
                Console.WriteLine("Project renamed from:\n {0}\nto\n {1}", project.Name, updatedProject.Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        [ClientSampleMethod]
        public bool DeleteProject()
        {
            // Use the project created in the earlier "create project" sample method
            TeamProject project;
            if (!Context.TryGetValue<TeamProject>("$newProject", out project))
            {
                Console.WriteLine("No previously created project found to delete.");

                return false;
            }

            // Get a client
            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            Console.WriteLine("Queuing project delete...");

            // Queue the delete operation
            Guid operationId = projectClient.QueueDeleteProject(project.Id).Result.Id;

            ClientSampleHttpLogger.SetSuppressOutput(Context, true);

            // Check the operation status every 2 seconds (for up to 30 seconds)
            Operation operationResult = WaitForLongRunningOperation(operationId, 2, 30).Result;

            Console.WriteLine();
            Console.WriteLine("Delete project operation completed {0}", operationResult.Status);

            return operationResult.Status == OperationStatus.Succeeded;
        }

        /// <summary>
        /// List projects by their state. For example, only deleted projects.
        /// </summary>
        /// <returns></returns>
        [ClientSampleMethod]
        public IEnumerable<TeamProjectReference> ListProjectsByState()
        {
            ProjectState state = ProjectState.Deleting;

            VssConnection connection = Context.Connection;
            ProjectHttpClient projectClient = connection.GetClient<ProjectHttpClient>();

            // Get a list of all the projects with a state of "deleting"
            IEnumerable<TeamProjectReference> deletedProjects = projectClient.GetProjects(state).Result;

            foreach (var project in deletedProjects)
            {
                Console.WriteLine("  " + project.Name);
            }

            if (deletedProjects.Count() == 0)
            {
                Console.WriteLine("No projects with the state {0}", state);
            }

            return deletedProjects;
        }

    }
}
