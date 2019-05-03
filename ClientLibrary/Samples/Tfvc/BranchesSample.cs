using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.Tfvc
{
    [ClientSample(TfvcConstants.AreaName, "branches")]
    public class BranchesSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<TfvcBranch> ListBranches()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            IEnumerable<TfvcBranch> branches = tfvcClient.GetBranchesAsync(includeParent: true, includeChildren: true).Result;

            foreach (TfvcBranch branch in branches)
            {
                Console.WriteLine("{0} ({2}): {1}", branch.Path, branch.Description ?? "<no description>", branch.Owner.DisplayName);
            }

            if (branches.Count() == 0)
            {
                Console.WriteLine("No branches found.");
            }

            return branches;
        }
    }
}
