using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.ProjectsAndTeams;

namespace VstsRestApiSamples.GettingStarted
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

        public ListofProjectsResponse.Projects InteractiveADAL(string vstsAccountName, string applicationId)
        {
            AuthenticationContext ctx = GetAuthenticationContext(null);
            AuthenticationResult result = null;
            try
            {
                result = ctx.AcquireTokenAsync(VSTSResourceId, applicationId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Auto)).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

            var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return ListProjects(vstsAccountName, bearerAuthHeader);
        }

        //NOTE: If the user is not already logged in, this will cause a web browser prompt to display
        public ListofProjectsResponse.Projects InteractiveADALExchangeGraphTokenForVSTSToken(string vstsAccountName, string applicationId)
        {
            AuthenticationContext ctx = GetAuthenticationContext(null);

            AuthenticationResult result = null;
            try
            {
                result = ctx.AcquireTokenAsync(GraphResourceId, applicationId, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Auto)).Result;

                //The result from the above call is now in the cache and will be used to assist in exchanging for a token given 
                //a different resource ID
                result = ctx.AcquireTokenSilentAsync(VSTSResourceId, applicationId).Result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

            var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return ListProjects(vstsAccountName, bearerAuthHeader);
        }

        public ListofProjectsResponse.Projects NonInteractivePersonalAccessToken(string vstsAccountName, string personalAccessToken)
        {
            // encode our personal access token                   
            string encodedPAT = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));
            var basicAuthHeader = new AuthenticationHeaderValue("Basic", encodedPAT);

            return ListProjects(vstsAccountName, basicAuthHeader);
        }

        public ListofProjectsResponse.Projects DeviceCodeADAL(string vstsAccountName, string applicationId)
        {
            string tenant = GetAccountTenant(vstsAccountName);
            AuthenticationContext ctx = GetAuthenticationContext(tenant);

            AuthenticationResult result = null;
            try
            {
                DeviceCodeResult codeResult = ctx.AcquireDeviceCodeAsync(VSTSResourceId, applicationId).Result;
                Console.WriteLine("You need to sign in.");
                Console.WriteLine("Message: " + codeResult.Message + "\n");
                result = ctx.AcquireTokenByDeviceCodeAsync(codeResult).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return ListProjects(vstsAccountName, bearerAuthHeader);
        }

        private static ListofProjectsResponse.Projects ListProjects(string vstsAccountName, AuthenticationHeaderValue authHeader)
        {
            // create a viewmodel that is a class that represents the returned json response
            ListofProjectsResponse.Projects viewModel = new ListofProjectsResponse.Projects();

            // use the httpclient
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(String.Format("https://{0}.visualstudio.com", vstsAccountName));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = authHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/projects?stateFilter=All&api-version=2.2").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    // set the viewmodel from the content in the response
                    viewModel = response.Content.ReadAsAsync<ListofProjectsResponse.Projects>().Result;
                    // var value = response.Content.ReadAsStringAsync().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        private static string GetAccountTenant(string vstsAccountName)
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
                LogError("Something went wrong retrieving the tenant");
            }

            return tenant;
        }

        private static AuthenticationContext GetAuthenticationContext(string tenant)
        {
            AuthenticationContext ctx = null;
            if (tenant != null)
                ctx = new AuthenticationContext("https://login.microsoftonline.com/" + tenant);
            else
            {
                ctx = new AuthenticationContext("https://login.windows.net/common");
                if (ctx.TokenCache.Count > 0)
                {
                    string homeTenant = ctx.TokenCache.ReadItems().First().TenantId;
                    ctx = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
                }
            }

            return ctx;
        }

        private static void LogError(string errorString)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something went wrong...");
            Console.WriteLine("\t " + errorString);
            Console.ResetColor();
        }
    }
}
