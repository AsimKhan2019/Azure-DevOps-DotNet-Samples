using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class GetProjectResponse
    {
        public class Project : BaseViewModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string description { get; set; }
            public string state { get; set; }
            public Capabilities capabilities { get; set; }
            public _Links _links { get; set; }
            public Defaultteam defaultTeam { get; set; }
        }

        public class Capabilities
        {
            public Versioncontrol versioncontrol { get; set; }
            public Processtemplate processTemplate { get; set; }
        }

        public class Versioncontrol
        {
            public string sourceControlType { get; set; }
        }

        public class Processtemplate
        {
            public string templateName { get; set; }
        }

        public class _Links
        {
            public Self self { get; set; }
            public Collection collection { get; set; }
            public Web web { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Collection
        {
            public string href { get; set; }
        }

        public class Web
        {
            public string href { get; set; }
        }

        public class Defaultteam
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }
    }

}
