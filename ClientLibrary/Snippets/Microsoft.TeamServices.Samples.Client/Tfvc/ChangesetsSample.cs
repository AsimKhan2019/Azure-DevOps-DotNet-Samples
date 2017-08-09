using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client.Tfvc
{
    [ClientSample(TfvcConstants.AreaName, "changesets")]
    public class ChangesetsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<TfvcChangesetRef> ListChangesets()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            IEnumerable<TfvcChangesetRef> changesets = tfvcClient.GetChangesetsAsync(top: 10).Result;

            foreach (TfvcChangesetRef changeset in changesets)
            {
                Console.WriteLine("{0} by {1}: {2}", changeset.ChangesetId, changeset.Author.DisplayName, changeset.Comment ?? "<no comment>");
            }

            return changesets;
        }
    }
}
