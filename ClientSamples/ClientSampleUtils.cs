using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;

namespace VstsSamples.Client
{

    /// <summary>
    /// Utilities for discovering client samples.
    /// </summary>
    public static class ClientSampleUtils
    {
        public static Dictionary<ClientSample, IEnumerable<IClientSampleMethod>> GetClientSampleMethods(string area = null, string resource = null)
        {
            List<IClientSampleMethod> methods = new List<IClientSampleMethod>();

            CompositionContainer container = new CompositionContainer(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            IEnumerable<Lazy<ClientSample>> samples = container.GetExports<ClientSample>();

            foreach (Lazy<ClientSample> cs in samples)
            {
                Type csType = cs.Value.GetType();

                ClientSampleAttribute csAttr = csType.GetCustomAttribute<ClientSampleAttribute>();

                foreach (MethodInfo m in csType.GetMethods())
                {
                    ClientSampleMethodAttribute[] attrs = (ClientSampleMethodAttribute[])m.GetCustomAttributes(typeof(ClientSampleMethodAttribute), false);
                    foreach (var ma in attrs)
                    {
                        if (string.IsNullOrEmpty(ma.Area))
                        {
                            ma.Area = csAttr.Area;
                        }

                        if (string.IsNullOrEmpty(ma.Resource))
                        {
                            ma.Resource = csAttr.Resource;
                        }

                        if (!string.IsNullOrEmpty(ma.Area) && !string.IsNullOrEmpty(ma.Resource) && !string.IsNullOrEmpty(ma.Operation))
                        {
                            methods.Add(ma);
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(area))
            {
                methods = methods.FindAll(csm => { return String.Equals(area, csm.Area, StringComparison.OrdinalIgnoreCase); });
            }

            return methods;
        }
    }

    public class ClientSampleInstanceWithMethods
    {
        public ClientSample ClientSample { get; set; }

        public IEnumerable<Method>

    }


    public class ClientSampleHttpLogger : DelegatingHandler
    {
        private JsonSerializerSettings serializerSettings;

        private static HashSet<string> s_excludedHeaders = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "x-VSS-PerfData",
            "x-TFS-Session",
            "x-VSS-E2EID",
             "x-VSS-Agent",
             "authorization",
             "x-TFS-ProcessId",
             "x-VSS-UserData",
             "activityId",
             "p3P",
             "x-Powered-By"
        };
 
        public ClientSampleHttpLogger()
        {
            serializerSettings = new JsonSerializerSettings();

            serializerSettings.Formatting = Formatting.Indented;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            ClientSampleMethodInfo sampleMethodInfo = null;//FindCallingSampleMethod();

            if (sampleMethodInfo != null)
            {
                Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
                foreach (var h in request.Headers.Where(kvp => { return !s_excludedHeaders.Contains(kvp.Key); }))
                {
                    requestHeaders[h.Key] = h.Value.First();
                }

                Dictionary<string, string> responseHeaders = new Dictionary<string, string>();

                foreach (var h in response.Headers.Where(kvp => { return !s_excludedHeaders.Contains(kvp.Key); }))
                {
                    responseHeaders[h.Key] = h.Value.First();
                }

                Object requestBody = null;
                try
                {
                    requestBody = await request.Content.ReadAsAsync(typeof(JObject));
                }
                catch (Exception) { }

                Object responseBody = null;
                try
                {
                    //  responseBody = await response.Content.ReadAsAsync(typeof(JObject));
                }
                catch (Exception) { }

                ApiRequestResponseMetdata data = new ApiRequestResponseMetdata()
                {
                    Area = sampleMethodInfo.Area,
                    Resource = sampleMethodInfo.Resource,
                    Method = request.Method.ToString().ToUpperInvariant(),
                    RequestUrl = request.RequestUri.ToString(),
                    RequestHeaders = requestHeaders,
                    RequestBody = requestBody,
                    StatusCode = (int)response.StatusCode,
                    ResponseHeaders = responseHeaders,
                    ResponseBody = responseBody
                };

                String output = JsonConvert.SerializeObject(data, this.serializerSettings);

                Console.WriteLine(output);
            }
          
            return response;
        }
    }
    
    [DataContract]
    class ApiRequestResponseMetdata : ClientSampleMethodInfo
    {
        [DataMember]
        public String Method;

        [DataMember]
        public String RequestUrl;

        [DataMember]
        public Dictionary<String, String> RequestHeaders;

        [DataMember(EmitDefaultValue = false)]
        public Object RequestBody;

        [DataMember]
        public int StatusCode;

        [DataMember]
        public Dictionary<String, String> ResponseHeaders;

        [DataMember(EmitDefaultValue = false)]
        public Object ResponseBody;

      
    } 
    
}
