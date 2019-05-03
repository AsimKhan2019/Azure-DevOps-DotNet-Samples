using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.ExtensionManagement
{
    /// <summary>
    /// 
    /// Samples showing how to manage installed extensions. 
    /// 
    /// For more details, see https://docs.microsoft.com/vsts/marketplace/install-vsts-extension
    /// 
    /// </summary>
    [ClientSample(ExtensionResourceIds.ExtensionsArea, ExtensionResourceIds.InstalledExtensionsByNameResouceName)]
    public class InstalledExtensionsSample : ClientSample
    {
        /// <summary>
        ///  List all installed extensions, including built-in extensions.
        /// </summary>
        [ClientSampleMethod]
        public IEnumerable<InstalledExtension> ListInstalledExtensions()
        {
            // Get the client
            VssConnection connection = Context.Connection;
            ExtensionManagementHttpClient extensionManagementClient = connection.GetClient<ExtensionManagementHttpClient>();

            // Get the list of installed extensions
            List<InstalledExtension> extensions = extensionManagementClient.GetInstalledExtensionsAsync().Result;

            foreach (InstalledExtension extension in extensions)
            {
                LogExtension(extension);
            }

            return extensions;
        }

        /// <summary>
        /// Install the Contributions Guide sample extension.
        /// </summary>
        [ClientSampleMethod]
        public InstalledExtension InstallSampleExtension()
        {
            // Identifiers of the extension to install
            string publisherName = "ms-samples";
            string extensionName = "samples-contributions-guide";

            // Get the client
            VssConnection connection = Context.Connection;
            ExtensionManagementHttpClient extensionManagementClient = connection.GetClient<ExtensionManagementHttpClient>();

            InstalledExtension extension = null;

            // Try to install the extension
            try
            {
                Context.Log("Trying to install extension {0}.{1}...", publisherName, extensionName);

                extension = extensionManagementClient.InstallExtensionByNameAsync(publisherName, extensionName).Result;

                Context.Log("Extension installed successfully!");
            }
            catch (Exception ex)
            {
                // Unable to install the extension
                Context.Log("Unable to install the extension. Error: {0}", ex.Message);
            }

            if (extension != null)
            {
                LogExtension(extension);
            }

            return extension;
        }

        /// <summary>
        /// Get the Contributions Guide sample extension.
        /// </summary>
        [ClientSampleMethod]
        public InstalledExtension GetInstalledSampleExtension()
        {
            // Identifiers of the extension to uninstall
            string publisherName = "ms-samples";
            string extensionName = "samples-contributions-guide";

            // Get the client
            VssConnection connection = Context.Connection;
            ExtensionManagementHttpClient extensionManagementClient = connection.GetClient<ExtensionManagementHttpClient>();

            // Try to get the extension
            try
            {
                InstalledExtension extension = extensionManagementClient.GetInstalledExtensionByNameAsync(publisherName, extensionName).Result;
                LogExtension(extension);
                return extension;
            }
            catch (Exception ex)
            {
                Context.Log("Unable to get the extension: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Uninstall the Contributions Guide sample extension.
        /// </summary>
        [ClientSampleMethod]
        public bool UninstallSampleExtension()
        {
            // Identifiers of the extension to uninstall
            string publisherName = "ms-samples";
            string extensionName = "samples-contributions-guide";

            // Get the client
            VssConnection connection = Context.Connection;
            ExtensionManagementHttpClient extensionManagementClient = connection.GetClient<ExtensionManagementHttpClient>();

            // Try to uninstall the extension
            try
            {
                extensionManagementClient.UninstallExtensionByNameAsync(publisherName, extensionName, "Just testing").SyncResult();
                Context.Log("Successfully uninstalled the extension.");
                return true;
            }
            catch (Exception ex)
            {
                Context.Log("Unable to uninstall the extension: {0}", ex.Message);
                return false;
            }
        }

        protected void LogExtension(InstalledExtension extension)
        {
            Context.Log(" {0}.{1}{2}: {3} by {4}",
                extension.PublisherName,
                extension.ExtensionName,
                (extension.Flags.HasFlag(ExtensionFlags.BuiltIn) ? "*" : ""),
                extension.ExtensionDisplayName,
                extension.PublisherDisplayName);
        }

    }

}
