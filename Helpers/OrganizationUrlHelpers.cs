using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;

namespace Samples.Helpers
{
    /// <summary>
    /// Helper methods for building URLs to VSTS organization ("account") resources.
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
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                ResourceAreaInfo value = await response.Content.ReadAsAsync<ResourceAreaInfo>();

                if (value != null)
                {
                    return new Uri(value.LocationUrl);
                }
            };

            throw new OrganizationNotFoundException(organizationName);
        }

        /// <summary>
        /// Gets the connection URL for the specified VSTS organization ID.
        /// </summary>
        public static async Task<Uri> GetUrl(Guid organizationId)
        {
            string requestUrl = $"{s_locationServiceUrl}/_apis/resourceAreas/{s_defaultResourceAreaId}/?hostid={organizationId}&api-version=5.0-preview.1";

            HttpResponseMessage response = await s_client.GetAsync(requestUrl);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                ResourceAreaInfo value = await response.Content.ReadAsAsync<ResourceAreaInfo>();

                if (value != null)
                {
                    return new Uri(value.LocationUrl);
                }
            };

            throw new OrganizationNotFoundException(organizationId.ToString());
        }

        private static readonly HttpClient s_client = new HttpClient();
        private static readonly string s_locationServiceUrl = "https://app.vssps.visualstudio.com";
        private static readonly Guid s_defaultResourceAreaId = Guid.Parse("79134C72-4A58-4B42-976C-04E7115F32BF");
    }
}
