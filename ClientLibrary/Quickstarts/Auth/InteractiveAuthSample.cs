using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AadAuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;

namespace Microsoft.TeamServices.Samples.Client.Auth
{
    public class InteractiveAuthSample
    {
        // This is the hard coded Resource ID for Visual Studio Team Services, do not change this value
        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798";

        // This is the hard coded Resource ID for Graph, do not change this value
        internal const string GraphResourceId = "https://graph.windows.net";

        // Redirect URI default value for native/mobile apps
        // https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-protocols-oauth-code
        internal const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";

        public InteractiveAuthSample()
        {
        }

        public IEnumerable<TeamProjectReference> InteractiveADAL(string vstsAccountName, string applicationId)
        {
            AuthenticationContext authenticationContext = new AadAuthenticationContext("https://login.windows.net/common", validateAuthority: true);
            var authenticationResultTask = authenticationContext.AcquireTokenAsync(VSTSResourceId, applicationId, new Uri(RedirectUri), new PlatformParameters(PromptBehavior.Auto)); 
            AuthenticationResult authenticationResult = authenticationResultTask.Result;

            VssOAuthAccessTokenCredential oAuthCredential = new VssOAuthAccessTokenCredential(authenticationResult.AccessToken);

            return ListProjectsViaClientLibrary(vstsAccountName, oAuthCredential);
        }

        public IEnumerable<TeamProjectReference> InteractiveADALExchangeGraphTokenForVSTSToken(string vstsAccountName, string applicationId)
        {
            AuthenticationContext authenticationContext = new AadAuthenticationContext("https://login.windows.net/common", validateAuthority: true);
            var authenticationResultTask = authenticationContext.AcquireTokenAsync(GraphResourceId, applicationId, new Uri(RedirectUri), new PlatformParameters(PromptBehavior.Auto));
            AuthenticationResult authenticationResult = authenticationResultTask.Result;

            authenticationResultTask = authenticationContext.AcquireTokenSilentAsync(VSTSResourceId, applicationId);
            authenticationResult = authenticationResultTask.Result;

            VssOAuthAccessTokenCredential oAuthCredential = new VssOAuthAccessTokenCredential(authenticationResult.AccessToken);

            return ListProjectsViaClientLibrary(vstsAccountName, oAuthCredential);
        }

        public IEnumerable<TeamProjectReference> NonInteractivePersonalAccessToken(string vstsAccountName, string personalAccessToken)
        {
            VssBasicCredential credentials = new VssBasicCredential("", personalAccessToken);

            return ListProjectsViaClientLibrary(vstsAccountName, credentials);
        }

        public IEnumerable<TeamProjectReference> DeviceCodeADAL(string vstsAccountName, string applicationId)
        {
            Guid tenant = GetAccountTenant(vstsAccountName);
            AuthenticationContext authenticationContext = new AadAuthenticationContext("https://login.windows.net/" + tenant, validateAuthority: true);
            DeviceCodeResult codeResult = authenticationContext.AcquireDeviceCodeAsync(VSTSResourceId, applicationId).Result;
            Console.WriteLine("You need to sign in.");
            Console.WriteLine("Message: " + codeResult.Message + "\n");
            AuthenticationResult authenticationResult = authenticationContext.AcquireTokenByDeviceCodeAsync(codeResult).Result;

            VssOAuthAccessTokenCredential oAuthCredential = new VssOAuthAccessTokenCredential(authenticationResult.AccessToken);

            return ListProjectsViaClientLibrary(vstsAccountName, oAuthCredential);
        }

        internal IEnumerable<TeamProjectReference> ListProjectsViaClientLibrary(string vstsAccountName, VssCredentials credentials)
        {
            // Create instance of VssConnection using passed credentials
            VssConnection connection = new VssConnection(new Uri(String.Format("https://{0}.visualstudio.com", vstsAccountName)), credentials);
            ProjectHttpClient projectHttpClient = connection.GetClient<ProjectHttpClient>();

            IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;

            return projects;
        }

        // MSA backed accounts will return Guid.Empty
        private static Guid GetAccountTenant(string vstsAccountName)
        {
            Guid tenantGuid = Guid.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(String.Format("https://{0}.visualstudio.com", vstsAccountName));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "VSTSAuthSample-AuthenticateADALNonInteractive");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                HttpResponseMessage response = client.GetAsync("_apis/connectiondata").Result;

                // Get the tenant from the Login URL
                var wwwAuthenticateHeaderResults = response.Headers.WwwAuthenticate.ToList();
                var bearerResult = wwwAuthenticateHeaderResults.Where(p => p.Scheme == "Bearer");
                foreach (var item in wwwAuthenticateHeaderResults)
                {
                    if (item.Scheme.StartsWith("Bearer"))
                    {
                        tenantGuid = Guid.Parse(item.Parameter.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[2]);
                        break;
                    }
                }
            }

            return tenantGuid;
        }
    }
}