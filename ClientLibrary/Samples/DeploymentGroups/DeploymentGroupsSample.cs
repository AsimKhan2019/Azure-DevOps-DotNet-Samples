using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.DeploymentGroups
{
    /// <summary>
    /// Deployment groups sample.
    /// </summary>
    [ClientSample(TaskResourceIds.AreaName, TaskResourceIds.DeploymentGroupsResource)]
    public class DeploymentGroupsSample : ClientSample
    {
        /// <summary>
        /// The added deployment group id.
        /// </summary>
        private Int32 addedDeploymentGroupId;

        /// <summary>
        /// Add a deployment group.
        /// </summary>
        /// <returns>
        /// The <see cref="DeploymentGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public DeploymentGroup CreateDeploymentGroup()
        {
            String projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient dgClient = connection.GetClient<TaskAgentHttpClient>();

            // Create deployment groups
            DeploymentGroupCreateParameter deploymentGroupCreateParameter = new DeploymentGroupCreateParameter()
            {
                Name = "MyDeploymentGroup1",
                Description = "This deployment group is created to demnostrate the client usage"
            };

            DeploymentGroup addedDeploymentGroup = dgClient.AddDeploymentGroupAsync(projectName, deploymentGroupCreateParameter).Result;
            this.addedDeploymentGroupId = addedDeploymentGroup.Id;
            return addedDeploymentGroup;
        }

        /// <summary>
        /// Get a deployment group.
        /// </summary>
        /// <returns>
        /// The <see cref="DeploymentGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public DeploymentGroup GetDeploymentGroupById()
        {
            String projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient dgClient = connection.GetClient<TaskAgentHttpClient>();

            // Get deployment group by Id
            DeploymentGroup deploymentGroup = dgClient.GetDeploymentGroupAsync(project: projectName, deploymentGroupId: this.addedDeploymentGroupId).Result;
            return deploymentGroup;
        }

        /// <summary>
        /// Get all deployment groups.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [ClientSampleMethod]
        public IList<DeploymentGroup> ListAllDeploymentGroups()
        {
            String projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient dgClient = connection.GetClient<TaskAgentHttpClient>();

            // List all deployment groups
            IList<DeploymentGroup> deploymentGroups = dgClient.GetDeploymentGroupsAsync(project: projectName).Result;
            return deploymentGroups;
        }

        /// <summary>
        /// Update a deployment group.
        /// </summary>
        /// <returns>
        /// The <see cref="DeploymentGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public DeploymentGroup UpdateDeploymentGroup()
        {
            String projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient dgClient = connection.GetClient<TaskAgentHttpClient>();

            // Get task group to update
            DeploymentGroup deploymentGroup = dgClient.GetDeploymentGroupAsync(project: projectName, deploymentGroupId: this.addedDeploymentGroupId).Result;

            DeploymentGroupUpdateParameter deploymentGroupUpdateParameter = new DeploymentGroupUpdateParameter
            {
                Name = deploymentGroup.Name + "-Update1",
                Description = "Description of this deployment group is updated"
            };

            // Update deployment group
            DeploymentGroup updatedDeploymentGroup = dgClient.UpdateDeploymentGroupAsync(project: projectName, deploymentGroupId: this.addedDeploymentGroupId, deploymentGroup: deploymentGroupUpdateParameter).Result;
            return updatedDeploymentGroup;
        }

        /// <summary>
        /// Delete a deployment group.
        /// </summary>
        [ClientSampleMethod]
        public void DeleteDeploymentGroup()
        {
            String projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient dgClient = connection.GetClient<TaskAgentHttpClient>();

            // Delete deployment group by ID
            dgClient.DeleteDeploymentGroupAsync(project: projectName, deploymentGroupId: this.addedDeploymentGroupId).SyncResult();
        }
    }
}
