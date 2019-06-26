using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Newtonsoft.Json;

using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;

namespace Microsoft.Azure.DevOps.ClientSamples.Release
{
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ManualInterventionsResource)]
    public class ManualInterventionSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web-with-MI";
        private int _newlyCreatedReleaseDefinitionId = 0;
        private IList<ManualIntervention> _manualInterventions;
        private WebApiRelease _newlyCreatedRelease1;
        private WebApiRelease _newlyCreatedRelease2;

        [ClientSampleMethod]
        public void CreateReleaseWithManualIntervention()
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

            this._newlyCreatedRelease1 = ReleasesSample.CreateRelease(releaseClient, _newlyCreatedReleaseDefinitionId, projectName);
            this._newlyCreatedRelease2 = ReleasesSample.CreateRelease(releaseClient, _newlyCreatedReleaseDefinitionId, projectName);

            Context.Log("{0} {1}", _newlyCreatedRelease1.Id.ToString().PadLeft(6), _newlyCreatedRelease1.Name);
            Context.Log("{0} {1}", _newlyCreatedRelease2.Id.ToString().PadLeft(6), _newlyCreatedRelease2.Name);
        }

        [ClientSampleMethod]
        public IList<ManualIntervention> GetManualInterventions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

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
                  manualInterventions = releaseClient.GetManualInterventionsAsync(project: projectName, releaseId: this._newlyCreatedRelease1.Id).Result;
                                   return manualInterventions.Count > 0;
                });

            foreach (ManualIntervention manualIntervention in manualInterventions)
            {
                Context.Log("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);
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

            // Get a manual intervention
            ManualIntervention manualIntervention = releaseClient.GetManualInterventionAsync(project: projectName, releaseId: this._newlyCreatedRelease1.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Context.Log("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

            return manualIntervention;
        }

        [ClientSampleMethod]
        public ManualIntervention ResumeManualIntervention()
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

            // Update a manual intervention
            ManualIntervention manualIntervention = releaseClient.UpdateManualInterventionAsync(manualInterventionUpdateMetadata: manualInterventionUpdateMetadata, project: projectName, releaseId: this._newlyCreatedRelease1.Id, manualInterventionId: this._manualInterventions.FirstOrDefault().Id).Result;
            Context.Log("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

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
                Comment = "Reject MI"
            };

            IList<ManualIntervention> manualInterventions = null;

            // Get all manual interventions
            ClientSampleHelpers.Retry(
            TimeSpan.FromMinutes(2),
            TimeSpan.FromSeconds(5),
            () =>
            {
                manualInterventions = releaseClient.GetManualInterventionsAsync(project: projectName, releaseId: this._newlyCreatedRelease2.Id).Result;
                return manualInterventions.Count > 0;
            });

            // Update a manual intervention
            ManualIntervention manualIntervention = releaseClient.UpdateManualInterventionAsync(manualInterventionUpdateMetadata: manualInterventionUpdateMetadata, project: projectName, releaseId: this._newlyCreatedRelease2.Id, manualInterventionId: manualInterventions.FirstOrDefault().Id).Result;
            Context.Log("{0} {1}", manualIntervention.Id.ToString().PadLeft(6), manualIntervention.Name);

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
            releaseClient.DeleteReleaseDefinitionAsync(project: projectName, definitionId: this._newlyCreatedReleaseDefinitionId, forceDelete: true).SyncResult();
        }
    }
}
