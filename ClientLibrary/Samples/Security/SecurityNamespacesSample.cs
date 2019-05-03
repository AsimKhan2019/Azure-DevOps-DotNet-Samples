using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.Security
{
    [ClientSample(LocationResourceIds.SecurityServiceArea, "securitynamespaces")]
    public class SecurityNamespacesSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<SecurityNamespaceDescription> ListSecurityNamespaces()
        {
            VssConnection connection = this.Context.Connection;
            SecurityHttpClient securityClient = connection.GetClient<SecurityHttpClient>();

            IEnumerable<SecurityNamespaceDescription> namespaces = securityClient.QuerySecurityNamespacesAsync(Guid.Empty).Result;

            Console.WriteLine("Listing all security namespaces");
            foreach (SecurityNamespaceDescription ns in namespaces)
            {
                Console.WriteLine("{0} ({1}) - {2} permissions", ns.DisplayName ?? ns.Name, ns.NamespaceId, ns.Actions.Count());
            }

            return namespaces;
        }

        [ClientSampleMethod]
        public IEnumerable<SecurityNamespaceDescription> ListLocalSecurityNamespaces()
        {
            VssConnection connection = this.Context.Connection;
            SecurityHttpClient securityClient = connection.GetClient<SecurityHttpClient>();

            IEnumerable<SecurityNamespaceDescription> namespaces = securityClient.QuerySecurityNamespacesAsync(Guid.Empty, localOnly: true).Result;

            Console.WriteLine("Listing local security namespaces");
            foreach (SecurityNamespaceDescription ns in namespaces)
            {
                Console.WriteLine("{0} ({1}) - {2} permissions", ns.DisplayName ?? ns.Name, ns.NamespaceId, ns.Actions.Count());
            }

            return namespaces;
        }

        [ClientSampleMethod]
        public SecurityNamespaceDescription GetGitSecurityNamespace()
        {
            VssConnection connection = this.Context.Connection;
            SecurityHttpClient securityClient = connection.GetClient<SecurityHttpClient>();

            IEnumerable<SecurityNamespaceDescription> namespaces = securityClient.QuerySecurityNamespacesAsync(this.GitSecurityNamespace).Result;
            SecurityNamespaceDescription gitNamespace = namespaces.First();

            Console.WriteLine("{0}", gitNamespace.DisplayName);
            foreach (ActionDefinition actionDef in gitNamespace.Actions)
            {
                string knownBit = "";

                if (actionDef.Bit == gitNamespace.ReadPermission)
                {
                    knownBit += " [Read]";
                }
                if (actionDef.Bit == gitNamespace.WritePermission)
                {
                    knownBit += " [Write]";
                }

                Console.WriteLine("\"{0}\" ({1}){2}", actionDef.DisplayName ?? actionDef.Name, actionDef.Bit, knownBit);
            }

            return gitNamespace;
        }

        private Guid GitSecurityNamespace = Guid.Parse("2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87");
    }
}
