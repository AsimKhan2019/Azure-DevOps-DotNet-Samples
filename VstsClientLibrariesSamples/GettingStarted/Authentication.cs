using System;
using System.Collections.Generic;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.GettingStarted
{
    public class Authentication
    {
        IConfiguration _configuration;

        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<TeamProjectReference> PersonalAccessToken(string url, string personalAccessToken)
        {
            // create uri and VssBasicCredential variables
            Uri uri = new Uri(url);
            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);

            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(uri, credentials))
            {
                IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;

                if (projects != null)
                {
                    return projects;
                }
                else
                {
                    return null;
                }                
            }
        }
    }
}