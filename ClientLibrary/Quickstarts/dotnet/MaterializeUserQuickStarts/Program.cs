using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MaterializeUserQuickStarts
{
    class Program
    {
        const string userName = "user@domain.com";
        const string password = "P@ssw0rd!"; 
        const string accountName = "account";

        static void Main(string[] args)
        {
            AuthenticationContext ctx = GetAuthenticationContext(null);
            AuthenticationResult result = ctx.AcquireTokenAsync("499b84ac-1321-427f-aa17-267ca6975798", "872cd9fa-d31f-45e0-9eab-6e460a02d1f1", new UserPasswordCredential(userName, password)).Result;
            var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(String.Format("https://{0}.vssps.visualstudio.com/", accountName)); //NOTE this is the SPS endpoint for the account
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "MaterializeUserQuickStarts");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = bearerAuthHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/connectionData").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Successfully materizlized the user");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                }
            }
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
    }
}
