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
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleaseGatesResource)]
    public class GatesSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web-with-Gates";
        private int _newlyCreatedReleaseDefinitionId = 0;

        [ClientSampleMethod]
        public ReleaseGates IgnoreGate()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string gateName = "Query Work Items";
            ReleaseDefinitionGatesStep releaseDefinitionGatesStep = new ReleaseDefinitionGatesStep
            {
                Gates = new List<ReleaseDefinitionGate>
                {
                    new ReleaseDefinitionGate
                    {
                         Tasks = new List<WorkflowTask>
                         {
                             new WorkflowTask
                             {
                                 Enabled  = true,
                                 Name = gateName,
                                 TaskId = new Guid("f1e4b0e6-017e-4819-8a48-ef19ae96e289"),
                                 Version = "0.*",
                                 Inputs = new Dictionary<string, string>
                                 {
                                     { "queryId", "c871ca91-e30c-43a0-9306-97a2be93861e" },
                                     { "maxThreshold", "6" },
                                     { "minThreshold", "2" }
                                 }
                             }
                         }
                    }
                },
                GatesOptions = new ReleaseDefinitionGatesOptions
                {
                    IsEnabled = true,
                    MinimumSuccessDuration = 2,
                    SamplingInterval = 5,
                    StabilizationTime = 10,
                    Timeout = 60
                },
                Id = 0
            };

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
                        },
                        PreDeploymentGates = releaseDefinitionGatesStep
                    }
                }
            };

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // create a release definition
            ReleaseDefinition releaseDefinition = releaseClient.CreateReleaseDefinitionAsync(project: projectName, releaseDefinition: definition).Result;
            this._newlyCreatedReleaseDefinitionId = releaseDefinition.Id;

            // create a release
            WebApiRelease release = ReleasesSample.CreateRelease(releaseClient, _newlyCreatedReleaseDefinitionId, projectName);
            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            // Wait till deployment moves to evaluation gates state
            ClientSampleHelpers.Retry(
                        TimeSpan.FromMinutes(2),
                        TimeSpan.FromSeconds(5),
                        () =>
                        {
                            release = releaseClient.GetReleaseAsync(project: projectName, releaseId: release.Id).Result;
                            return release != null && release.Environments.First().DeploySteps.FirstOrDefault().OperationStatus == DeploymentOperationStatus.EvaluatingGates;
                        });

            // Ignore the gate
            GateUpdateMetadata gateUpdateMetadata = new GateUpdateMetadata
            {
                Comment = "Ignore gate",
                GatesToIgnore = new List<string> { gateName }
            };

            int gateStepId = release.Environments.FirstOrDefault().DeploySteps.FirstOrDefault().PreDeploymentGates.Id;
            ReleaseGates releaseGates = releaseClient.UpdateGatesAsync(gateUpdateMetadata: gateUpdateMetadata, project: projectName, gateStepId: gateStepId).Result;

            Context.Log("{0} {1}", releaseGates.Id.ToString().PadLeft(6), releaseGates.Id);

            return releaseGates;
        }

        [ClientSampleMethod]
        public void DeleteReleaseDefinitionWithGates()
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
