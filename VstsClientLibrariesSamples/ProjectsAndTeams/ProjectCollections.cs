using System;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using System.Collections.Generic;

namespace VstsClientLibrariesSamples.ProjectsAndTeams
{
    public class ProjectCollections
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public ProjectCollections(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public IEnumerable<TeamProjectCollectionReference> GetProjectCollections()
        {         
            using (ProjectCollectionHttpClient projectCollectionHttpClient = new ProjectCollectionHttpClient(_uri, _credentials))
            {
                IEnumerable<TeamProjectCollectionReference> teamProjectCollectionReference = projectCollectionHttpClient.GetProjectCollections(null).Result;
                return teamProjectCollectionReference;
            }
        }

        public TeamProjectCollectionReference GetProjectCollection(string id)
        {
            using (ProjectCollectionHttpClient projectCollectionHttpClient = new ProjectCollectionHttpClient(_uri, _credentials))
            {
                TeamProjectCollectionReference teamProjectCollectionReference = projectCollectionHttpClient.GetProjectCollection(id).Result;
                return teamProjectCollectionReference;
            }
        }


    }
}
