using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples
{
    public class Configuration : IConfiguration
    {
        public string UriString { get; set; }
        public string PersonalAccessToken { get; set; }
        public string Project { get; set; }
        public string Team { get; set; }
        public string Query { get; set; }
        public string Identity { get; set; }
        public string WorkItemIds { get; set; }
        public Int32 WorkItemId { get; set; }
        public string FilePath { get; set; }
    }
}
