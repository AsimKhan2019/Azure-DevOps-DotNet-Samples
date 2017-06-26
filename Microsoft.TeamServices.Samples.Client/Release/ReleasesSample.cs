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

namespace Microsoft.TeamServices.Samples.Client.Release
{
    [ClientSample(ReleaseManagementApiConstants.ReleaseAreaName, ReleaseManagementApiConstants.ReleasesResource)]
    public class ReleasesSample : ClientSample
    {
        private const string releaseDefinitionName = "Fabrikam-web";

        [ClientSampleMethod]

        public List<ReleaseDefinition> ListAllReleaseDefinitions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // Show the releaes definitions
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

            // Show the releaes definitions
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

            // Show the releaes definitions
            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, expand: ReleaseDefinitionExpands.Artifacts).Result;

            foreach (ReleaseDefinition releaseDefinition in releaseDefinitions)
            {
                Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);
            }

            return releaseDefinitions;
        }

        [ClientSampleMethod]

        public ReleaseDefinition GetAReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;

            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;

            // Show a releaes definitions
            ReleaseDefinition releaseDefinition = releaseClient.GetReleaseDefinitionAsync(project: projectName, definitionId: releaseDefinitionId).Result;

            Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);

            return releaseDefinition;
        }

        [ClientSampleMethod]

        public ReleaseDefinition CreateAReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            // JSON to create release definition
            string releaseDefinitionJson = "{\"name\":\"Fabrikam-web\",\"revision\" : 1,\"environments\":[{\"name\":\"PROD\",\"deployPhases\":[{\"name\":\"Run on agent\",\"phaseType\":1,\"rank\":1, \"deploymentInput\":{\"queueId\":2}}],\"preDeployApprovals\":{\"approvals\":[{\"rank\":1,\"isAutomated\":true}]},\"postDeployApprovals\":{\"approvals\":[{\"rank\":1,\"isAutomated\":true}]},\"retentionPolicy\":{\"daysToKeep\":30,\"releasesToKeep\":3,\"retainBuild\":true}}]}";

            ReleaseDefinition definition = JsonConvert.DeserializeObject<ReleaseDefinition>(releaseDefinitionJson);

            // create a release definition
            ReleaseDefinition releaseDefinition = releaseClient.CreateReleaseDefinitionAsync(project: projectName, releaseDefinition: definition).Result;

            Console.WriteLine("{0} {1}", releaseDefinition.Id.ToString().PadLeft(6), releaseDefinition.Name);

            return releaseDefinition;
        }

        [ClientSampleMethod]

        public ReleaseDefinition UpdateAReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, searchText: releaseDefinitionName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;
            ReleaseDefinition releaseDefinition = releaseClient.GetReleaseDefinitionAsync(project: projectName, definitionId: releaseDefinitionId).Result;

            // add a non secret variable to definition
            ConfigurationVariableValue variable = new ConfigurationVariableValue();
            variable.Value = "NonSecretValue";
            variable.IsSecret = false;
            releaseDefinition.Variables.Add("NonSecretVariable", variable);

            // update release definition
            ReleaseDefinition updatedReleaseDefinition = releaseClient.UpdateReleaseDefinitionAsync(project: projectName, releaseDefinition: releaseDefinition).Result;

            Console.WriteLine("{0} {1}", updatedReleaseDefinition.Id.ToString().PadLeft(6), updatedReleaseDefinition.Name);

            return releaseDefinition;
        }

        [ClientSampleMethod]

        public IEnumerable<ReleaseDefinitionRevision> GetReleaseDefinitionRevisions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, searchText: releaseDefinitionName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;

            // Get revision
            List<ReleaseDefinitionRevision> revisions = releaseClient.GetReleaseDefinitionHistoryAsync(projectName, releaseDefinitionId).Result;

            foreach (ReleaseDefinitionRevision revision in revisions)
            {
                Console.WriteLine("{0} {1}", revision.DefinitionId.ToString().PadLeft(6), revision.Revision);
            }

            return revisions;
        }

        [ClientSampleMethod]

        public System.IO.Stream GetReleaseDefinitionForARevision()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, searchText: releaseDefinitionName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;

            // Get revision
            System.IO.Stream revision = releaseClient.GetReleaseDefinitionRevisionAsync(projectName, releaseDefinitionId, revision: 1).Result;

            return revision;
        }

        [ClientSampleMethod]

        public void DeleteAReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName, searchText: releaseDefinitionName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;

            // delete release definition
            releaseClient.DeleteReleaseDefinitionAsync(project: projectName, definitionId: releaseDefinitionId).SyncResult();

        }

        [ClientSampleMethod]

        public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> ListAllReleases()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();

            List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases = releaseClient.GetReleasesAsync(project: projectName).Result;

            // Show the releases
            foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release in releases)
            {
                Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Status);
            }

            return releases;
        }

        [ClientSampleMethod]

        public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> ListAllReleasesForAReleaseDefinition()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient2 releaseClient = connection.GetClient<ReleaseHttpClient2>();
            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;
            List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases = releaseClient.GetReleasesAsync(project: projectName, definitionId: releaseDefinitionId).Result;

            // Show the approvals
            foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release in releases)
            {
                Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Status);
            }

            return releases;
        }

        [ClientSampleMethod]

        public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release GetARelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release> releases = releaseClient.GetReleasesAsync(project: projectName).Result;

            int releaseId = releases.FirstOrDefault().Id;

            // Show a releaes
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release = releaseClient.GetReleaseAsync(project: projectName, releaseId: releaseId).Result;

            Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            return release;
        }

        [ClientSampleMethod]

        public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release CreateARelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;

            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release = CreateRelease(releaseClient, releaseDefinitionId, projectName);

            Console.WriteLine("{0} {1}", release.Id.ToString().PadLeft(6), release.Name);

            return release;
        }

        [ClientSampleMethod]

        public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release UpdateReleaseEnvironment()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release = CreateRelease(releaseClient, releaseDefinitionId, projectName);

            ReleaseEnvironmentUpdateMetadata releaseEnvironmentUpdateMetadata = new ReleaseEnvironmentUpdateMetadata()
            {
                Status = EnvironmentStatus.InProgress
            };

            int releaseEnvironmentId = release.Environments.FirstOrDefault().Id;

            // Abandon a release
            ReleaseEnvironment releaseEnvironment = releaseClient.UpdateReleaseEnvironmentAsync(releaseEnvironmentUpdateMetadata, projectName, release.Id, releaseEnvironmentId).Result;
            Console.WriteLine("{0} {1}", releaseEnvironment.Id.ToString().PadLeft(6), releaseEnvironment.Name);

            return release;
        }

        [ClientSampleMethod]

        public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release AbandonAnActiveRelease()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a release client instance
            VssConnection connection = Context.Connection;
            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

            List<ReleaseDefinition> releaseDefinitions = releaseClient.GetReleaseDefinitionsAsync(project: projectName).Result;
            int releaseDefinitionId = releaseDefinitions.FirstOrDefault().Id;
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release = CreateRelease(releaseClient, releaseDefinitionId, projectName);

            ReleaseUpdateMetadata releaseUpdateMetadata = new ReleaseUpdateMetadata()
            {
                Comment = "Abandon the release",
                Status = ReleaseStatus.Abandoned
            };

            // Abandon a release
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release updatedRelease = releaseClient.UpdateReleaseResourceAsync(releaseUpdateMetadata, projectName, release.Id).Result;
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
            string assignedToFilter = ClientSampleHelpers.GetCurrentUserName(this.Context);
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

        public void UpdateStatusOfAnApprovalFromPendingToApproved()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string assignedToFilter = ClientSampleHelpers.GetCurrentUserName(this.Context);

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

        private static VisualStudio.Services.ReleaseManagement.WebApi.Release CreateRelease(ReleaseHttpClient releaseClient, int releaseDefinitionId, string projectName)
        {
            BuildVersion instanceReference = new BuildVersion { Id = "2" };
            ArtifactMetadata artifact = new ArtifactMetadata { Alias = "Fabrikam.CI", InstanceReference = instanceReference };
            ReleaseStartMetadata releaseStartMetaData = new ReleaseStartMetadata();
            releaseStartMetaData.DefinitionId = releaseDefinitionId;
            releaseStartMetaData.Description = "Creating Sample release";
            releaseStartMetaData.Artifacts = new[] { artifact };
            // Create  a releaes
            Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release =
                releaseClient.CreateReleaseAsync(project: projectName, releaseStartMetadata: releaseStartMetaData).Result;
            return release;
        }
    }
}
