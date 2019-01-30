using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamServices.Samples.Client.Build;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Build
{
    [ClientSample(BuildResourceIds.AreaName, BuildResourceIds.BuildsResource)]
    public class BuildsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<BuildDefinitionReference> ListBuildDefinitions()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a build client instance
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            List<BuildDefinitionReference> buildDefinitions = new List<BuildDefinitionReference>();

            // Iterate (as needed) to get the full set of build definitions
            string continuationToken = null;
            do
            {
                IPagedList<BuildDefinitionReference> buildDefinitionsPage = buildClient.GetDefinitionsAsync2(
                    project: projectName,
                    continuationToken: continuationToken).Result;

                buildDefinitions.AddRange(buildDefinitionsPage);

                continuationToken = buildDefinitionsPage.ContinuationToken;
            } while (!String.IsNullOrEmpty(continuationToken));

            // Show the build definitions
            foreach (BuildDefinitionReference definition in buildDefinitions)
            {
                Console.WriteLine("{0} {1}", definition.Id.ToString().PadLeft(6), definition.Name);
            }

            return buildDefinitions;
        }

        [ClientSampleMethod]
        public BuildArtifact CreateArtifact()
        {
            BuildArtifact result = new BuildArtifact();
            // Get a build client instance
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            ArtifactResource newArtifactResource = new ArtifactResource()
            {
                Type = "new ArtifactResource type",
                Data = "new ArtifactResource Data",
                Properties = null,
                Url = "new Url",
                DownloadUrl = "new DownloadUrl",
                DownloadTicket = "new downloadticket"
            };


            BuildArtifact newArtifact = new BuildArtifact()
            {
                Id = 900,
                Name = "New test artifact",
                Resource = newArtifactResource
            };

            BuildArtifact newArtifact2 = new BuildArtifact()
            {
                Id = 901,
                Name = "New test artifact 2",
                Resource = newArtifactResource
            };

            try
            {
                result = buildClient.CreateArtifactAsync(newArtifact, 1).Result;
                result = buildClient.CreateArtifactAsync(newArtifact2, 1).Result;

                Console.WriteLine("success");
                Console.WriteLine("{0}", newArtifact.Name);
            }
            catch(Exception ex)
            {
                Console.WriteLine("failed");
                Console.WriteLine("Error creating artifact: " + ex.InnerException.Message);
            }
            

            return result;
        }

        [ClientSampleMethod]
        public List<BuildArtifact> GetAllArtifacts()
        {
            // Get a build client instance
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            List<BuildArtifact> artifactsList = buildClient.GetArtifactsAsync(1).Result;

            return artifactsList;
        }

        [ClientSampleMethod]
        public BuildArtifact GetArtifact()
        {
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            BuildArtifact result = buildClient.GetArtifactAsync("Test Project", 1, "New test artifact").Result;

            return result;
        }

        [ClientSampleMethod]
        public void GetFile()
        {
            System.Diagnostics.Debugger.Launch();
            VssConnection connection = Context.Connection;
            BuildHttpClient buildClient = connection.GetClient<BuildHttpClient>();

            //int buildId, string artifactName, string fileId, string fileName
            try
            {
                var result = buildClient.GetFileAsync(1, "New test artifact", "1", "test file").Result;
                Console.WriteLine("Get file successed");
            }
            catch(Exception e)
            {
                Console.WriteLine("Get File failed: " + e.Message);
            }
        }
    }
}
