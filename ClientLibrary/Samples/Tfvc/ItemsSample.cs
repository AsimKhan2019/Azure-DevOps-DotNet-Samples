using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.Tfvc
{
    [ClientSample(TfvcConstants.AreaName, "items")]
    public class ItemsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<TfvcItem> ListItems()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            // get just the items in the root of the project
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string scopePath = $"$/{projectName}/";
            List<TfvcItem> items = tfvcClient.GetItemsAsync(scopePath: scopePath, recursionLevel: VersionControlRecursionType.OneLevel).Result;

            foreach (TfvcItem item in items)
            {
                Console.WriteLine("{0}    {1}   #{3}   {2}", item.ChangeDate, item.IsFolder ? "<DIR>" : "     ", item.Path, item.ChangesetVersion);
            }

            if (items.Count() == 0)
            {
                Console.WriteLine("No items found.");
            }

            return items;
        }

        [ClientSampleMethod]
        public TfvcItem DownloadItem()
        {
            VssConnection connection = this.Context.Connection;
            TfvcHttpClient tfvcClient = connection.GetClient<TfvcHttpClient>();

            // get the items in the root of the project
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;
            string scopePath = $"$/{projectName}/";
            List<TfvcItem> items = tfvcClient.GetItemsAsync(scopePath: scopePath, recursionLevel: VersionControlRecursionType.OneLevel).Result;

            foreach (TfvcItem item in items)
            {
                if (!item.IsFolder)
                {
                    Console.WriteLine("You can download file contents for {0} at {1}", item.Path, item.Url);
                    return item;
                }
            }

            Console.WriteLine("No files found in the root.");
            return null;
        }
    }
}
