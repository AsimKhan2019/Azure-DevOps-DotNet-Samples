using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class Samples
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Samples(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public string GetWorkItemsByQuery()
        {
            var project = _configuration.Project;
            var path = _configuration.Query;    // path to the query   
                        
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // if you already know the query id, then you can skip this step
                HttpResponseMessage queryHttpResponseMessage = client.GetAsync(project + "/_apis/wit/queries/" + path + "?api-version=2.2").Result;

                if (queryHttpResponseMessage.IsSuccessStatusCode)
                {
                    // bind the response content to the queryResult object
                    QueryResult queryResult = queryHttpResponseMessage.Content.ReadAsAsync<QueryResult>().Result;
                    string queryId = queryResult.id;

                    // using the queryId in the url, we can execute the query
                    HttpResponseMessage httpResponseMessage = client.GetAsync(project + "/_apis/wit/wiql/" + queryId + "?api-version=2.2").Result;

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        WorkItemQueryResult workItemQueryResult = httpResponseMessage.Content.ReadAsAsync<WorkItemQueryResult>().Result;

                        // now that we have a bunch of work items, build a list of id's so we can get details
                        var builder = new System.Text.StringBuilder();
                        foreach (var item in workItemQueryResult.workItems)
                        {
                            builder.Append(item.id.ToString()).Append(",");
                        }
                        
                        // clean up string of id's
                        string ids = builder.ToString().TrimEnd(new char[] { ',' });
                        
                        HttpResponseMessage getWorkItemsHttpResponse = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=System.Id,System.Title,System.State&asOf=" + workItemQueryResult.asOf + "&api-version=2.2").Result;

                        if (getWorkItemsHttpResponse.IsSuccessStatusCode)
                        {
                            var result = getWorkItemsHttpResponse.Content.ReadAsStringAsync().Result;
                            return "success";
                        }                

                        return "failed";
                    }

                    return "failed";
                }

                return "failed";                            
            }
        }

        public string GetWorkItemsByWiql()
        {
            string project = _configuration.Project;
            
            // create wiql object
            var wiql = new
            {
                query = "Select [State], [Title] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] = 'New' " +
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the wiql object into a json string   
                var postValue = new StringContent(JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json"); // mediaType needs to be application/json for a post call

                // set the httpmethod to PPOST
                var method = new HttpMethod("POST");

                // send the request               
                var httpRequestMessage = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/wiql?api-version=2.2") { Content = postValue };
                var httpResponseMessage = client.SendAsync(httpRequestMessage).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    WorkItemQueryResult workItemQueryResult = httpResponseMessage.Content.ReadAsAsync<WorkItemQueryResult>().Result;
                                     
                    // now that we have a bunch of work items, build a list of id's so we can get details
                    var builder = new System.Text.StringBuilder();
                    foreach (var item in workItemQueryResult.workItems)
                    {
                        builder.Append(item.id.ToString()).Append(",");
                    }

                    // clean up string of id's
                    string ids = builder.ToString().TrimEnd(new char[] { ',' });

                    HttpResponseMessage getWorkItemsHttpResponse = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=System.Id,System.Title,System.State&asOf=" + workItemQueryResult.asOf + "&api-version=2.2").Result;

                    if (getWorkItemsHttpResponse.IsSuccessStatusCode)
                    {
                        var result = getWorkItemsHttpResponse.Content.ReadAsStringAsync().Result;
                        return "success";
                    }

                    return "failed";               
                }

                return "failed";                               
            }
        }

        public string CreateBug()
        {
            var projectName = _configuration.Project;
           
            Object[] patchDocument = new Object[4];

            patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = "Authorization Errors" };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.TCM.ReproSteps", value = "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http:// msdn.microsoft.com/en-us/library/live/hh826547.aspx" };
            patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "1" };
            patchDocument[3] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = "2 - High" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Bug?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                patchDocument = null;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    return(msg.ToString());
                }               
            }
        }

        public string CreateBugByPassingRules()
        {
            var projectName = _configuration.Project;

            Object[] patchDocument = new Object[6];

            patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = "Imported bug from my other system (rest api)" };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.TCM.ReproSteps", value = "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http:// msdn.microsoft.com/en-us/library/live/hh826547.aspx" };
            patchDocument[2] = new { op = "add", path = "/fields/System.CreatedBy", value = "Some User" };
            patchDocument[3] = new { op = "add", path = "/fields/System.ChangedBy", value = "Some User" };
            patchDocument[4] = new { op = "add", path = "/fields/System.CreatedDate", value = "4/15/2016" };
            patchDocument[5] = new { op = "add", path = "/fields/System.History", value = "Data imported from source" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Bug?bypassRules=true&api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                patchDocument = null;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    return (msg.ToString());
                }
            }
        }

        public string UpdateBug()
        {
            string _id = _configuration.WorkItemId;

            Object[] patchDocument = new Object[3];

            // change some values on a few fields
            patchDocument[0] = new { op = "add", path = "/fields/System.History", value = "Tracking that we changed the priority and severity of this bug to high" };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "1" };
            patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = "1 - Critical" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + _id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                }

                return "success";
            }
        }

        public string AddLinkToBug()
        {
            string _id = _configuration.WorkItemId;
            string _linkToId = _configuration.WorkItemIds.Split(',')[0];

            Object[] patchDocument = new Object[1];

            // change some values on a few fields
            patchDocument[0] = new {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "System.LinkTypes.Dependency-forward",
                    url = _configuration.UriString + "/_apis/wit/workitems/" + _linkToId,
                    attributes = new
                    {
                        comment = "Making a new link for the dependency"
                    }
                }
            };
        
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + _id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                }

                return "success";
            }
        }

        public string AddAttachmentToBug()
        {
            string _id = _configuration.WorkItemId;
            string _filePath = _configuration.FilePath;

            // get the file name from the full path
            String[] breakApart = _filePath.Split('\\');
            int length = breakApart.Length;
            string fileName = breakApart[length - 1];

            Byte[] bytes = System.IO.File.ReadAllBytes(@_filePath);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                ByteArrayContent content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                HttpResponseMessage uploadResponse = client.PostAsync("_apis/wit/attachments?fileName=" + fileName + "&api-version=2.2", content).Result;

                if (uploadResponse.IsSuccessStatusCode)
                {
                    var attachmentReference = uploadResponse.Content.ReadAsAsync<AttachmentReference>().Result;

                    Object[] patchDocument = new Object[1];

                    // add required attachment values
                    patchDocument[0] = new
                    {
                        op = "add",
                        path = "/relations/-",
                        value = new
                        {
                            rel = "AttachedFile",
                            url = attachmentReference.url, // url from uploadresult
                            attributes = new
                            {
                                comment = "adding attachment to bug"
                            }
                        }
                    };

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                    // serialize the fields array into a json string          
                    var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                    // set the httpmethod to Patch
                    var method = new HttpMethod("PATCH");

                    // send the request
                    var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + _id + "?api-version=2.2") { Content = patchValue };
                    var response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                        Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                        return (msg.ToString());
                    }

                    return "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = uploadResponse.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    return (msg.ToString());
                }  
            }
        }

        public string AddCommentToBug()
        {
            string _id = _configuration.WorkItemId;

            Object[] patchDocument = new Object[1];

            // change some values on a few fields
            patchDocument[0] = new { op = "add", path = "/fields/System.History", value = "Adding 'hello world' comment to this bug" };
          
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + _id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                }

                return "success";
            }
        }
    }

    public class QueryResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }           
        public string url { get; set; }
    }
    public class WorkItemQueryResult 
    {
        public string queryType { get; set; }
        public string queryResultType { get; set; }
        public DateTime asOf { get; set; }
        public Column[] columns { get; set; }
        public Workitem[] workItems { get; set; }
    }   
    public class Workitem
    {
        public int id { get; set; }
        public string url { get; set; }
    }
    public class Column
    {
        public string referenceName { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class AttachmentReference
    {
        public string id { get; set; }
        public string url { get; set; }
    }
}
