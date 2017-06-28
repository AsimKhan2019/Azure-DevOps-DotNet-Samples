using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;

using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;

namespace Microsoft.TeamServices.Samples.Client.Release
{
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleasesResource)]
    public class ReleasesSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web";
        private int newlyCreatedReleaseDefinitionId = 0;

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
                Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
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
                Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
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
                Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
            }

            return releaseDefinitions;
        }

        [ClientSampleMethod]
        public ReleaseDefinition CreateReleaseDefinition()
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
                        DeployPhases = new List<DeployPhase>()
                            {
                                new AgentBasedDeployPhase()
                                {
                                    Name = "Run on agent",
                                    Rank = 1
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

            newlyCreatedReleaseDefinitionId = releaseDefinition.Id;

            Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);

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

            Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);

            return releaseDefinition;
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

            Console.WriteLine("{0} {1} {2}", updatedReleaseDefinition.Id.ToString().PadLeft(6), updatedReleaseDefinition.Revision, updatedReleaseDefinition.ModifiedOn);

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
                Console.WriteLine("{0} {1} {2} {3}", revision.DefinitionId.ToString().PadLeft(6), revision.Revision, revision.ChangedDate, revision.ChangedBy.DisplayName);
            }

            return revisions;
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
                Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Status);
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
                Console.WriteLine("{0} {1} {2}", release.Id.ToString().PadLeft(6), release.Status, release.ReleaseDefinitionReference.Name);
            }

            return releases;
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

            Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            return release;
        }

        [ClientSampleMethod]
        public WebApiRelease CreateRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            WebApiRelease release = CreateRelease(releaseClient, newlyCreatedReleaseDefinitionId, projectName);

            Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            return release;
        }

        [ClientSampleMethod]
        public WebApiRelease StartDeployment()
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
            Console.WriteLine("{0} {1}", releaseEnvironment.Id.ToString().PadLeft(6), releaseEnvironment.Name);

            return release;
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
            Console.WriteLine("{0} {1}", updatedRelease.Id.ToString().PadLeft(6), updatedRelease.Name);

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
                Console.WriteLine("{0} {1}", releaseApproval.Id.ToString().PadLeft(6), releaseApproval.Status);
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
                Console.WriteLine("{0} {1}", releaseApproval.Id.ToString().PadLeft(6), releaseApproval.Status);
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
                Console.WriteLine("{0} {1}", releaseApproval.Id.ToString().PadLeft(6), releaseApproval.Status);
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

                Console.WriteLine("{0} {1}", approval.Id.ToString().PadLeft(6), approval.Status);
            }
        }

        [ClientSampleMethod]
        public void DeleteReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // delete release definition
            releaseClient.DeleteReleaseDefinitionAsync(project: projectName, definitionId: newlyCreatedReleaseDefinitionId).SyncResult();

        }

        private static WebApiRelease CreateRelease(ReleaseHttpClient releaseClient, int releaseDefinitionId, string projectName)
        {
            BuildVersion instanceReference = new BuildVersion { Id = "2" };
            ArtifactMetadata artifact = new ArtifactMetadata { Alias = "Fabrikam.CI", InstanceReference = instanceReference };
            ReleaseStartMetadata releaseStartMetaData = new ReleaseStartMetadata();
            releaseStartMetaData.DefinitionId = releaseDefinitionId;
            releaseStartMetaData.Description = "Creating Sample release";
            releaseStartMetaData.Artifacts = new[] { artifact };
            // Create  a release
            WebApiRelease release =
                releaseClient.CreateReleaseAsync(project: projectName, releaseStartMetadata: releaseStartMetaData).Result;
            return release;
        }
    }
}
