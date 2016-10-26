using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace VstsClientLibrariesSamples.ProjectsAndTeams
{
    public class Projects
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public Projects(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);            
        }

        public IEnumerable<TeamProjectReference> GetProjects()
        {
            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;
                return projects;
            }
        }

        public TeamProjectReference GetProject(string name)
        {
            // create project object
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(_uri, _credentials))
            {
                TeamProjectReference project = projectHttpClient.GetProject(name).Result;
                return project;               
            }                
        }
    }
}
