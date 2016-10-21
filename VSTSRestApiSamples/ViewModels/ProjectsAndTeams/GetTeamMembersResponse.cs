using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class GetTeamMembersResponse
    {

        public class Members : BaseViewModel
        {
            public Value[] value { get; set; }
            public int count { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string uniqueName { get; set; }
            public string url { get; set; }
            public string imageUrl { get; set; }
        }

    }
}
