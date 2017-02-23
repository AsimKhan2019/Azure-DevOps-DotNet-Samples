using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsClientLibrariesSamples.Work
{
    public class TeamSettings
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public TeamSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public TeamSetting GetTeamSettings(string project)
        {    
            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkHttpClient workHttpClient = connection.GetClient<WorkHttpClient>();        
            var teamContext = new TeamContext(project);
            TeamSetting result = workHttpClient.GetTeamSettingsAsync(teamContext).Result;
            return result;
        }

        public TeamSetting UpdateTeamSettings(string project)
        {
            IDictionary<string, bool> backlogVisibilities = new Dictionary<string, bool>() {
                { "Microsoft.EpicCategory", false },
                { "Microsoft.FeatureCategory", true },
                { "Microsoft.RequirementCategory", true }
            };

            TeamSettingsPatch patchDocument = new TeamSettingsPatch() {
                BugsBehavior = BugsBehavior.AsRequirements,
                WorkingDays = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday },
                BacklogVisibilities = backlogVisibilities
            };

            VssConnection connection = new VssConnection(_uri, _credentials);
            WorkHttpClient workHttpClient = connection.GetClient<WorkHttpClient>();
            var teamContext = new TeamContext(project);
            TeamSetting result = workHttpClient.UpdateTeamSettingsAsync(patchDocument, teamContext).Result;
            return result;
        }
    }
}
