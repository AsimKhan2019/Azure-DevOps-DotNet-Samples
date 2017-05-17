using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Git
{
    [ClientSample(GitWebApiConstants.AreaName, "refs")]
    public class RefsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<GitRef> ListBranches()
        {
            VssConnection connection = this.Context.Connection;
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            Guid repoId = GitSampleHelpers.FindAnyRepository(this.Context, projectId).Id;

            List<GitRef> refs = gitClient.GetRefsAsync(repoId, filter: "heads/").Result;
            
            foreach(GitRef gitRef in refs)
            {
                Console.WriteLine("{0} {1} {2}", gitRef.Name, gitRef.ObjectId, gitRef.Url);
            }

            return refs;            
        }
    }
}
