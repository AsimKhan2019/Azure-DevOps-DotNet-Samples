using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.DevOps.ClientSamples.Security
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

            // we'll store one interesting ACE to expand in a later method
            bool storedAcl = false;

            Console.WriteLine("token | inherit? | count of ACEs");
            Console.WriteLine("------+----------+--------------");
            foreach (AccessControlList acl in acls)
            {
                Console.WriteLine("{0} | {1} | {2} ACEs", acl.Token, acl.InheritPermissions, acl.AcesDictionary.Count());

                if (!storedAcl && acl.Token.Length > "repoV2/".Length)
                {
                    this.Context.SetValue(this.StoredAclKey, acl);
                    storedAcl = true;
                }
            }

            return acls;
        }

        [ClientSampleMethod]
        public void ExpandGitAcl()
        {
            AccessControlList acl;
            bool hasInterestingAcl = this.Context.TryGetValue(this.StoredAclKey, out acl);
            if (!hasInterestingAcl)
            {
                Console.WriteLine("no interesting ACLs found");
                return;
            }

            // get the details for Git permissions
            Dictionary<int, string> permission = GetGitPermissionNames();

            // use the Git permissions data to expand the ACL
            Console.WriteLine("Expanding ACL for {0} ({1} ACEs)", acl.Token, acl.AcesDictionary.Count());
            foreach (var kvp in acl.AcesDictionary)
            {
                // in the key-value pair, Key is an identity and Value is an ACE (access control entry)
                // allow and deny are bit flags indicating which permissions are allowed/denied
                Console.WriteLine("Identity {0}");
                Console.WriteLine("  Allowed: {0} (value={1})", GetPermissionString(kvp.Value.Allow, permission), kvp.Value.Allow);
                Console.WriteLine("  Denied: {0} (value={1})", GetPermissionString(kvp.Value.Deny, permission), kvp.Value.Deny);
            }

            return;
        }

        private Dictionary<int, string> GetGitPermissionNames()
        {
            VssConnection connection = this.Context.Connection;
            SecurityHttpClient securityClient = connection.GetClient<SecurityHttpClient>();

            IEnumerable<SecurityNamespaceDescription> namespaces;

            using (new ClientSampleHttpLoggerOutputSuppression())
            {
                namespaces = securityClient.QuerySecurityNamespacesAsync(this.GitSecurityNamespace).Result;
            }
            SecurityNamespaceDescription gitNamespace = namespaces.First();

            Dictionary<int, string> permission = new Dictionary<int, string>();
            foreach (ActionDefinition actionDef in gitNamespace.Actions)
            {
                permission[actionDef.Bit] = actionDef.DisplayName;
            }

            return permission;
        }

        private string GetPermissionString(int bitsSet, Dictionary<int, string> bitMeanings)
        {
            List<string> permissionStrings = new List<string>();
            foreach(var kvp in bitMeanings)
            {
                if ((bitsSet & kvp.Key) == kvp.Key)
                {
                    permissionStrings.Add(kvp.Value);
                }
            }

            string value = string.Join(", ", permissionStrings.ToArray());
            if (string.IsNullOrEmpty(value))
            {
                return "<none>";
            }
            return value;
        }

        private Guid GitSecurityNamespace = Guid.Parse("2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87");
        private string StoredAclKey = "example_git_acl";
    }
}
