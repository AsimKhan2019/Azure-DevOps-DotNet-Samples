using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class GetProjectCollectionResponse
    {
        public class ProjectCollection : BaseViewModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string state { get; set; }
            public _Links _links { get; set; }
        }

        public class _Links
        {
            public Self self { get; set; }
            public Web web { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Web
        {
            public string href { get; set; }
        }

    }
}
