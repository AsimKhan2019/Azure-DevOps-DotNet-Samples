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
            public Backlogiteration backlogIteration { get; set; }
            public string bugsBehavior { get; set; }
            public string[] workingDays { get; set; }
            public Backlogvisibilities backlogVisibilities { get; set; }
            public Defaultiteration defaultIteration { get; set; }
            public string defaultIterationMacro { get; set; }
            public string url { get; set; }
            public _Links _links { get; set; }
        }

        public class Backlogiteration
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }

        public class Backlogvisibilities
        {
            public bool MicrosoftEpicCategory { get; set; }
            public bool MicrosoftFeatureCategory { get; set; }
            public bool MicrosoftRequirementCategory { get; set; }
        }

        public class Defaultiteration
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
