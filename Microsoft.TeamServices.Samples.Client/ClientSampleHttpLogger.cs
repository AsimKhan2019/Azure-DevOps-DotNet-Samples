using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamServices.Samples.Client
{
    public class ClientSampleHttpLogger : DelegatingHandler
    {
        public static readonly string PropertyOutputFilePath = "$outputFilePath";   // value is a string indicating the folder to output files to
        public static readonly string PropertySuppressOutput = "$suppressOutput";   // value is a boolan indicating whether to suppress output
        //public static readonly string PropertyOutputToConsole = "$outputToConsole"; // value is a boolan indicating whether to output JSON to the console
        public static readonly string PropertyOperationName = "$operationName";   // value is a string indicating the logical name of the operation. If output is enabled, this value is used to produce the output file name.

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
            "x-Powered-By",
            "cookie"
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

            RunnableClientSampleMethod runnableMethod = ClientSampleContext.CurrentRunnableMethod;

            if (runnableMethod != null)
            {
                bool suppressOutput;
                if (!ClientSampleContext.CurrentContext.TryGetValue<bool>(PropertySuppressOutput, out suppressOutput))
                {
                    suppressOutput = false;
                }

                string operationName;
                if (!ClientSampleContext.CurrentContext.TryGetValue<string>(PropertyOperationName, out operationName))
                {
                    operationName = ClientSampleContext.CurrentRunnableMethod.MethodBase.Name;
                }
                else
                {
                    // TODO: add validation around the operation name
                }

                if (!suppressOutput)
                {
                    DirectoryInfo baseOutputPath;
                    if (ClientSampleContext.CurrentContext.TryGetValue<DirectoryInfo>(PropertyOutputFilePath, out baseOutputPath))
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

                        JObject requestBody = null;
                        try
                        {
                            string requestBodyString = await request.Content.ReadAsStringAsync();
                            if (!String.IsNullOrEmpty(requestBodyString))
                            {
                                requestBody = JObject.Parse(requestBodyString);
                            }
                        }
                        catch (Exception) { }

                        JObject responseBody = null;
                        try
                        {
                            if (IsJsonResponse(response))
                            {
                                string responseBodyString = await response.Content.ReadAsStringAsync();
                                responseBody = JObject.Parse(responseBodyString);
                            }
                        }
                        catch (Exception) { }

                        ApiRequestResponseMetdata data = new ApiRequestResponseMetdata()
                        {
                            Area = runnableMethod.Area,
                            Resource = runnableMethod.Resource,
                            HttpMethod = request.Method.ToString().ToUpperInvariant(),
                            RequestUrl = request.RequestUri.ToString(),
                            RequestHeaders = requestHeaders,
                            RequestBody = requestBody,
                            StatusCode = (int)response.StatusCode,
                            ResponseHeaders = responseHeaders,
                            ResponseBody = responseBody
                        };

                        string outputPath = Path.Combine(baseOutputPath.FullName, data.Area, data.Resource);
                        string outputFileName = operationName + ".json";

                        DirectoryInfo outputDirectory = Directory.CreateDirectory(outputPath);

                        string outputFile = Path.Combine(outputDirectory.FullName, outputFileName);

                        string output = JsonConvert.SerializeObject(data, this.serializerSettings);

                        File.WriteAllText(outputFile, output);
                    }
                }
            }

            return response;
        }

        private static bool ResponseHasContent(HttpResponseMessage response)
        {
            if (response != null &&
                response.StatusCode != HttpStatusCode.NoContent &&
                response.Content != null &&
                response.Content.Headers != null &&
                (!response.Content.Headers.ContentLength.HasValue ||
                 (response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength != 0)))
            {
                return true;
            }

            return false;
        }

        private static bool IsJsonResponse(HttpResponseMessage response)
        {
            if (ResponseHasContent(response)
                && response.Content.Headers != null && response.Content.Headers.ContentType != null
                && !String.IsNullOrEmpty(response.Content.Headers.ContentType.MediaType))
            {
                return (0 == String.Compare("application/json", response.Content.Headers.ContentType.MediaType, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public static void SetSuppressOutput(ClientSampleContext context, bool suppress)
        {
            context.SetValue<bool>(PropertySuppressOutput, suppress);
        }

        public static void SetOperationName(ClientSampleContext context, string name)
        {
            context.SetValue<string>(PropertyOperationName, name);
        }

        public static void ResetOperationName(ClientSampleContext context)
        {
            context.RemoveValue(PropertyOperationName);
        }
    }

    public class ClientSampleHttpLoggerOutputSuppression : IDisposable
    {
        public ClientSampleHttpLoggerOutputSuppression()
        {
            ClientSampleHttpLogger.SetSuppressOutput(ClientSampleContext.CurrentContext, true);
        }

        public void Dispose()
        {
            ClientSampleHttpLogger.SetSuppressOutput(ClientSampleContext.CurrentContext, false);
        }
    }

    [DataContract]
    class ApiRequestResponseMetdata : ClientSampleMethodInfo
    {
        [DataMember(Name = "method")]
        public String HttpMethod;

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
