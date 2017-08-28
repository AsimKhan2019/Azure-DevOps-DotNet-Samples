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

namespace Microsoft.TeamServices.Samples.Client.Release
{
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleasesResource)]
    public class ManualInterventionSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web-with-MI";
        private int _newlyCreatedReleaseDefinitionId = 0;
        private IList<ManualIntervention> _manualInterventions;
        private WebApiRelease _newlyCreatedRelease;

        [ClientSampleMethod]
        public WebApiRelease CreateReleaseWithManualIntervention()
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
            this._newlyCreatedReleaseDefinitionId = releaseDefinition.Id;

            this._newlyCreatedRelease = ReleasesSample.CreateRelease(releaseClient, _newlyCreatedReleaseDefinitionId, projectName);

            Console.WriteLine("{0} {1}", _newlyCreatedRelease.Id.ToString().PadLeft(6), _newlyCreatedRelease.Name);

            return _newlyCreatedRelease;
        }

        [ClientSampleMethod]
        public IList<ManualIntervention> GetManualInterventions()
        {
            Debugger.Launch();
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get all manual interventions
            IList<ManualIntervention> manualInterventions = releaseClient.GetManualInterventionsAsync(project: projectName, releaseId: this._newlyCreatedRelease.Id).Result;
            foreach (ManualIntervention manualIntervention in manualInterventions)
            {
                Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);
            }

            this._manualInterventions = manualInterventions;

            return manualInterventions;
        }

        [ClientSampleMethod]
        public ManualIntervention GetManualIntervention()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get a manual interventions
            ManualIntervention manualIntervention = releaseClient.GetManualInterventionAsync(project: projectName, releaseId: this._newlyCreatedRelease.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

            return manualIntervention;
        }

        [ClientSampleMethod]
        public ManualIntervention UpdateManualIntervention()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            ManualInterventionUpdateMetadata manualInterventionUpdateMetadata = new ManualInterventionUpdateMetadata()
            {
                Status = ManualInterventionStatus.Approved,
                Comment = "Good to resume"
            };

            // Get a manual interventions
            ManualIntervention manualIntervention = releaseClient.UpdateManualInterventionAsync(manualInterventionUpdateMetadata: manualInterventionUpdateMetadata, project: projectName, releaseId: this._newlyCreatedRelease.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Console.WriteLine("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

            return manualIntervention;
        }

        [ClientSampleMethod]
        public void DeleteReleaseDefinitionWithManualIntervention()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // delete release definition
            releaseClient.DeleteReleaseDefinitionAsync(project: projectName, definitionId: this._newlyCreatedReleaseDefinitionId).SyncResult();

        }
        
    }
}
