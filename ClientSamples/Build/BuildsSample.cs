using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vsts.ClientSamples.Build
{
    [ClientSample(BuildResourceIds.AreaName, BuildResourceIds.BuildsResource)]
    public class BuildsSample : ClientSample
    { 
        [ClientSampleMethod]
        public IEnumerable<BuildDefinitionReference> ListBuildDefinitions()
        {
            string projectName = ClientSampleHelpers.GetDefaultProject(this.Context).Name;

            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            IEnumerable<BuildDefinitionReference> buildDefinitions = buildClient.GetDefinitionsAsync2(project: projectName).Result;

            return buildDefinitions;
        }
    }
}
