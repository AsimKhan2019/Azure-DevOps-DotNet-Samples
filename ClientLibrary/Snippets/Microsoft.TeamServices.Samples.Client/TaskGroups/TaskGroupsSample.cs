// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskGroupsSample.cs" company="">
//   
// </copyright>
// <summary>
//   The task groups sample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.TeamServices.Samples.Client.TaskGroups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.TeamFoundation.DistributedTask.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// The task groups sample.
    /// </summary>
    [ClientSample(TaskResourceIds.AreaName, TaskResourceIds.TaskGroupsResource)]
    public class TaskGroupsSample : ClientSample
    {
        /// <summary>
        /// The added task group id.
        /// </summary>
        private Guid addedTaskGroupId;

        /// <summary>
        /// The add a task group.
        /// </summary>
        /// <returns>
        /// The <see cref="TaskGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public TaskGroup CreateTaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            Dictionary<string, string> inputs = new Dictionary<string, string>()
                                                    {
                                                        { "scriptType", "inlineScript" },
                                                        { "inlineScript", "# You can write your powershell scripts inline here. \n# You can also pass predefined and custom variables to this scripts using arguments\n\n Write-Host \"Hello World\"" },
                                                    };
            // Task inside the task group
            TaskGroupStep taskGroupStep = new TaskGroupStep()
            {
                Enabled = true,
                DisplayName = "PowerShell Script",
                Inputs = inputs,
                Task = new TaskDefinitionReference { Id = new Guid("e213ff0f-5d5c-4791-802d-52ea3e7be1f1"), VersionSpec = "1.*", DefinitionType = "task" },
            };

            // Task group object
            TaskGroup taskGroup = new TaskGroup()
            {
                Tasks = new List<TaskGroupStep>() { taskGroupStep },
                Category = "Deploy",
                Name = "My PowerShell TG",
                InstanceNameFormat = "Task group: TG",
                Version = new TaskVersion { IsTest = false, Major = 1, Minor = 0, Patch = 0 }
            };

            // Create task group
            var addedTg = taskClient.AddTaskGroupAsync(project: projectName, taskGroup: taskGroup).Result;

            this.addedTaskGroupId = addedTg.Id;

            // Show the added task group
            Console.WriteLine("{0} {1}", addedTg.Id.ToString().PadLeft(6), addedTg.Name);

            return addedTg;
        }

        /// <summary>
        /// The update task group.
        /// </summary>
        /// <returns>
        /// The <see cref="TaskGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public TaskGroup UpdateTaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // Get task group to update
            List<TaskGroup> taskGroups = taskClient.GetTaskGroupsAsync(project: projectName, taskGroupId: this.addedTaskGroupId).Result;

            // Update comment in the task group object.
            TaskGroup taskGroup = taskGroups.FirstOrDefault();
            taskGroup.Comment = "Updated the task group";

            TaskGroup updatedTaskGroup = taskClient.UpdateTaskGroupAsync(project: projectName, taskGroup: taskGroup).Result;

            Console.WriteLine("{0} {1}", updatedTaskGroup.Id.ToString().PadLeft(6), updatedTaskGroup.Comment);

            return updatedTaskGroup;
        }

        /// <summary>
        /// Get task group.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [ClientSampleMethod]
        public TaskGroup GetTaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // Get task group for a given taskgroupId and version.
            TaskGroup taskGroup = taskClient.GetTaskGroupAsync(project: projectName, taskGroupId: addedTaskGroupId, versionSpec: "1.0").Result;

            Console.WriteLine("{0} {1}", taskGroup.Id.ToString().PadLeft(6), taskGroup.Name);

            return taskGroup;
        }

        /// <summary>
        /// The delete a task group.
        /// </summary>
        [ClientSampleMethod]
        public void DeleteATaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // Delete the already created task group
            taskClient.DeleteTaskGroupAsync(project: projectName, taskGroupId: this.addedTaskGroupId).SyncResult();
        }
    }
}
