using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Samples.Helpers
{
    /// <summary>
    /// Helper methods for building URLs to VSTS organization ("account") resources using just a name or ID.
    /// </summmary> 
    public static class OrganizationUrlHelpers
    {
        /// <summary>
        /// Gets the connection URL for the specified VSTS organization name.
        /// </summary>
        public static async Task<Uri> GetUrl(string organizationName)
        {
            string requestUrl = $"{s_locationServiceUrl}/_apis/resourceAreas/{s_defaultResourceAreaId}/?accountName={organizationName}&api-version=5.0-preview.1";

            HttpResponseMessage response = await s_client.GetAsync(requestUrl);
            
            return await ExtractLocationUrl(response);
        }

        /// <summary>
        /// Gets the connection URL for the specified VSTS organization ID.
        /// </summary>
        public static async Task<Uri> GetUrl(Guid organizationId)
        {
            string requestUrl = $"{s_locationServiceUrl}/_apis/resourceAreas/{s_defaultResourceAreaId}/?hostid={organizationId}&api-version=5.0-preview.1";

            HttpResponseMessage response = await s_client.GetAsync(requestUrl);
            
            return await ExtractLocationUrl(response);
        }

        private static async Task<Uri> ExtractLocationUrl(HttpResponseMessage response)
        {            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resourceArea = await response.Content.ReadAsAsync<JObject>();

                if (resourceArea != null)
                {
                    return new Uri(resourceArea["locationUrl"].ToString());
                }
            }

            return null;
        }

        private static readonly HttpClient s_client = new HttpClient();
        private static readonly string s_locationServiceUrl = "https://app.vssps.visualstudio.com";
        private static readonly Guid s_defaultResourceAreaId = Guid.Parse("79134C72-4A58-4B42-976C-04E7115F32BF");
    }
}
