using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.Security
{
    [ClientSample(LocationResourceIds.SecurityServiceArea, "accesscontrollists")]
    public class AccessControlListsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<AccessControlList> ListAllGitAcls()
        {
            VssConnection connection = this.Context.Connection;
            SecurityHttpClient securityClient = connection.GetClient<SecurityHttpClient>();

            IEnumerable<AccessControlList> acls = securityClient.QueryAccessControlListsAsync(
                // in a real app, you should get this value via securityClient.QuerySecurityNamespacesAsync
                GitSecurityNamespace,
                string.Empty,
                descriptors: null,
                includeExtendedInfo: false,
                recurse: true).Result;

            Console.WriteLine("token | inherit? | count of ACEs");
            Console.WriteLine("------+----------+--------------");
            foreach (AccessControlList acl in acls)
            {
                Console.WriteLine("{0} | {1} | {2} ACEs", acl.Token, acl.InheritPermissions, acl.AcesDictionary.Count());
            }

            return acls;
        }

        private Guid GitSecurityNamespace = Guid.Parse("2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87");
    }
}
