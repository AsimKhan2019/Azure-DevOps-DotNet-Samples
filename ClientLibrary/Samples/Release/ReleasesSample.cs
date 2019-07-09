using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net.Http;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Newtonsoft.Json;

using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;

namespace Microsoft.Azure.DevOps.ClientSamples.Release
{
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleasesResource)]
    public class ReleasesSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web";
        private const string NonSecretReleaseLevelVaraible = "NonSecretReleaseLevelVaraible";
        private int newlyCreatedReleaseDefinitionId = 0;
        private int completedReleaseId = 0;

        [ClientSampleMethod]
        public ReleaseDefinition CreateReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string currentUserId = ClientSampleHelpers.GetCurrentUserId(this.Context).ToString();

            // If you want to override varaibles at release create time you should set 'AllowOverride'.
            ConfigurationVariableValue nonSceretVariable = new ConfigurationVariableValue();
            nonSceretVariable.Value = "NonSecretValue";
            nonSceretVariable.IsSecret = false;
            nonSceretVariable.AllowOverride = true;

            ConfigurationVariableValue sceretVariable = new ConfigurationVariableValue();
            sceretVariable.Value = "SecretValue";
            sceretVariable.IsSecret = true;

            ConfigurationVariableValue nonOverrideVariable = new ConfigurationVariableValue();
            nonOverrideVariable.Value = "NonOverridevariable";
            nonOverrideVariable.IsSecret = false;
            nonOverrideVariable.AllowOverride = false;

            Dictionary<string, ConfigurationVariableValue> releaseLevelVariables = new Dictionary<string, ConfigurationVariableValue>();
            releaseLevelVariables.Add(NonSecretReleaseLevelVaraible, nonSceretVariable);
            releaseLevelVariables.Add("SecretReleaseLevelVaraible", sceretVariable);
            releaseLevelVariables.Add("NonOverridevariable", nonOverrideVariable);

            ReleaseDefinition definition = new ReleaseDefinition()
            {
                Name = releaseDefinitionName,
                Revision = 1,
                Variables = releaseLevelVariables, // You can add varaibles at environment level also.
                Environments = new List<ReleaseDefinitionEnvironment>()
                {
                    new ReleaseDefinitionEnvironment()
                    {
                        Name = "PROD",
                        DeployPhases = new List<DeployPhase>()
                            {
                                new AgentBasedDeployPhase()
                                {
                                    Name = "Run on agent",
                                    Rank = 1,
                                    DeploymentInput = new AgentDeploymentInput()
                                    {
                                      QueueId = 1
                                    }
                                }
                            },
                        PreDeployApprovals = new ReleaseDefinitionApprovals()
                        {
                            Approvals = new List<ReleaseDefinitionApprovalStep>()
                            {
                                new ReleaseDefinitionApprovalStep()
                                {
                                    IsAutomated = false,
                                    Rank = 1,
                                    Approver = new IdentityRef() { Id = currentUserId }
                                },
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

            newlyCreatedReleaseDefinitionId = releaseDefinition.Id;

            Context.Log("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);

            return releaseDefinition;
        }

        [ClientSampleMethod]
        public ReleaseDefinition GetReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Show a release definitions
            ReleaseDefinition releaseDefinition = releaseClient.GetReleaseDefinitionAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId).Result;

            Context.Log("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);

            return releaseDefinition;
        }

        [ClientSampleMethod]
        public List<ReleaseDefinition> ListAllReleaseDefinitions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Show the release definitions
            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;

            foreach (ReleaseDefinition releaseDefinition in releaseDefinitions)
            {
                Context.Log("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
            }

            return releaseDefinitions;
        }

        [ClientSampleMethod]
        public List<ReleaseDefinition> ListAllReleaseDefinitionsWithEnvironmentsExpanded()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Show the release definitions
            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, expand: ReleaseDefinitionExpands.Environments).Result;

            foreach (ReleaseDefinition releaseDefinition in releaseDefinitions)
            {
                Context.Log("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
            }

            return releaseDefinitions;
        }

        [ClientSampleMethod]
        public List<ReleaseDefinition> ListAllReleaseDefinitionsWithArtifactsExpanded()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Show the release definitions
            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, expand: ReleaseDefinitionExpands.Artifacts).Result;

            foreach (ReleaseDefinition releaseDefinition in releaseDefinitions)
            {
                Context.Log("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
            }

            return releaseDefinitions;
        }

        [ClientSampleMethod]
        public List<ReleaseDefinition> ListAllReleaseDefinitionsWithEnvironmentsAndArtifactsExpanded()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Show the release definitions
            ReleaseDefinitionExpands expands = ReleaseDefinitionExpands.Environments | ReleaseDefinitionExpands.Artifacts;
            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, expand: expands).Result;

            foreach (ReleaseDefinition releaseDefinition in releaseDefinitions)
            {
                Context.Log("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
            }

            return releaseDefinitions;
        }

        [ClientSampleMethod]
        public ReleaseDefinition UpdateReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            ReleaseDefinition releaseDefinition = releaseClient.GetReleaseDefinitionAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId).Result;

            // add a non secret variable to definition
            ConfigurationVariableValue variable = new ConfigurationVariableValue();
            variable.Value = "NonSecretValue";
            variable.IsSecret = false;
            releaseDefinition.Variables.Add("NonSecretVariable", variable);

            // update release definition
            ReleaseDefinition updatedReleaseDefinition = releaseClient.UpdateReleaseDefinitionAsync(project: projectName, releaseDefinition: releaseDefinition).Result;

            Context.Log("{0} {1} {2}", updatedReleaseDefinition.Id.ToString().PadLeft(6), updatedReleaseDefinition.Revision, updatedReleaseDefinition.ModifiedOn);

            return releaseDefinition;
        }

        [ClientSampleMethod]
        public IEnumerable<ReleaseDefinitionRevision> GetReleaseDefinitionRevisions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get revision
            List<ReleaseDefinitionRevision> revisions = releaseClient.GetReleaseDefinitionHistoryAsync(projectName, newlyCreatedReleaseDefinitionId).Result;

            foreach (ReleaseDefinitionRevision revision in revisions)
            {
                Context.Log("{0} {1} {2} {3}", revision.DefinitionId.ToString().PadLeft(6), revision.Revision, revision.ChangedDate, revision.ChangedBy.DisplayName);
            }

            return revisions;
        }

        [ClientSampleMethod]
        public WebApiRelease CreateRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Override release level varaible. You can override environment level varaible also.
            Dictionary<string, ConfigurationVariableValue> overrideReleaseLevelVariables = new Dictionary<string, ConfigurationVariableValue>();
            ConfigurationVariableValue nonSecretReleaseLevelVaraible = new ConfigurationVariableValue();
            nonSecretReleaseLevelVaraible.Value = "NonSecretValueChanged";
            nonSecretReleaseLevelVaraible.IsSecret = false;
            overrideReleaseLevelVariables.Add(NonSecretReleaseLevelVaraible, nonSecretReleaseLevelVaraible);

            // Create release
            WebApiRelease release = CreateRelease(releaseClient, newlyCreatedReleaseDefinitionId, projectName, overrideReleaseLevelVariables);

            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            return release;
        }

        [ClientSampleMethod]
        public WebApiRelease GetRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<WebApiRelease> releases = releaseClient.GetReleasesAsync(project: projectName).Result;

            int releaseId = releases.FirstOrDefault().Id;

            // Show a release
            WebApiRelease release = releaseClient.GetReleaseAsync(project: projectName, releaseId: releaseId).Result;

            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            return release;
        }

        [ClientSampleMethod]
        public WebApiRelease UpdateRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<WebApiRelease> releases = releaseClient.GetReleasesAsync(project: projectName).Result;
            int releaseId = releases.FirstOrDefault().Id;

            WebApiRelease release = releaseClient.GetReleaseAsync(project: projectName, releaseId: releaseId).Result;
            
            IDictionary<string, ConfigurationVariableValue> varaibles = release.Variables;
            varaibles.Add(
                new KeyValuePair<string, ConfigurationVariableValue>("System.Debug", new ConfigurationVariableValue
                        { AllowOverride = true, IsSecret = false, Value = "false" }
                )
            );
             
            // Update the release
            WebApiRelease updatedRelease = releaseClient.UpdateReleaseAsync(release: release, project: projectName, releaseId: releaseId).Result;

            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), updatedRelease.Name);

            return release;
        }

        [ClientSampleMethod]
        public IEnumerable<WebApiRelease> ListAllReleases()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();

            List<WebApiRelease> releases = releaseClient.GetReleasesAsync(project: projectName).Result;

            // Show the releases
            foreach (WebApiRelease release in releases)
            {
                Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Status);
            }

            return releases;
        }

        [ClientSampleMethod]
        public IEnumerable<WebApiRelease> ListAllReleasesForReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();
            List<WebApiRelease> releases = releaseClient.GetReleasesAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId).Result;

            // Show releases
            foreach (WebApiRelease release in releases)
            {
                Context.Log("{0} {1} {2}", release.Id.ToString().PadLeft(6), release.Status, release.ReleaseDefinitionReference.Name);
            }

            return releases;
        }

        [ClientSampleMethod]
        public ReleaseEnvironment StartDeployment()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            WebApiRelease release = CreateRelease(releaseClient, newlyCreatedReleaseDefinitionId, projectName);

            ReleaseEnvironmentUpdateMetadata releaseEnvironmentUpdateMetadata = new ReleaseEnvironmentUpdateMetadata()
            {
                Status = EnvironmentStatus.InProgress
            };

            int releaseEnvironmentId = release.Environments.FirstOrDefault().Id;

            // Start deployment to an environment
            ReleaseEnvironment releaseEnvironment = releaseClient.UpdateReleaseEnvironmentAsync(releaseEnvironmentUpdateMetadata, projectName, release.Id, releaseEnvironmentId).Result;
            Context.Log("{0} {1}", releaseEnvironment.Id.ToString().PadLeft(6), releaseEnvironment.Name);

            return releaseEnvironment;
        }

        [ClientSampleMethod]
        public WebApiRelease AbandonAnActiveRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            WebApiRelease release = CreateRelease(releaseClient, newlyCreatedReleaseDefinitionId, projectName);

            ReleaseUpdateMetadata releaseUpdateMetadata = new ReleaseUpdateMetadata()
            {
                Comment = "Abandon the release",
                Status = ReleaseStatus.Abandoned
            };

            // Abandon a release
            WebApiRelease updatedRelease = releaseClient.UpdateReleaseResourceAsync(releaseUpdateMetadata, projectName, release.Id).Result;
            Context.Log("{0} {1}", updatedRelease.Id.ToString().PadLeft(6), updatedRelease.Name);

            return release;
        }

        [ClientSampleMethod]
        public IEnumerable<ReleaseApproval> ListAllPendingApprovals()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();

            List<ReleaseApproval> releaseApprovals = new List<ReleaseApproval>();

            // Iterate (as needed) to get the full set of approvals
            int continuationToken = 0;
            bool parseResult;
            do
            {
                IPagedCollection<ReleaseApproval> releaseApprovalsPage = releaseClient.GetApprovalsAsync2(project: projectName, continuationToken: continuationToken).Result;

                releaseApprovals.AddRange(releaseApprovalsPage);

                int parsedContinuationToken = 0;
                parseResult = int.TryParse(releaseApprovalsPage.ContinuationToken, out parsedContinuationToken);
                if (parseResult)
                {
                    continuationToken = parsedContinuationToken;
                }
            } while ((continuationToken != 0) && parseResult);

            // Show the approvals
            foreach (ReleaseApproval releaseApproval in releaseApprovals)
            {
                Context.Log("{0} {1}", releaseApproval.Id.ToString().PadLeft(6), releaseApproval.Status);
            }

            return releaseApprovals;
        }

        [ClientSampleMethod]
        public IEnumerable<ReleaseApproval> ListPendingApprovalsForASpecificUser()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string assignedToFilter = ClientSampleHelpers.GetCurrentUserDisplayName(this.Context);
            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();

            List<ReleaseApproval> releaseApprovals = new List<ReleaseApproval>();

            // Iterate (as needed) to get the full set of approvals
            int continuationToken = 0;
            bool parseResult;
            do
            {
                IPagedCollection<ReleaseApproval> releaseApprovalsPage = releaseClient.GetApprovalsAsync2(project: projectName, assignedToFilter: assignedToFilter, continuationToken: continuationToken).Result;

                releaseApprovals.AddRange(releaseApprovalsPage);

                int parsedContinuationToken = 0;
                parseResult = int.TryParse(releaseApprovalsPage.ContinuationToken, out parsedContinuationToken);
                if (parseResult)
                {
                    continuationToken = parsedContinuationToken;
                }
            } while ((continuationToken != 0) && parseResult);

            // Show the approvals
            foreach (ReleaseApproval releaseApproval in releaseApprovals)
            {
                Context.Log("{0} {1}", releaseApproval.Id.ToString().PadLeft(6), releaseApproval.Status);
            }

            return releaseApprovals;
        }

        [ClientSampleMethod]
        public IEnumerable<ReleaseApproval> ListPendingApprovalsForASpecificARelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();

            var releases = releaseClient.GetReleasesAsync(project: projectName).Result;

            int releaseIdFilter = releases.FirstOrDefault().Id;

            List<ReleaseApproval> releaseApprovals = new List<ReleaseApproval>();

            // Iterate (as needed) to get the full set of approvals
            int continuationToken = 0;
            bool parseResult;
            do
            {
                IPagedCollection<ReleaseApproval> releaseApprovalsPage = releaseClient.GetApprovalsAsync2(project: projectName, releaseIdsFilter: new List<int> { releaseIdFilter }, continuationToken: continuationToken).Result;

                releaseApprovals.AddRange(releaseApprovalsPage);

                int parsedContinuationToken = 0;
                parseResult = int.TryParse(releaseApprovalsPage.ContinuationToken, out parsedContinuationToken);
                if (parseResult)
                {
                    continuationToken = parsedContinuationToken;
                }
            } while ((continuationToken != 0) && parseResult);

            // Show the approvals
            foreach (ReleaseApproval releaseApproval in releaseApprovals)
            {
                Context.Log("{0} {1}", releaseApproval.Id.ToString().PadLeft(6), releaseApproval.Status);
            }

            return releaseApprovals;
        }

        [ClientSampleMethod]
        public void ApprovePendingRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string assignedToFilter = ClientSampleHelpers.GetCurrentUserDisplayName(this.Context);

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();
            ReleaseApproval updateApproval = new ReleaseApproval { Status = ApprovalStatus.Approved, Comments = "Good to go!" };

            // Get all pending approval to the current user
            IList<ReleaseApproval> releaseApprovalsPage = releaseClient.GetApprovalsAsync(project: projectName, assignedToFilter: assignedToFilter).Result;
            ReleaseApproval releaseApprovalToApprove = releaseApprovalsPage.FirstOrDefault();

            if (releaseApprovalToApprove != null)
            {
                ReleaseApproval approval = releaseClient.UpdateReleaseApprovalAsync(project: projectName, approval: updateApproval, approvalId: releaseApprovalToApprove.Id).Result;

                Context.Log("{0} {1}", approval.Id.ToString().PadLeft(6), approval.Status);
            }
        }

        [ClientSampleMethod]
        public IEnumerable<Deployment> ListDeployments()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<Deployment> deployments = releaseClient.GetDeploymentsAsync(project: projectName).Result;

            // Show the deployments
            foreach (Deployment deployment in deployments)
            {
                Context.Log("{0} {1}", deployment.Id.ToString().PadLeft(6), deployment.Release.Name);
            }

            return deployments;
        }

        [ClientSampleMethod]
        public IEnumerable<Deployment> ListDeploymentsForAGivenDefinitionId()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<Deployment> deployments = releaseClient.GetDeploymentsAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId).Result;

            // Show the deployments
            foreach (Deployment deployment in deployments)
            {
                Context.Log("{0} {1}", deployment.Id.ToString().PadLeft(6), deployment.Release.Name);
            }

            return deployments;
        }

        [ClientSampleMethod]
        public IEnumerable<Deployment> ListAllDeploymentsForASpecificReleaseDefinitionId()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;

            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;

            List<Deployment> deployments = new List<Deployment>();

            // Iterate (as needed) to get the full set of deployments
            int continuationToken = 0;
            bool parseResult;
            do
            {
                IPagedCollection<Deployment> releaseDeploymentsPage = releaseClient.GetDeploymentsAsync2(project: projectName, definitionId: releaseDefinitionId, continuationToken: continuationToken).Result;

                deployments.AddRange(releaseDeploymentsPage);

                int parsedContinuationToken = 0;
                parseResult = int.TryParse(releaseDeploymentsPage.ContinuationToken, out parsedContinuationToken);
                if (parseResult)
                {
                    continuationToken = parsedContinuationToken;
                }
            } while ((continuationToken != 0) && parseResult);

            // Show the deployments
            foreach (Deployment deployment in deployments)
            {
                Context.Log("{0} {1}", deployment.Id.ToString().PadLeft(6), deployment.DeploymentStatus);
            }

            return deployments;
        }

        [ClientSampleMethod]
        public System.IO.Stream GetLogsOfReleaseTask()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get Release Defintion
            ReleaseDefinition releaseDefinition = releaseClient.GetReleaseDefinitionAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId).Result;

            // Get all completed environment releases. 
            // environmentStatusFilter values Undefined = 0,  NotStarted = 1, InProgress = 2, Succeeded = 4, Canceled = 8, Rejected = 16, Qeued = 32, Scheduled = 64, PartiallySucceeded = 128
            List<WebApiRelease> releases = new List<WebApiRelease>();
            ClientSampleHelpers.Retry(
                        TimeSpan.FromMinutes(2),
                        TimeSpan.FromSeconds(5),
                        () =>
                        {
                            releases = releaseClient.GetReleasesAsync(project: projectName, definitionId: releaseDefinition.Id, definitionEnvironmentId: releaseDefinition.Environments.First().Id, expand: ReleaseExpands.Environments, environmentStatusFilter: 4).Result;
                            return releases.Count > 0;
                        });

            // Get first release first environment first deployment first task log
            WebApiRelease release = releaseClient.GetReleaseAsync(project: projectName, releaseId: releases.First().Id).Result;
            this.completedReleaseId = release.Id;
            ReleaseEnvironment releaseEnvironment = release.Environments.First();
            ReleaseDeployPhase deployPhase = releaseEnvironment.DeploySteps.First().ReleaseDeployPhases.FirstOrDefault();
            ReleaseTask task = deployPhase.DeploymentJobs.First().Tasks.First();
            System.IO.Stream taskLog = releaseClient.GetTaskLogAsync(project: projectName, releaseId: release.Id, environmentId: releaseEnvironment.Id, releaseDeployPhaseId: deployPhase.Id, taskId: task.Id).Result;
            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Id);

            return taskLog;
        }

        [ClientSampleMethod]
        public System.IO.Stream GetLogsOfRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            System.IO.Stream taskLogs = releaseClient.GetLogsAsync(project: projectName, releaseId: completedReleaseId).Result;
            Context.Log("{0} {1}", completedReleaseId.ToString().PadLeft(6), completedReleaseId);

            return taskLogs;
        }

        [ClientSampleMethod]
        public string GetReleaseRevisionOfRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get release
            WebApiRelease release = releaseClient.GetReleaseAsync(project: projectName, releaseId: completedReleaseId).Result;

            // Get release revision
            System.IO.Stream releaseRevision = releaseClient.GetReleaseRevisionAsync(project: projectName, releaseId: release.Id, definitionSnapshotRevision: release.DefinitionSnapshotRevision).Result;
            Context.Log("{0} {1}", release.Id.ToString().PadLeft(6), release.Id);

            StreamReader reader = new StreamReader(releaseRevision);
            string releaseData = reader.ReadToEnd();

            return releaseData;
        }

        [ClientSampleMethod]
        public ReleaseDefinitionSummary GetReleaseDefinitionSummary()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Get release definition summary
            ReleaseDefinitionSummary releaseDefinitionSummary = releaseClient.GetReleaseDefinitionSummaryAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId, releaseCount: 2).Result;

            Context.Log("{0} {1}", newlyCreatedReleaseDefinitionId.ToString().PadLeft(6), newlyCreatedReleaseDefinitionId);

            return releaseDefinitionSummary;
        }


        [ClientSampleMethod]
        public void GetReleaseDefinitionAccessControlLists()
        {
            VssConnection connection = Context.Connection;
            var project = ClientSampleHelpers.FindAnyProject(this.Context);

            // We need the security client to talk to RM service instead of the default TFS to get us the ACLs for release definition 
            SecurityHttpClient securityHttpClient = connection.GetClient<SecurityHttpClient>(Guid.Parse(ReleaseManagementApiConstants.InstanceType));
            var acls = securityHttpClient.QueryAccessControlListsAsync(
                SecurityConstants.ReleaseManagementSecurityNamespaceId,
                CreateToken(project.Id, newlyCreatedReleaseDefinitionId),
                null, // all desciptors 
                true,
                true).Result;
        }

        [ClientSampleMethod]
        public void DeleteReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // delete release definition
            releaseClient.DeleteReleaseDefinitionAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId, forceDelete: true).SyncResult();

        }

        public static WebApiRelease CreateRelease(ReleaseHttpClient releaseClient, int releaseDefinitionId, string projectName, Dictionary<string, ConfigurationVariableValue> overrideVaraibles = null)
        {
            BuildVersion instanceReference = new BuildVersion { Id = "2" };
            ArtifactMetadata artifact = new ArtifactMetadata { Alias = "Fabrikam.CI", InstanceReference = instanceReference };
            ReleaseStartMetadata releaseStartMetaData = new ReleaseStartMetadata();
            releaseStartMetaData.DefinitionId = releaseDefinitionId;
            releaseStartMetaData.Description = "Creating Sample release";

            if (overrideVaraibles != null)
            {
                // If you want to override varaibles at release create time, 'AllowOverride' on variable should be set while creating RD.
                // You can override environment level variables using releaseStartMetaData.EnvironmentsMetadata.Variables.
                releaseStartMetaData.Variables = overrideVaraibles;
            }

            releaseStartMetaData.Artifacts = new[] { artifact };
            // Create  a release
            WebApiRelease release =
                releaseClient.CreateReleaseAsync(project: projectName, releaseStartMetadata: releaseStartMetaData).Result;
            return release;
        }

        private static string CreateToken(Guid projectId, int releaseDefinitionId)
        {
            const string tokenNameFormat = "{0}/{1}";

            return string.Format(tokenNameFormat, projectId, releaseDefinitionId);
        }
    }
}
