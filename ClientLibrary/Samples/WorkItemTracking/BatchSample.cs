using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    public class BatchRequest
    {
        public string method { get; set; }
        public Dictionary<string, string> headers { get; set; }
        public object[] body { get; set; }
        public string uri { get; set; }
    }
  
    internal class WorkItemBatchPostResponse
    {
        public int count { get; set; }
        [JsonProperty("value")]
        public List<Value> values { get; set; }

        public class Value
        {
            public int code { get; set; }
            public Dictionary<string, string> headers { get; set; }
            public string body { get; set; }
        }
    }


    public class BatchSample : ClientSample
    {

        public void Run()
        {
            string connectionUrl = "https://dev.azure.com/fabrikam";
            string projectName = "fab-ops";
            string personalAccessToken = "FILLIN";
            string basicAuthHeaderValue = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "pat", personalAccessToken)));

            BatchRequest[] batchRequests = new BatchRequest[2];
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "Content-Type", "application/json-patch+json" }
            };

            Object[] parentPatchDocumentBody = new Object[2];
            parentPatchDocumentBody[0] = new { op = "add", path = "/fields/System.Title", value = "Customer can sign in using their Microsoft Account" };
            parentPatchDocumentBody[1] = new { op = "add", path = "/id", value = "-1" };
            batchRequests[0] = new BatchRequest
            {
                method = "PATCH",
                uri = '/' + projectName + "/_apis/wit/workitems/$User Story?api-version=2.2",
                headers = headers,
                body = parentPatchDocumentBody
            };

            Object[] childPatchDocumentBody = new Object[3];
            childPatchDocumentBody[0] = new { op = "add", path = "/fields/System.Title", value = "JavaScript implementation for Microsoft Account" };
            childPatchDocumentBody[1] = new { op = "add", path = "/id", value = "-2" };
            childPatchDocumentBody[2] = new
            {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "System.LinkTypes.Hierarchy-Reverse",
                    url = connectionUrl + "/_apis/wit/workitems/-1"
                }
            };

            batchRequests[1] = new BatchRequest
            {
                method = "PATCH",
                uri = '/' + projectName + "/_apis/wit/workitems/$Task?api-version=2.2",
                headers = headers,
                body = childPatchDocumentBody
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeaderValue);

                var batchRequest = new StringContent(JsonConvert.SerializeObject(batchRequests), Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");

                // send the request
                var request = new HttpRequestMessage(method, connectionUrl + "/_apis/wit/$batch?api-version=2.2") { Content = batchRequest };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = response.Content.ReadAsStringAsync();
                    WorkItemBatchPostResponse batchResponse = response.Content.ReadAsAsync<WorkItemBatchPostResponse>().Result;

                }
                else
                {
                    // not successful
                }
            }
        }

    }
}
