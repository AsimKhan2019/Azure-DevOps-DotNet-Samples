using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class Batch
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Batch(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        // <summary>
        // Create work item and link the multiple work items
        // </summary>
        // <param name="projectName"></param>
        // <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemBatchPostResponse CreateAndLinkMultipleWorkItems(string projectName)
        {
            WorkItemBatchPost.BatchRequest[] batchRequests = new WorkItemBatchPost.BatchRequest[2];
            Dictionary<string, string> headers = new Dictionary<string, string>() {
                { "Content-Type", "application/json-patch+json" }
            };

            Object[] parentPatchDocumentBody = new Object[2];
            parentPatchDocumentBody[0] = new { op = "add", path = "/fields/System.Title", value = "Customer can sign in using their Microsoft Account" };
            parentPatchDocumentBody[1] = new { op = "add", path = "/id", value = "-1" };
            batchRequests[0] = new WorkItemBatchPost.BatchRequest {
                                    method = "PATCH",
                                    uri = '/' + projectName + "/_apis/wit/workitems/$User Story?api-version=2.2",
                                    headers = headers,
                                    body = parentPatchDocumentBody
                                };

            Object[] childPatchDocumentBody = new Object[3];
            childPatchDocumentBody[0] = new { op = "add", path = "/fields/System.Title", value = "JavaScript implementation for Microsoft Account" };
            childPatchDocumentBody[1] = new { op = "add", path = "/id", value = "-2" };
            childPatchDocumentBody[2] = new {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "System.LinkTypes.Hierarchy-Reverse",
                    url = _configuration.UriString + "_apis/wit/workitems/-1"
                }
            };

            batchRequests[1] = new WorkItemBatchPost.BatchRequest {
                                    method = "PATCH",
                                    uri = '/' + projectName + "/_apis/wit/workitems/$Task?api-version=2.2",
                                    headers = headers,
                                    body = childPatchDocumentBody
                                };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                                       
                var batchRequest = new StringContent(JsonConvert.SerializeObject(batchRequests), Encoding.UTF8, "application/json");                         
                var method = new HttpMethod("POST");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/$batch?api-version=2.2") { Content = batchRequest };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = response.Content.ReadAsStringAsync();
                    WorkItemBatchPostResponse batchResponse = response.Content.ReadAsAsync<WorkItemBatchPostResponse>().Result;
                    return batchResponse;
                }
            }

            return null;
        }
    }
}
