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
            // Create instance of VssConnection using passed credentials
            VssConnection connection = new VssConnection(_uri, _credentials);
            ProjectCollectionHttpClient projectCollectionHttpClient = connection.GetClient<ProjectCollectionHttpClient>();
            IEnumerable<TeamProjectCollectionReference> teamProjectCollectionReference = projectCollectionHttpClient.GetProjectCollections(null).Result;
            return teamProjectCollectionReference;
        }

        public TeamProjectCollectionReference GetProjectCollection(string id)
        {
            // Create instance of VssConnection using passed credentials
            VssConnection connection = new VssConnection(_uri, _credentials);
            ProjectCollectionHttpClient projectCollectionHttpClient = connection.GetClient<ProjectCollectionHttpClient>();
            TeamProjectCollectionReference teamProjectCollectionReference = projectCollectionHttpClient.GetProjectCollection(id).Result;
            return teamProjectCollectionReference;
        }
    }
}
