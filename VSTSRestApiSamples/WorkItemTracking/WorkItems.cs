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
    public class WorkItems
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public WorkItems(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }
       
        public GetWorkItemsResponse.WorkItems GetWorkItemsByIDs(string ids)
        {
            GetWorkItemsResponse.WorkItems viewModel = new GetWorkItemsResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&api-version=2.2").Result;
                            
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
            
        public GetWorkItemsResponse.WorkItems GetWorkItemsWithSpecificFields(string ids)
        {
            GetWorkItemsResponse.WorkItems viewModel = new GetWorkItemsResponse.WorkItems();

            // list of fields that i care about
            string fields = "System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Scheduling.RemainingWork";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=" + fields + "&api-version=2.2").Result;
                                
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetWorkItemsResponse.WorkItems GetWorkItemsAsOfDate(string ids, DateTime asOfDate)
        {
            GetWorkItemsResponse.WorkItems viewModel = new GetWorkItemsResponse.WorkItems();

            // list of fields that i care about
            string fields = "System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Scheduling.RemainingWork";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=" + fields + "&asOf=" + asOfDate.ToString() + "&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public GetWorkItemsWithLinksAndAttachmentsResponse.WorkItems GetWorkItemsWithLinksAndAttachments(string ids)
        {
            GetWorkItemsWithLinksAndAttachmentsResponse.WorkItems viewModel = new GetWorkItemsWithLinksAndAttachmentsResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&expand=all&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemsWithLinksAndAttachmentsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
              
        public GetWorkItemExpandAllResponse.WorkItem GetWorkItem(string id)
        {
            GetWorkItemExpandAllResponse.WorkItem viewModel = new GetWorkItemExpandAllResponse.WorkItem();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // use $expand=all to get all fields
                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems/" + id + "?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemExpandAllResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetWorkItemExpandAllResponse.WorkItem GetWorkItemWithLinksAndAttachments(string id)
        {
            GetWorkItemExpandAllResponse.WorkItem viewModel = new GetWorkItemExpandAllResponse.WorkItem();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // use $expand=all to get all fields
                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems/" + id + "?$expand=relations&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemExpandAllResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetWorkItemExpandAllResponse.WorkItem GetWorkItemFullyExpanded(string id)
        {
            GetWorkItemExpandAllResponse.WorkItem viewModel = new GetWorkItemExpandAllResponse.WorkItem();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // use $expand=all to get all fields
                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems/" + id + "?$expand=all&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemExpandAllResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public GetDefaultValuesResponse.Defaults GetDefaultValues(string type, string project)
        {
            GetDefaultValuesResponse.Defaults viewModel = new GetDefaultValuesResponse.Defaults();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // use $expand=all to get all fields
                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/workitems/$" + type + "?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetDefaultValuesResponse.Defaults>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
            
        public WorkItemPatchResponse.WorkItem CreateWorkItem(string projectName)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[1];

            patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = "JavaScript implementation for Microsoft Account" };
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
                             
                var method = new HttpMethod("PATCH");                 
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Task?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                var me = response.ToString();

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public WorkItemPatchResponse.WorkItem CreateWorkItemWithWorkItemLink(string projectName, string linkToId)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[5];

            patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = "JavaScript implementation for Microsoft Account" };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork", value = "4" };
            patchDocument[2] = new { op = "add", path = "/fields/System.Description", value = "Follow the code samples from MSDN" };
            patchDocument[3] = new { op = "add", path = "/fields/System.History", value = "Jim has the most context around this." };
            patchDocument[4] = new
            {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "System.LinkTypes.Hierarchy-Reverse",
                    url = _configuration.UriString + "/_apis/wit/workitems/" + linkToId,
                    attributes = new
                    {
                        comment = "decomposition of work"
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

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Task?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;              

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public WorkItemPatchResponse.WorkItem CreateWorkItemByPassingRules(string projectName)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();

            Object[] patchDocument = new Object[3];
                      
            // patch document to create a work item
            patchDocument[0] = new { op = "add", path = "/fields/System.Title", value = "JavaScript implementation for Microsoft Account" };
            patchDocument[1] = new { op = "add", path = "/fields/System.CreatedDate", value = "6/1/2016" };
            patchDocument[2] = new { op = "add", path = "/fields/System.CreatedBy", value = "Art VanDelay" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                var method = new HttpMethod("PATCH");               
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$User Story?bypassRules=true&api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                var me = response.ToString();

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
              
        public WorkItemPatchResponse.WorkItem UpdateWorkItemUpdateField(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[3];

            // change some values on a few fields
            patchDocument[0] = new { op = "test", path = "/rev", value = "1" };
            patchDocument[1] = new { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "2" };
            patchDocument[2] = new { op = "add", path = "/fields/System.History", value = "Changing priority" };
                      
            using (var client = new HttpClient())
            {               
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);
                
                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                var method = new HttpMethod("PATCH");                 
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;
                               
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemMoveWorkItem(string id, string teamProject, string areaPath, string iterationPath)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[3];

            // set the required field values for the destination
            patchDocument[0] = new { op = "add", path = "/fields/System.TeamProject", value = teamProject };
            patchDocument[1] = new { op = "add", path = "/fields/System.AreaPath", value = areaPath };
            patchDocument[2] = new { op = "add", path = "/fields/System.IterationPath", value = iterationPath };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
                               
                var method = new HttpMethod("PATCH");         
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public WorkItemPatchResponse.WorkItem UpdateWorkItemChangeWorkItemType(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[2];

            // change the work item type, state and reason values in order to change the work item type
            patchDocument[0] = new { op = "add", path = "/fields/System.WorkItemType", value = "User Story" };
            patchDocument[1] = new { op = "add", path = "/fields/System.State", value = "Active" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
                               
                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;               

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemAddTag(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[1];

            // change some values on a few fields
            patchDocument[0] = new { op = "add", path = "/fields/System.Tags", value = "Tag1; Tag2" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
                               
                var method = new HttpMethod("PATCH");             
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemAddLink(string id, string linkToId)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[1];

            // change some values on a few fields
            patchDocument[0] = new
            {
                op = "add",
                path = "/relations/-",
                value = new {
                    rel = "System.LinkTypes.Dependency-forward",
                    url = _configuration.UriString + "/_apis/wit/workitems/" + linkToId,
                    attributes = new {
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
                             
                var method = new HttpMethod("PATCH");                
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
     
        public WorkItemPatchResponse.WorkItem UpdateWorkItemUpdateLink(string id, string linkToId)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[2];

            // change some values on a few fields
            patchDocument[0] = new { op = "test", path = "/rev", value = "1" };
            patchDocument[1] = new {
                op = "add",
                path = "/relations/-",
                value = new WorkItemPatch.Value()
                {
                    rel = "System.LinkTypes.Dependency-forward",
                    url = _configuration.UriString + "/_apis/wit/workitems/" + linkToId,
                    attributes = new WorkItemPatch.Attributes()
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

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemRemoveLink(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[2];

            // change some values on a few fields
            patchDocument[0] = new { op = "test", path = "/rev", value = "1" };
            patchDocument[1] = new
            {
                op = "remove",
                path = "/relations/0",               
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemAddAttachment(string id, string url)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[3];

            // change some values on a few fields
            patchDocument[0] = new { op = "test", path = "/rev", value = "1" };
            patchDocument[1] = new { op = "add", path = "/fields/System.History", value = "Adding the necessary spec" };
            patchDocument[2] = new
            {
                op = "add",
                path = "/relations/-",
                value = new
                {
                    rel = "AttachedFile",
                    url = url,
                    attributes = new
                    {
                        comment = "VanDelay Industries - Spec"
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

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemRemoveAttachment(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[2];

            // change some values on a few fields
            patchDocument[0] = new { op = "test", path = "/rev", value = "2" };
            patchDocument[1] = new { op = "remove", path = "/relations/0" };
          
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem UpdateWorkItemAddHyperLink(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[2];

            // change some values on a few fields
            patchDocument[0] = new { op = "test", path = "/rev", value = "1" };
            patchDocument[1] = new {
                op = "add",
                path = "/relations/-",
                value = new {
                    rel = "Hyperlink",
                    url = "http://www.visualstudio.com/team-services",
                    attributes = new {
                        comment = "Visual Studio Team Services"
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
                           
                var method = new HttpMethod("PATCH");                
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                    viewModel.Message = "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    viewModel.Message = msg.ToString();
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        public WorkItemPatchResponse.WorkItem UpdateWorkItemByPassingRules(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[1]; ;

            // replace value on a field that you normally cannot change, like system.createdby
            patchDocument[0] = new { op = "replace", path = "/fields/System.CreatedBy", value = "Foo <Foo@hotmail.com>" };
                      
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(patchDocument), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call
                              
                var method = new HttpMethod("PATCH");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?bypassRules=true&api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
           
        public WorkItemPatchResponse.WorkItem UpdateWorkItemAddCommitLink(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            Object[] patchDocument = new Object[1]; ;

            // change some values on a few fields
            patchDocument[0] = new WorkItemPatch.Field()
            {
                op = "add",
                path = "/relations/-",
                value = new {
                    rel = "ArtifactLink",
                    url = "vstfs:///Git/Commit/1435ac99-ba45-43e7-9c3d-0e879e7f2691%2Fd00dd2d9-55dd-46fc-ad00-706891dfbc48%2F3fefa488aac46898a25464ca96009cf05a6426e3",
                    attributes = new {
                       name = "Fixed in Commit"
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

                var method = new HttpMethod("PATCH");                
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                    viewModel.Message = "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    viewModel.Message = msg.ToString();
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        public WorkItemPatchResponse.WorkItem DeleteWorkItem(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
           
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                              
                var method = new HttpMethod("DELETE");
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2");
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {                   
                    viewModel.Message = "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    viewModel.Message = msg.ToString();
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}


