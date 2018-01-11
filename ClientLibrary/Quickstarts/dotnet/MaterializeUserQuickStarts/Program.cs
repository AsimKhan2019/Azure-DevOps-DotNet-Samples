using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using AadAuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;

namespace MaterializeUserQuickStarts
{
    class Program
    {

        const string userName = "user@domain.com";
        const string password = "Password";
        const string accountName = "VSTSACCOUNT";

        static void Main(string[] args)
        {
            var authenticationContext = new AadAuthenticationContext("https://login.windows.net/common", validateAuthority: true);
            var authenticationResultTask = authenticationContext.AcquireTokenAsync(
                "https://graph.windows.net",
                "872cd9fa-d31f-45e0-9eab-6e460a02d1f1",
                new UserPasswordCredential(userName, password));
            var authenticationResult = authenticationResultTask.Result;
            var bearerAuthHeader = new AuthenticationHeaderValue("Bearer", authenticationResultTask.Result.AccessToken);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(String.Format("https://{0}.vssps.visualstudio.com/", accountName)); //NOTE this is the SPS endpoint for the account
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
                client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                client.DefaultRequestHeaders.Authorization = bearerAuthHeader;

                // connect to the REST endpoint            
                HttpResponseMessage response = client.GetAsync("_apis/connectionData").Result;

                // check to see if we have a succesfull respond
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\tSuccesfully materizlized the user");
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
    }
}
