using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Newtonsoft.Json;

using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Release
{
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleasesResource)]
    public class ManualInterventionSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web-with-MI";
        private IList<ManualIntervention> _manualInterventions;
        private WebApiRelease _newlyCreatedRelease;

        [ClientSampleMethod]
        public IList<ManualIntervention> GetManualInterventions()
        {

            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Create release with manual intervention
            WebApiRelease release = this.CreateReleaseWithManualIntervention("Fabrikam-web-with-MI-1");

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            IList<ManualIntervention> manualInterventions = null;

            // Get all manual interventions
            ClientSampleHelpers.Retry(
                TimeSpan.FromMinutes(2),
                TimeSpan.FromSeconds(5),
                () =>
                {
                    manualInterventions = releaseClient.GetManualInterventionsAsync(project: projectName, releaseId: release.Id).Result;
                    return manualInterventions.Count > 0;
                });

            
            foreach (ManualIntervention manualIntervention in manualInterventions)
            {
                Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);
            }

            this.DeleteReleaseDefinitionWithManualIntervention(release.ReleaseDefinitionReference.Id);

            return manualInterventions;
        }

        [ClientSampleMethod]
        public ManualIntervention GetManualIntervention()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get a manual intervention
            ManualIntervention manualIntervention = releaseClient.GetManualInterventionAsync(project: projectName, releaseId: this._newlyCreatedRelease.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

            return manualIntervention;
        }

        [ClientSampleMethod]
        public ManualIntervention ResumeManualIntervention()
        {
            Debugger.Launch();

            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            ManualInterventionUpdateMetadata manualInterventionUpdateMetadata = new ManualInterventionUpdateMetadata()
            {
                Status = ManualInterventionStatus.Approved,
                Comment = "Good to resume"
            };

            // Resume a manual intervention
            ManualIntervention manualIntervention = releaseClient.UpdateManualInterventionAsync(manualInterventionUpdateMetadata: manualInterventionUpdateMetadata, project: projectName, releaseId: this._newlyCreatedRelease.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

            return manualIntervention;
        }

        [ClientSampleMethod]
        public ManualIntervention RejectManualIntervention()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            ManualInterventionUpdateMetadata manualInterventionUpdateMetadata = new ManualInterventionUpdateMetadata()
            {
                Status = ManualInterventionStatus.Rejected,
                Comment = "Reject it."
            };

            // Reject a manual intervention
            ManualIntervention manualIntervention = releaseClient.UpdateManualInterventionAsync(manualInterventionUpdateMetadata: manualInterventionUpdateMetadata, project: projectName, releaseId: this._newlyCreatedRelease.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

            return manualIntervention;
        }

        private WebApiRelease CreateReleaseWithManualIntervention(string releaseDefinitionName)
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            ReleaseDefinition definition = new ReleaseDefinition()
            {
                Name = releaseDefinitionName,
                Revision = 1,
                Environments = new List<ReleaseDefinitionEnvironment>()
                {
                    new ReleaseDefinitionEnvironment()
                    {
                        Name = "PROD",
                        Conditions = new List<Condition>()
                        {
                            new Condition()
                            {
                                ConditionType = ConditionType.Event,
                                Name = "ReleaseStarted"
                            }
                        },
                        DeployPhases = new List<DeployPhase>()
                            {
                                new RunOnServerDeployPhase()
                                {
                                    Name = "Agentless phase",
                                    Rank = 1,
                                    WorkflowTasks = new List<WorkflowTask>()
                                    {
                                        new WorkflowTask()
                                        {
                                          Name = "Manual Intervention",
                                          TaskId = new Guid("bcb64569-d51a-4af0-9c01-ea5d05b3b622"),
                                          Version = "8.*",
                                          Enabled = true
                                        }
                                    }
                                }
                            },
                        PreDeployApprovals = new ReleaseDefinitionApprovals()
                        {
                            Approvals = new List<ReleaseDefinitionApprovalStep>()
                            {
                                new ReleaseDefinitionApprovalStep()
                                {
                                    IsAutomated = true,
                                    Rank = 1,
                                }
                            }
                        },
                        PostDeployApprovals = new ReleaseDefinitionApprovals()
                        {
                            Approvals = new List<ReleaseDefinitionApprovalStep>()
                            {
                                new ReleaseDefinitionApprovalStep()
                                {
                                    IsAutomated = true,
                                    Rank = 1
                                }
                            }
                        },
                        RetentionPolicy = new EnvironmentRetentionPolicy()
                        {
                            DaysToKeep = 30,
                            ReleasesToKeep = 3,
                            RetainBuild = true
                        }
                    }
                }
            };

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // create a release definition
            ReleaseDefinition releaseDefinition = releaseClient.CreateReleaseDefinitionAsync(project: projectName, releaseDefinition: definition).Result;

            this._newlyCreatedRelease = ReleasesSample.CreateRelease(releaseClient, releaseDefinition.Id, projectName);

            Console.WriteLine("{0} {1}", _newlyCreatedRelease.Id.ToString().PadLeft(6), _newlyCreatedRelease.Name);

            return _newlyCreatedRelease;
        }

        private void DeleteReleaseDefinitionWithManualIntervention(int definitionId)
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // delete release definition
            releaseClient.DeleteReleaseDefinitionAsync(project: projectName, definitionId: definitionId).SyncResult();

        }
        
    }
}
