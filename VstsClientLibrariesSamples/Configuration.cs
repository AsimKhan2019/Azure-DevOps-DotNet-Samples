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
        public Uri Uri { get; }
        public VssBasicCredential Credentials { get; }
        public string ProjectName { get; }
        public string QueryName { get; }
        public string Identity { get; }
    }
}
