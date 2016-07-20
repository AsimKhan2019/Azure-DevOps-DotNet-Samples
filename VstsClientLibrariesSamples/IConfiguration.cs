using System;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples
{
    public interface IConfiguration
    {
        VssBasicCredential Credentials { get; }
        string ProjectName { get; }
        Uri Uri { get; }
        string QueryName { get; }
        string Identity { get; }
    }
}