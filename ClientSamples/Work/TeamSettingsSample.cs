using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using System;
using System.Collections.Generic;

namespace Vsts.ClientSamples.Work
{
    [ClientSample(WorkWebConstants.RestArea, "teamsettings")]
    public class TeamSettingsSample : ClientSample
    {

        public TeamSettingsSample(): base()
        {
        }

        public TeamSettingsSample(ClientSampleContext context) : base(context)
        {
        }

        [ClientSampleMethod]
        public TeamSetting GetTeamSettings()
        {    
            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            Guid projectId = ClientSampleHelpers.GetDefaultProject(this.Context).Id;
                 
            var context = new TeamContext(projectId);
            TeamSetting result = workClient.GetTeamSettingsAsync(context).Result;

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
                BugsBehavior = BugsBehavior.AsRequirements,
                WorkingDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday },
                BacklogVisibilities = backlogVisibilities
            };

            VssConnection connection = Context.Connection;
            WorkHttpClient workClient = connection.GetClient<WorkHttpClient>();

            Guid projectId = ClientSampleHelpers.GetDefaultProject(this.Context).Id;
            var context = new TeamContext(projectId);

            TeamSetting result = workClient.UpdateTeamSettingsAsync(updatedTeamSettings, context).Result;

            return result;
        }
    }
}
