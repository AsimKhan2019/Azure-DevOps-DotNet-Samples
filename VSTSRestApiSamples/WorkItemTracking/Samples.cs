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
            var path = _configuration.Query;    //path to the query   
                        
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                //if you already know the query id, then you can skip this step
                HttpResponseMessage queryHttpResponseMessage = client.GetAsync(project + "/_apis/wit/queries/" + path + "?api-version=2.2").Result;

                if (queryHttpResponseMessage.IsSuccessStatusCode)
                {
                    //bind the response content to the queryResult object
                    QueryResult queryResult = queryHttpResponseMessage.Content.ReadAsAsync<QueryResult>().Result;
                    string queryId = queryResult.id;

                    //using the queryId in the url, we can execute the query
                    HttpResponseMessage httpResponseMessage = client.GetAsync(project + "/_apis/wit/wiql/" + queryId + "?api-version=2.2").Result;

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        QueryWorkItemsResult queryWorkItemsResult = httpResponseMessage.Content.ReadAsAsync<QueryWorkItemsResult>().Result;

                        //now that I have a bunch of work item id's, i can get the individual work items
                        foreach (var item in queryWorkItemsResult.workItems)
                        {
                            HttpResponseMessage getWorkItemHttpResponse = client.GetAsync("_apis/wit/workitems/" + item.id + "?$expand=all&api-version=1.0").Result;

                            if (getWorkItemHttpResponse.IsSuccessStatusCode)
                            {
                                var result = getWorkItemHttpResponse.Content.ReadAsStringAsync().Result;
                                return "success";
                            }

                            return "failed";
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
            
            //create wiql object
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

                //serialize the wiql object into a json string   
                var postValue = new StringContent(JsonConvert.SerializeObject(wiql), Encoding.UTF8, "application/json"); //mediaType needs to be application/json for a post call

                //set the httpmethod to PPOST
                var method = new HttpMethod("POST");

                //send the request               
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/wiql?api-version=2.2") { Content = postValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    QueryWorkItemsResult queryWorkItemsResult = response.Content.ReadAsAsync<QueryWorkItemsResult>().Result;

                    //now that I have a bunch of work item id's, i can get the individual work items
                    foreach(var item in queryWorkItemsResult.workItems)
                    {
                        HttpResponseMessage getWorkItemHttpResponse = client.GetAsync("_apis/wit/workitems/" + item.id + "?$expand=all&api-version=1.0").Result;

                        if (getWorkItemHttpResponse.IsSuccessStatusCode)
                        {
                            var result = getWorkItemHttpResponse.Content.ReadAsStringAsync().Result;
                            return "success";
                        }

                        return "failed";
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
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.TCM.ReproSteps", value = "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http://msdn.microsoft.com/en-us/library/live/hh826547.aspx" };
            patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "1" };
            patchDocument[3] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = "2 - High" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                //send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Bug?api-version=1.0") { Content = patchValue };
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

        public string UpdateBug()
        {
            string _id = _configuration.WorkItemId;

            Object[] patchDocument = new Object[3];

            //change some values on a few fields
            patchDocument[0] = new { op = "add", path = "/fields/System.History", value = "Tracking that we changed the priority and severity of this bug to high" };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "1" };
            patchDocument[2] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = "1 - Critical" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                //send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + _id + "?api-version=1.0") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                }

                return "success";
            }
        }

        public string AddCommentToBug()
        {
            string _id = _configuration.WorkItemId;

            Object[] patchDocument = new Object[1];

            //change some values on a few fields
            patchDocument[0] = new { op = "add", path = "/fields/System.History", value = "Adding 'hello world' comment to this bug" };
          
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                //send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + _id + "?api-version=1.0") { Content = patchValue };
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

    public class QueryWorkItemsResult 
    {
        public string queryType { get; set; }
        public string queryResultType { get; set; }
        public DateTime asOf { get; set; }       
        public Workitem[] workItems { get; set; }
    }   

public class Workitem
{
    public int id { get; set; }
    public string url { get; set; }
}
}
