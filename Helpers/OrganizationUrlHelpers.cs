using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
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
        public static async Task<Uri> GetConnectionUrl(string organizationName)
        {
            try
            {
                HttpResponseMessage response = await s_client.GetAsync($"https://app.vssps.visualstudio.com/_apis/resourceAreas?accountName={organizationName}&api-version=5.0-preview.1");
            
                var wrapper = await response.Content.ReadAsAsync<VssJsonCollectionWrapper<List<ResourceAreaInfo>>>();

                return new Uri(wrapper.Value.First(resourceArea => resourceArea.Id == s_coreResourceAreaId).LocationUrl);
            }
            catch (Exception ex)
            {
                throw new OrganizationNotFoundException(organizationName, ex);
            }
        }

        /// <summary>
        /// Gets the connection URL for the specified VSTS organization ID.
        /// </summary>
        public static async Task<Uri> GetConnectionUrl(Guid organizationId)
        {
            try
            {
                HttpResponseMessage response = await s_client.GetAsync($"https://app.vssps.visualstudio.com/_apis/resourceAreas?hostId={organizationId}&api-version=5.0-preview.1");
            
                var wrapper = await response.Content.ReadAsAsync<VssJsonCollectionWrapper<List<ResourceAreaInfo>>>();

                return new Uri(wrapper.Value.First(resourceArea => resourceArea.Id == s_coreResourceAreaId).LocationUrl);
            }
            catch (Exception ex)
            {
                throw new OrganizationNotFoundException(organizationId.ToString(), ex);
            }
        }

        private static readonly HttpClient s_client = new HttpClient();

        public static readonly Guid s_coreResourceAreaId = Guid.Parse("79134C72-4A58-4B42-976C-04E7115F32BF");
    }
    
    public class OrganizationNotFoundException : Exception
    {
        public OrganizationNotFoundException(string message, Exception ex)
            : base(message, ex)
        {  
        }
    }

}
