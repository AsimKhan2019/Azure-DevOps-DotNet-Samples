using System;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples
{
    public interface IConfiguration
    {
        string AccountName { get; set; }
        string ApplicationId { get; set; }
        string PersonalAccessToken { get; set; }
        string Project { get; set; }
        string UriString { get; }
        string Query { get; set; }
        string Identity { get; set; }
        string WorkItemIds { get; set; }
        Int32 WorkItemId { get; set; }
        string FilePath { get; set; }
    }
}