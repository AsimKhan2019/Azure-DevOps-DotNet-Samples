using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vsts.ClientSamples.Build
{
    [ClientSample]
    public class BuildsSample : ClientSample
    {
        public BuildsSample(ClientSampleContext context) : base(context)
        {
        }

        [ClientSampleMethod]
        public IEnumerable<BuildDefinitionReference> ListBuildDefinitions(string projectName = null)
        {
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            return buildClient.GetDefinitionsAsync2(project: projectName).Result;
        }
    }
}
