using System;

namespace VstsClientLibrariesSamples
{
    public class Configuration : IConfiguration
    {
        public string CollectionId { get; set; }
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
