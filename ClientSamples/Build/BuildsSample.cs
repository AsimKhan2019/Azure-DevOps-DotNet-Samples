using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsSamples.Client.Build
{
    [ClientSample]
    public class BuildsSample : ClientSample
    {
        public BuildsSample(ClientSampleConfiguration configuration) : base(configuration)
        {
        }

        [ClientSampleMethod]
        public IEnumerable<BuildDefinitionReference> ListBuildDefinitions(string projectName = null)
        {
            VssConnection connection = this.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            return buildClient.GetDefinitionsAsync2(project: projectName).Result;
        }
    }
}
