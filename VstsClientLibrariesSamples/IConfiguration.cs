 using System;

namespace VstsClientLibrariesSamples
{
    public interface IConfiguration
    {
        string AccountName { get; set; }
        Uri CollectionUri { get; }
        string UriString { get; }
        string ApplicationId { get; set; }
        string PersonalAccessToken { get; set; }
        string Project { get; set; }
        string CollectionId { get; set; }
        string Team { get; set; }
        string MoveToProject { get; set; }  
        string Query { get; set; }
        string Identity { get; set; }
        string WorkItemIds { get; set; }
        Int32 WorkItemId { get; set; }
        string FilePath { get; set; }
    }
}