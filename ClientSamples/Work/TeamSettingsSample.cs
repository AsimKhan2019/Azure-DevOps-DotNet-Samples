using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;

namespace Vsts.ClientSamples.Work
{
    public class TeamSettingsSample : ClientSample
    {
  
        public TeamSettingsSample(ClientSampleContext context) : base(context)
        {
        }

        [ClientSampleMethod]
        public TeamSetting GetTeamSettings(string project)
        {    
            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();   
                 
            var context = new TeamContext(project);
            TeamSetting result = workClient.GetTeamSettingsAsync(context).Result;

            return result;
        }

        [ClientSampleMethod]
        public TeamSetting UpdateTeamSettings(string project)
        {
            IDictionary<string, bool> backlogVisibilities = new Dictionary<string, bool>() {
                { "Microsoft.EpicCategory", false },
                { "Microsoft.FeatureCategory", true },
                { "Microsoft.RequirementCategory", true }
            };

            TeamSettingsPatch updatedTeamSettings = new TeamSettingsPatch() {
                BugsBehavior = BugsBehavior.AsRequirements,
                WorkingDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday },
                BacklogVisibilities = backlogVisibilities
            };

            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            var context = new TeamContext(project);

            TeamSetting result = workClient.UpdateTeamSettingsAsync(updatedTeamSettings, context).Result;

            return result;
        }
    }
}
