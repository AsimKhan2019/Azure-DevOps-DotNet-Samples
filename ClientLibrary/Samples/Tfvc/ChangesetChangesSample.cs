using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.Tfvc
{
    [ClientSample(TfvcConstants.AreaName, "changesetchanges")]
    public class ChangesetChangesSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<TfvcChange> GetChangesetChanges()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            TfvcChangesetRef latestChangeset;
            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                latestChangeset = tfvcClient.GetChangesetsAsync(top: 1).Result.First();
            }

            IEnumerable<TfvcChange> changes = tfvcClient.GetChangesetChangesAsync(id: latestChangeset.ChangesetId).Result;

            foreach (TfvcChange change in changes)
            {
                Console.WriteLine("{0}: {1}", change.ChangeType, change.Item.Path);
            }

            return changes;
        }
    }
}
