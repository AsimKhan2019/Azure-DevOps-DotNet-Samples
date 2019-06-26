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
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleaseAttachmentsResource)]
    public class ReleaseAttachmentSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web-with-ReleaseAttachment";
        private int _newlyCreatedReleaseDefinitionId = 0;
        private int _newlyCreatedRelease = 0;
        private const string taskName = "PowerShell Script";

        [ClientSampleMethod]
        public List<ReleaseTaskAttachment> GetReleaseAttachment()
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
                                new AgentBasedDeployPhase()
                                {
                                    Name = "Run on agent",
                                    Rank = 1,
                                    DeploymentInput = new AgentDeploymentInput()
                                    {
                                      QueueId = 1
                                    },
                                    WorkflowTasks = new List<WorkflowTask>
                                    {
                                         new WorkflowTask
                                         {
                                            Name =  taskName,
                                            Enabled = true,
                                            TimeoutInMinutes = 0,
                                            Inputs = new Dictionary<string, string> {
                                                { "targetType", "inline" },
                                                { "script", "New-Item -Path 'newfile.txt' -ItemType File\n\nWrite-Host \"##vso[task.addattachment type=myattachmenttype;name=myattachmentname;]$(SYSTEM.DEFAULTWORKINGDIRECTORY)\\newfile.txt\"" }
                                            },
                                            TaskId = new Guid("e213ff0f-5d5c-4791-802d-52ea3e7be1f1"),
                                            Version = "2.*",
                                            DefinitionType = "task",
                                            Condition = "succeeded()",
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
                                    Rank = 1
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
            Context.Log("{0} {1} {2}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name, projectName);

            // create a release
            WebApiRelease release = ReleasesSample.CreateRelease(releaseClient, _newlyCreatedReleaseDefinitionId, projectName);
            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);
            _newlyCreatedRelease = release.Id;

            // Wait till deployment completed
            ClientSampleHelpers.Retry(
                        TimeSpan.FromMinutes(2),
                        TimeSpan.FromSeconds(5),
                        () =>
                        {
                            release = releaseClient.GetReleaseAsync(project: projectName, releaseId: release.Id).Result;
                            return release != null && release.Environments.First().Status == EnvironmentStatus.Succeeded;
                        });

            // Get release task attachments
            ReleaseEnvironment environment = release.Environments.FirstOrDefault();
            DeploymentAttempt deployStep = environment.DeploySteps.First();
            Guid planId = deployStep.ReleaseDeployPhases.First().RunPlanId.Value;
            List<ReleaseTaskAttachment> releaseTaskAttachment = releaseClient.GetReleaseTaskAttachmentsAsync(project: projectName, releaseId: release.Id, environmentId: environment.Id, attemptId: deployStep.Attempt, planId: planId, type: "myattachmenttype").Result;

            Context.Log("{0} {1}", releaseTaskAttachment.First().Name.PadLeft(6), releaseTaskAttachment.First().Type);

            return releaseTaskAttachment;
        }

        [ClientSampleMethod]
        public System.IO.Stream GetReleaseTaskAttachmentContent()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            WebApiRelease release = releaseClient.GetReleaseAsync(project: projectName, releaseId: this._newlyCreatedRelease).Result;
            
            // Get release task attachments
            ReleaseEnvironment environment = release.Environments.FirstOrDefault();
            DeploymentAttempt deployStep = environment.DeploySteps.First();
            Guid planId = deployStep.ReleaseDeployPhases.First().RunPlanId.Value;

            List<ReleaseTaskAttachment> releaseTaskAttachment = releaseClient.GetReleaseTaskAttachmentsAsync(project: projectName, releaseId: release.Id, environmentId: environment.Id, attemptId: deployStep.Attempt, planId: planId, type: "myattachmenttype").Result;

            ReleaseTaskAttachment firstReleaseTaskAttachment = releaseTaskAttachment.First();
            Guid timelineId = firstReleaseTaskAttachment.TimelineId;
            Guid recordId = firstReleaseTaskAttachment.RecordId;
            string attachmentType = firstReleaseTaskAttachment.Type;
            string attachmentName = firstReleaseTaskAttachment.Name;
            System.IO.Stream attachmentData = releaseClient.GetReleaseTaskAttachmentContentAsync(project: projectName, releaseId: release.Id, environmentId: environment.Id, attemptId: deployStep.Attempt, planId: planId, timelineId: timelineId, recordId: recordId,  type: attachmentType, name: attachmentName).Result;

            Context.Log("{0} {1}", attachmentName.PadLeft(6), attachmentType);

            return attachmentData;
        }

        [ClientSampleMethod]
        public void DeleteReleaseDefinitionWithReleaseAttachments()
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
