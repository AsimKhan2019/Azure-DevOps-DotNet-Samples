using System;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples
{
    public interface IConfiguration
    {        
        string PersonalAccessToken { get; set; }
        string Project { get; set; }
        string UriString { get; set; }        
        string Query { get; set; }
        string Identity { get; set; }
        string WorkItemIds { get; set; }
    }
}