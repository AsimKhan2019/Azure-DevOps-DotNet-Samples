using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using System.IO;

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

        // / <summary>
        // / Get a list of work items by one ore more id's
        // / </summary>
        // / <param name="ids"></param>
        // / <returns>ListofWorkItemsResponse.WorkItems</returns>
        public ListofWorkItemsResponse.WorkItems GetListOfWorkItems_ByIDs(string ids)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&api-version=2.2").Result;
                            
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / Get list work items but only specific fields
        // / </summary>
        // / <param name="ids"></param>
        // / <returns>ListofWorkItemsResponse.WorkItems</returns>
        public ListofWorkItemsResponse.WorkItems GetListOfWorkItems_ByIDsWithSpecificFields(string ids)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();

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
                    viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        // / <summary>
        // / get a work item by id
        // / </summary>
        // / <param name="id"></param>
        // / <returns>GetWorkItemExpandAllResponse.WorkItem</returns>
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
                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems/" + id + "?$expand=all&api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemExpandAllResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / create a work item using bypass rules
        // / </summary>
        // / <param name="projectName">name of project</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem CreateWorkItemUsingByPassRules(string projectName)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[2];

            // add a title and add a field you normally cant add such as CreatedDate
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Title", value = "hello world!" };
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.CreatedDate", value = "6/1/2016" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                var url = _configuration.UriString + projectName + "/_apis/wit/workitems/$UserStory?api-version=2.2";

                // send the request
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

        // / <summary>
        // / Create a bug
        // / </summary>
        // / <param name="projectName"></param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem CreateBug(string projectName)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[4];

            // set some field values like title and description
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Title", value = "Authorization Errors" };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.TCM.ReproSteps", value = "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http:// msdn.microsoft.com/en-us/library/live/hh826547.aspx" };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "1" };
            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = "2 - High" };


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");
               
                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Bug?api-version=2.2") { Content = patchValue };
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

        // / <summary>
        // / update a specific work item by id and return that changed worked item
        // / </summary>
        // / <param name="id"></param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem UpdateWorkItemFields(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[4];

            // change some values on a few fields
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.History", value = "adding some history" };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "2" };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.BusinessValue", value = "100" };
            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.ValueArea", value = "Architectural" };
                      
            using (var client = new HttpClient())
            {               
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);
                
                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH"); 

                // send the request
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

        // / <summary>
        // / update fields on work item using bypass rules
        // / </summary>
        // / <param name="id">work item id</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem UpdateWorkItemFieldsWithByPassRules(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[1];

            // replace value on a field that you normally cannot change, like system.createdby
            fields[0] = new WorkItemPatch.Field() { op = "replace", path = "/fields/System.CreatedBy", value = "Foo <Foo@hotmail.com>" };
                      
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
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

        // / <summary>
        // / get a batch of work item links starting at a specific start date and scoped to a project
        // / </summary>
        // / <param name="project">project name or id</param>
        // / <param name="startDateTime"></param>
        // / <returns>BatchOfWorkItemLinksResponse.WorkItemLinks</returns>
        public BatchOfWorkItemLinksResponse.WorkItemLinks GetBatchOfWorkItemLinks(string project, DateTime startDateTime)
        {
            BatchOfWorkItemLinksResponse.WorkItemLinks viewModel = new BatchOfWorkItemLinksResponse.WorkItemLinks();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/reporting/workitemlinks?startDateTime=" + startDateTime.ToShortDateString() + "&api-version=2.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / get all of the work item links by paging through list
        // / </summary>
        // / <returns>BatchOfWorkItemLinksResponse.WorkItemLinks</returns>
        public BatchOfWorkItemLinksResponse.WorkItemLinks GetBatchOfWorkItemLinksAll()
        {
            BatchOfWorkItemLinksResponse.WorkItemLinks viewModel = new BatchOfWorkItemLinksResponse.WorkItemLinks();
            BatchOfWorkItemLinksResponse.WorkItemLinks tempViewModel = new BatchOfWorkItemLinksResponse.WorkItemLinks();
            List<BatchOfWorkItemLinksResponse.Value> list = new List<BatchOfWorkItemLinksResponse.Value>();
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {               
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);
                                
                response = client.GetAsync("_apis/wit/reporting/workitemlinks?api-version=2.0").Result;

                if (!response.IsSuccessStatusCode)
                {
                    viewModel.HttpStatusCode = response.StatusCode;
                    return viewModel;
                }
                else
                {
                    // read from response
                    tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;

                    // and add values to list object
                    list.AddRange(tempViewModel.values);

                    // keep looping through the list untill done
                    // loop thru until isLastBatch = true
                    while (!tempViewModel.isLastBatch)
                    {
                        // using watermarked nextLink value, get next page from list
                        response = client.GetAsync(tempViewModel.nextLink).Result;

                        if (!response.IsSuccessStatusCode)
                        {
                            viewModel.HttpStatusCode = response.StatusCode;
                            return viewModel;
                        }
                        else
                        {
                            // read and add to your list
                            tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;
                            list.AddRange(tempViewModel.values);
                        }
                    }

                    // loaded all pages, now set value in viewModel with list object so we can send the entire list back
                    viewModel.values = list.ToArray<BatchOfWorkItemLinksResponse.Value>();
                    viewModel.HttpStatusCode = response.StatusCode;

                    return viewModel;
                }
            }
        }

        // / <summary>
        // / get batch of work item revisions by start date scoped to project
        // / </summary>
        // / <param name="project">project name or id</param>
        // / <param name="startDateTime"></param>
        // / <returns>BatchOfWorkItemRevisionsResponse.WorkItemRevisions</returns>
        public BatchOfWorkItemRevisionsResponse.WorkItemRevisions GetBatchOfWorkItemRevisionsByDate(string project, DateTime startDateTime)
        {
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions viewModel = new BatchOfWorkItemRevisionsResponse.WorkItemRevisions();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/reporting/workItemRevisions?startDateTime=" + startDateTime.ToShortDateString() + "&api-version=2.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        // / <summary>
        // / get all work item revisions by paging through list
        // / </summary>
        // / <returns>BatchOfWorkItemRevisionsResponse.WorkItemRevisions</returns>
        public BatchOfWorkItemRevisionsResponse.WorkItemRevisions GetBatchOfWorkItemRevisionsAll()
        {
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions tempViewModel = new BatchOfWorkItemRevisionsResponse.WorkItemRevisions();
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions viewModel = new BatchOfWorkItemRevisionsResponse.WorkItemRevisions();
            HttpResponseMessage response;
            List<BatchOfWorkItemRevisionsResponse.Value> list = new List<BatchOfWorkItemRevisionsResponse.Value>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                response = client.GetAsync("_apis/wit/reporting/workItemRevisions?api-version=2.0").Result;

                if (!response.IsSuccessStatusCode)
                {
                    viewModel.HttpStatusCode = response.StatusCode;
                    return viewModel;
                }
                else
                {
                    // read from response
                    tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;

                    // add values to the list object
                    list.AddRange(tempViewModel.values);

                    // keep looping through the list untill done
                    // loop thru until isLastBatch = true
                    while (!tempViewModel.isLastBatch)
                    {
                        response = client.GetAsync(tempViewModel.nextLink).Result;

                        if (!response.IsSuccessStatusCode)
                        {
                            viewModel.HttpStatusCode = response.StatusCode;
                            return viewModel;
                        }
                        else
                        {
                            // read response
                            tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;

                            // add new batch to my list
                            list.AddRange(tempViewModel.values);
                        }                       
                    }

                    viewModel.HttpStatusCode = response.StatusCode;
                    viewModel.values = list.ToArray<BatchOfWorkItemRevisionsResponse.Value>();

                    return viewModel;
                }                
            }
        }

        // / <summary>
        // / add link to another work item
        // / </summary>
        // / <param name="id">work item id</param>
        // / <param name="linkToId">link to work item id</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem AddLink(string id, string linkToId)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[1];

            // change some values on a few fields
            fields[0] = new WorkItemPatch.Field()
            {
                op = "add",
                path = "/relations/-",
                value =  new WorkItemPatch.Value()
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
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
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

        // / <summary>
        // / add link to another work item
        // / </summary>
        // / <param name="id">work item id</param>
        // / <param name="linkToId">link to work item id</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem AddAttachment(string id, string url)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[1];

            // change some values on a few fields
            fields[0] = new WorkItemPatch.Field()
            {
                op = "add",
                path = "/relations/-",
                value = new WorkItemPatch.Value()
                {
                    rel = "AttachedFile",
                    url = url,
                    attributes = new WorkItemPatch.Attributes()
                    {
                        comment = "adding attachment to work item"
                    }
                }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
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
              
        public WorkItemPatchResponse.WorkItem AddWorkItemTags(string id, string tags)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[1];

            // change some values on a few fields
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Tags", value = tags };
           
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
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

        // / <summary>
        // / Move a work item from one project to another when the projects are the same process (agile to agile)
        // / </summary>
        // / <param name="id">work item id</param>
        // / <param name="teamProject">project name</param>
        // / <param name="areaPath">area path</param>
        // / <param name="iterationPath">iteration path</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem MoveWorkItem(string id, string teamProject, string areaPath, string iterationPath)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[3];

            // set the required field values for the destination
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.TeamProject", value = teamProject };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = areaPath };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = iterationPath };           

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
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

        // / <summary>
        // / move a work item from a project in agile to a project in scrum
        // / </summary>
        // / <param name="id">work item id</param>
        // / <param name="teamProject">project name</param>
        // / <param name="areaPath">area path</param>
        // / <param name="iterationPath">iteration path</param>
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem MoveWorkItemAndChangeType(string id, string teamProject, string areaPath, string iterationPath)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[6];

            // change the required field values in order to do move
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.TeamProject", value = teamProject };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = areaPath };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = iterationPath };

            // change the work item type, state and reason values in order to change the work item type
            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.WorkItemType", value = "Product Backlog Item" };
            fields[4] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.State", value = "New" };
            fields[5] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Reason", value = "New Backlog Item" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",_credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
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
        
        // / <summary>
        // / move a work item from a project in agile to a project in scrum
        // / </summary>
        // / <param name="id">work item id</param>
        // / <param name="type">Bug or User Story</param>       
        // / <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem ChangeType(string id, string type)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[3];
           
            // change the work item type, state and reason values in order to change the work item type
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.WorkItemType", value = type };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.State", value = "New" };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Reason", value = "New" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); // mediaType needs to be application/json-patch+json for a patch call

                // set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                // send the request
                var request = new HttpRequestMessage(method, _configuration.UriString + "_apis/wit/workitems/" + id + "?api-version=2.2") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                var someme = response.ToString();

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}


