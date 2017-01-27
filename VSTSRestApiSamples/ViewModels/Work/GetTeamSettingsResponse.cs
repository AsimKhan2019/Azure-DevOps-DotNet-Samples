using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Work
{
    public class GetTeamSettingsResponse
    {
        public class Settings : BaseViewModel
        {
            public BacklogIteration backlogIteration { get; set; }
            public string bugsBehavior { get; set; }
            public string[] workingDays { get; set; }
            public BacklogVisibilities backlogVisibilities { get; set; }
            public DefaultIteration defaultIteration { get; set; }
            public string defaultIterationMacro { get; set; }
            public string url { get; set; }
            public _Links _links { get; set; }
        }

        public class BacklogIteration
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }

        public class BacklogVisibilities
        {
            [JsonProperty(PropertyName = "Microsoft.EpicCategory")]
            public bool MicrosoftEpicCategory { get; set; }

            [JsonProperty(PropertyName = "Microsoft.FeatureCategory")]
            public bool MicrosoftFeatureCategory { get; set; }

            [JsonProperty(PropertyName = "Microsoft.RequirementCategory")]
            public bool MicrosoftRequirementCategory { get; set; }
        }

        public class DefaultIteration
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }

        public class _Links
        {
            public Self self { get; set; }
            public Project project { get; set; }
            public Team team { get; set; }
            public Teamiterations teamIterations { get; set; }
            public Teamfieldvalues teamFieldValues { get; set; }
            public Classificationnode[] classificationNode { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Project
        {
            public string href { get; set; }
        }

        public class Team
        {
            public string href { get; set; }
        }

        public class Teamiterations
        {
            public string href { get; set; }
        }

        public class Teamfieldvalues
        {
            public string href { get; set; }
        }

        public class Classificationnode
        {
            public string href { get; set; }
        }

    }
}
