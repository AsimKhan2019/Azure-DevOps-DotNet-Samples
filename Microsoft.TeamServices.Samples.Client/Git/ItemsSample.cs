using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "items")]
    public class ItemsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<GitItem> ListItems()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);
            GitRepository repo = GitSampleHelpers.FindAnyRepository(this.Context, project.Id);

            List<GitItem> items = gitClient.GetItemsAsync(repo.Id, scopePath: "/", recursionLevel: VersionControlRecursionType.OneLevel).Result;

            Console.WriteLine("project {0}, repo {1}", project.Name, repo.Name);
            foreach(GitItem item in items)
            {
                Console.WriteLine("{0} {1} {2}", item.GitObjectType, item.ObjectId, item.Path);
            }

            return items;            
        }
    }
}
