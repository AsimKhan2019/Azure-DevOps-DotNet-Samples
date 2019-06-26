using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.Work
{
    [ClientSample(WorkWebConstants.RestArea, "teamsettings")]
    public class TeamSettingsSample : ClientSample
    {

        [ClientSampleMethod]
        public TeamSetting GetTeamSettings()
        {    
            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            Guid teamId = ClientSampleHelpers.FindAnyTeam(this.Context, projectId).Id;

            var context = new TeamContext(projectId, teamId);
            TeamSetting result = workClient.GetTeamSettingsAsync(context).Result;

            Console.WriteLine("Backlog iteration: {0}", result.BacklogIteration.Name);
            Console.WriteLine("Bugs behavior: {0}", result.BugsBehavior);
            Console.WriteLine("Default iteration : {0}", result.DefaultIterationMacro);

            return result;
        }

        [ClientSampleMethod]
        public List<TeamSettingsIteration> GetTeamSettingsCurrentIteration()
        {
            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            Guid teamId = ClientSampleHelpers.FindAnyTeam(this.Context, projectId).Id;

            var context = new TeamContext(projectId, teamId);
            List<TeamSettingsIteration> result = workClient.GetTeamIterationsAsync(context, "current").Result;

            foreach(var item in result)
            {
                Console.WriteLine("Current Iteration: {0}", item.Name);
                Console.WriteLine("Start Date: {0}", item.Attributes.StartDate.ToString());
                Console.WriteLine("Finish Date: {0}", item.Attributes.FinishDate.ToString());
            }           

            return result;
        }

        [ClientSampleMethod]
        public TeamSetting UpdateTeamSettings()
        {
            IDictionary<string, bool> backlogVisibilities = new Dictionary<string, bool>() {
                { "Microsoft.EpicCategory", false },
                { "Microsoft.FeatureCategory", true },
                { "Microsoft.RequirementCategory", true }
            };

            TeamSettingsPatch updatedTeamSettings = new TeamSettingsPatch() {
                BugsBehavior = BugsBehavior.AsTasks,
                WorkingDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday },
                BacklogVisibilities = backlogVisibilities
            };

            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            var context = new TeamContext(projectId);

            TeamSetting result = workClient.UpdateTeamSettingsAsync(updatedTeamSettings, context).Result;

            Console.WriteLine("Backlog iteration: {0}", result.BacklogIteration.Name);
            Console.WriteLine("Bugs behavior: {0}", result.BugsBehavior);
            Console.WriteLine("Default iteration : {0}", result.DefaultIterationMacro);
            Console.WriteLine("Working days: {0}", String.Join(",", result.WorkingDays.Select<DayOfWeek,string>(dow => { return dow.ToString(); })));

            return result;
        }
    }
}
