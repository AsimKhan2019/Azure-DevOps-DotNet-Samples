using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using Vsts.ClientSamples;

namespace Vsts.ClientSamples.Core
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.ProjectCollectionsResource)]
    public class ProjectCollectionsSample : ClientSample
    {

        [ClientSampleMethod]
        public IEnumerable<TeamProjectCollectionReference> GetProjectCollections()
        {         
            // Create instance of VssConnection using passed credentials
            VssConnection connection = Context.Connection;
            ProjectCollectionHttpClient projectCollectionClient = connection.GetClient<ProjectCollectionHttpClient>();

            IEnumerable<TeamProjectCollectionReference> projectCollections = projectCollectionClient.GetProjectCollections().Result;

            return projectCollections;
        }

        [ClientSampleMethod]
        public TeamProjectCollectionReference GetProjectCollection(string collectionName)
        {
            VssConnection connection = Context.Connection;
            ProjectCollectionHttpClient projectCollectionClient = connection.GetClient<ProjectCollectionHttpClient>();

            TeamProjectCollectionReference teamProjectCollectionReference = projectCollectionClient.GetProjectCollection(collectionName).Result;

            return teamProjectCollectionReference;
        }
    }
}
