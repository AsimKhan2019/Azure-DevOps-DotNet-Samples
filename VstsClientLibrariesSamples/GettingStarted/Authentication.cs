using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AadAuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;

namespace VstsClientLibrariesSamples.GettingStarted
{
    public class Authentication
    {
        // This is the hard coded Resource ID for Visual Studio Team Services, do not change this value
        internal const string VSTSResourceId = "499b84ac-1321-427f-aa17-267ca6975798";

        // This is the hard coded Resource ID for Graph, do not change this value
        internal const string GraphResourceId = "https://graph.windows.net";

        public Authentication()
        {
        }

        public IEnumerable<TeamProjectReference> InteractiveADAL(string vstsAccountName, string applicationId)
        {
            AuthenticationContext authenticationContext = new AadAuthenticationContext("https://login.windows.net/common", validateAuthority: true);
            var authenticationResultTask = authenticationContext.AcquireTokenAsync(VSTSResourceId, applicationId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Never)); 
            AuthenticationResult authenticationResult = authenticationResultTask.Result;

            VssOAuthAccessTokenCredential oAuthCredential = new VssOAuthAccessTokenCredential(authenticationResult.AccessToken);

            return ListProjectsViaClientLibrary(vstsAccountName, oAuthCredential);
        }

        public IEnumerable<TeamProjectReference> InteractiveADALExchangeGraphTokenForVSTSToken(string vstsAccountName, string applicationId)
        {
            AuthenticationContext authenticationContext = new AadAuthenticationContext("https://login.windows.net/common", validateAuthority: true);
            var authenticationResultTask = authenticationContext.AcquireTokenAsync(GraphResourceId, applicationId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Never));
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
            string tenant = GetAccountTenant(vstsAccountName);
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
            Uri uri = new Uri(String.Format("https://{0}.visualstudio.com", vstsAccountName));
            using (ProjectHttpClient projectHttpClient = new ProjectHttpClient(uri, credentials))
            {
                IEnumerable<TeamProjectReference> projects = projectHttpClient.GetProjects().Result;

                if (projects != null)
                {
                    return projects;
                }
                else
                {
                    return null;
                }
            }
        }

        internal static string GetAccountTenant(string vstsAccountName)
        {
            string tenant = null;
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
                        tenant = item.Parameter.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[2];
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(tenant))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong...");
                Console.WriteLine("\t " + "Something went wrong retrieving the tenant");
                Console.ResetColor();
            }

            return tenant;
        }
    }
}