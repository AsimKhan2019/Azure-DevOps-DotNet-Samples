using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class GetOperationResponse
    {
        public class Operation : BaseViewModel
        {
            public string id { get; set; }
            public string status { get; set; }
            public string url { get; set; }
            public _Links _links { get; set; }
        }

        public class _Links
        {
            public Self self { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

    }
}
