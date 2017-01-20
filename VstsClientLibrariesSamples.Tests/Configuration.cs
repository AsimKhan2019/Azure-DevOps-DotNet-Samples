using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.Tests
{
    public class Configuration : IConfiguration
    {
        public string AccountName { get; set; }
        public string ApplicationId { get; set; }
        public string UriString { get { return string.Format("https://{0}.visualstudio.com", AccountName); } }
        public Uri CollectionUri { get { return new Uri(UriString); } }
        public string CollectionId { get; set; }
        public string PersonalAccessToken { get; set; }
        public string Project { get; set; }
        public string Team { get; set; }
        public string MoveToProject { get; set; }
        public string Query { get; set; }
        public string Identity { get; set; }
        public string WorkItemIds { get; set; }
        public Int32 WorkItemId { get; set; }
        public string FilePath { get; set; }
    }
}
