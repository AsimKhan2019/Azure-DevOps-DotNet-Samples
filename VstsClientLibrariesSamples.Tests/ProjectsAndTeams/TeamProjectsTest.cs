using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VstsClientLibrariesSamples.ProjectsAndTeams;

namespace VstsClientLibrariesSamples.Tests.ProjectsAndTeams
{
    [TestClass]
    public class TeamProjectsTest
    {
        private IConfiguration _configuration = new Configuration();

        [TestInitialize]
        public void TestInitialize()
        {
            InitHelper.GetConfiguration(_configuration);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _configuration = null;
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_GetTeamProjects_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);

            // act            
            IEnumerable<TeamProjectReference> results = projects.GetTeamProjects();

            // assert
            Assert.IsNotNull(results);           
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_GetTeamProjectsByState_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);

            // act            
            IEnumerable<TeamProjectReference> results = projects.GetTeamProjectsByState();

            // assert
            Assert.IsNotNull(results);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_GetTeamProject_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);

            // act
            try
            {
                TeamProjectReference result = projects.GetTeamProjectWithCapabilities(_configuration.Project);

                // assert
                Assert.AreEqual(_configuration.Project, result.Name); 
            }
            catch (System.AggregateException)
            {
                Assert.Inconclusive("project '" + _configuration.Project + "' not found");
            }           
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_CreateTeamProject_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);
            string name = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act           
            OperationReference result = projects.CreateTeamProject(name);

            // assert
            Assert.AreNotEqual(result.Status, OperationStatus.Failed);
           
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_RenameTeamProject_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);
            string name = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act  
            //create the project         
            OperationReference createResult = projects.CreateTeamProject(name);

            //TODO: Instead of sleep, monitor the status ("online")
            System.Threading.Thread.Sleep(5000);

            //get the project so we can get the id
            TeamProjectReference getResult = projects.GetTeamProjectWithCapabilities(name);

            //rename the project
            OperationReference renameResult = projects.RenameTeamProject(getResult.Id, "Vandelay Scrum Project");
            
            //TODO: keep checking the operation untill it failed or is done
                        
            // assert
            Assert.AreNotEqual(createResult.Status, OperationStatus.Failed);
            Assert.AreNotEqual(renameResult.Status, OperationStatus.Failed);

        }
               
        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_ChangeTeamProjectDescription_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);
            string name = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act   
            //create project        
            OperationReference createResult = projects.CreateTeamProject(name);

            //TODO: Instead of sleep, monitor the status ("online")
            System.Threading.Thread.Sleep(5000);

            //get the project we just created so we can get the id
            TeamProjectReference getResult = projects.GetTeamProjectWithCapabilities(name);
            
            //change project desription
            OperationReference updateResult = projects.ChangeTeamProjectDescription(getResult.Id, "This is my new project description");

            //TODO: keep checking the operation untill it failed or is done

            // assert
            Assert.AreNotEqual(createResult.Status, OperationStatus.Failed);
            Assert.AreNotEqual(updateResult.Status, OperationStatus.Failed);
        }

        [TestMethod, TestCategory("Client Libraries")]
        public void CL_ProjectsAndTeams_Projects_DeleteTeamProject_Success()
        {
            // arrange
            TeamProjects projects = new TeamProjects(_configuration);
            string name = System.Guid.NewGuid().ToString().ToLower().Substring(0, 30);

            // act 
            //create a new project          
            OperationReference createResult = projects.CreateTeamProject(name);

            //TODO: Instead of sleep, monitor the status ("online")
            System.Threading.Thread.Sleep(5000);

            //get the project we just created so we can get the id
            TeamProjectReference getResult = projects.GetTeamProjectWithCapabilities(name);

            //delete the project
            OperationReference deleteResult = projects.DeleteTeamProject(getResult.Id);

            //TODO: keep checking the operation untill it failed or is done

            // assert
            Assert.AreNotEqual(createResult.Status, OperationStatus.Failed);
            Assert.AreNotEqual(deleteResult.Status, OperationStatus.Failed);
        }
    }
}
