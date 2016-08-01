using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public TeamProjectReference PersonalAccessToken(string url, string personalAccessToken)
        {
            //create uri and VssBasicCredential variables
            Uri uri = new Uri(url);
            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);

            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(uri, credentials))
            {
                TeamProjectReference project = projectHttpClient.GetProject(_configuration.Project).Result;

                if (project != null)
                {
                    return project;
                }
                else
                {
                    throw new ProjectDoesNotExistException(_configuration.Project);
                }
            }
        }
    }
}
