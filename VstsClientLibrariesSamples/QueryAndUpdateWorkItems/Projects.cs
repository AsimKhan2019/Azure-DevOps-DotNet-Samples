using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.QueryAndUpdateWorkItems
{
    public class Projects
    {
        readonly IConfiguration _configuration;

        public Projects(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TeamProjectReference GetProjectByName(string name)
        {
            //create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_configuration.Uri, _configuration.Credentials))
            {
                TeamProjectReference project = projectHttpClient.GetProject(name).Result;

                if (project != null)
                {
                    return project;
                }
                else
                {
                    throw new ProjectDoesNotExistException(name);
                }
            }                
        }
    }
}
