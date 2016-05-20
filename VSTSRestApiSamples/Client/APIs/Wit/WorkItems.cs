using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Wit;

namespace VstsRestApiSamples.Client.APIs.Wit
{
    public class WorkItems
    {
        private string _account;
        private string _login;

        public WorkItems(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
        }

        /// <summary>
        /// Get a list of work items by one ore more id's
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>ListofWorkItemsResponse.WorkItems</returns>
        public ListofWorkItemsResponse.WorkItems GetListOfWorkItemsByIDs(string ids)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&api-version=1.0").Result;
                            
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// Get list work items but only specific fields
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>ListofWorkItemsResponse.WorkItems</returns>
        public ListofWorkItemsResponse.WorkItems GetListOfWorkItemsByIDsWithSpecificFields(string ids)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();

            //list of fields that i care about
            string fields = "System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Scheduling.RemainingWork";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=" + fields + "&api-version=1.0").Result;
                                
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get a work item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>GetWorkItemExpandAllResponse.WorkItem</returns>
        public GetWorkItemExpandAllResponse.WorkItem GetWorkItem(string id)
        {
            GetWorkItemExpandAllResponse.WorkItem viewModel = new GetWorkItemExpandAllResponse.WorkItem();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                //use $expand=all to get all fields
                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems/" + id + "?$expand=all&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetWorkItemExpandAllResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// update a specific work item by id and return that changed worked item
        /// </summary>
        /// <param name="id"></param>
        /// <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem UpdateWorkItemFields(string id)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[4];

            //change some values on a few fields
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.History", value = "adding some history" };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "2" };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.BusinessValue", value = "100" };
            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.ValueArea", value = "Architectural" };
                      
            using (var client = new HttpClient())
            {               
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);
                
                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH"); 

                //send the request
                var request = new HttpRequestMessage(method, _account + "_apis/wit/workitems/" + id + "?api-version=1.0") { Content = patchValue };
                var response = client.SendAsync(request).Result;
                               
                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
        
        /// <summary>
        /// get a batch of work item links starting at a specific start date and scoped to a project
        /// </summary>
        /// <param name="project">project name or id</param>
        /// <param name="startDateTime"></param>
        /// <returns>BatchOfWorkItemLinksResponse.WorkItemLinks</returns>
        public BatchOfWorkItemLinksResponse.WorkItemLinks GetBatchOfWorkItemLinks(string project, DateTime startDateTime)
        {
            BatchOfWorkItemLinksResponse.WorkItemLinks viewModel = new BatchOfWorkItemLinksResponse.WorkItemLinks();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/reporting/workitemlinks?startDateTime=" + startDateTime.ToShortDateString() + "&api-version=2.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get all of the work item links by paging through list
        /// </summary>
        /// <returns>BatchOfWorkItemLinksResponse.WorkItemLinks</returns>
        public BatchOfWorkItemLinksResponse.WorkItemLinks GetBatchOfWorkItemLinksAll()
        {
            BatchOfWorkItemLinksResponse.WorkItemLinks viewModel = new BatchOfWorkItemLinksResponse.WorkItemLinks();
            BatchOfWorkItemLinksResponse.WorkItemLinks tempViewModel = new BatchOfWorkItemLinksResponse.WorkItemLinks();
            List<BatchOfWorkItemLinksResponse.Value> list = new List<BatchOfWorkItemLinksResponse.Value>();
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {               
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);
                                
                response = client.GetAsync("_apis/wit/reporting/workitemlinks?api-version=2.0").Result;

                if (!response.IsSuccessStatusCode)
                {
                    viewModel.HttpStatusCode = response.StatusCode;
                    return viewModel;
                }
                else
                {
                    //read from response
                    tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;

                    //and add values to list object
                    list.AddRange(tempViewModel.values);

                    //keep looping through the list untill done
                    //loop thru until isLastBatch = true
                    while (!tempViewModel.isLastBatch)
                    {
                        //using watermarked nextLink value, get next page from list
                        response = client.GetAsync(tempViewModel.nextLink).Result;

                        if (!response.IsSuccessStatusCode)
                        {
                            viewModel.HttpStatusCode = response.StatusCode;
                            return viewModel;
                        }
                        else
                        {
                            //read and add to your list
                            tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;
                            list.AddRange(tempViewModel.values);
                        }
                    }

                    //loaded all pages, now set value in viewModel with list object so we can send the entire list back
                    viewModel.values = list.ToArray<BatchOfWorkItemLinksResponse.Value>();
                    viewModel.HttpStatusCode = response.StatusCode;

                    return viewModel;
                }
            }
        }

        /// <summary>
        /// get batch of work item revisions by start date scoped to project
        /// </summary>
        /// <param name="project">project name or id</param>
        /// <param name="startDateTime"></param>
        /// <returns>BatchOfWorkItemRevisionsResponse.WorkItemRevisions</returns>
        public BatchOfWorkItemRevisionsResponse.WorkItemRevisions GetBatchOfWorkItemRevisionsByDate(string project, DateTime startDateTime)
        {
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions viewModel = new BatchOfWorkItemRevisionsResponse.WorkItemRevisions();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/reporting/workItemRevisions?startDateTime=" + startDateTime.ToShortDateString() + "&api-version=2.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// get all work item revisions by paging through list
        /// </summary>
        /// <returns>BatchOfWorkItemRevisionsResponse.WorkItemRevisions</returns>
        public BatchOfWorkItemRevisionsResponse.WorkItemRevisions GetBatchOfWorkItemRevisionsAll()
        {
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions tempViewModel = new BatchOfWorkItemRevisionsResponse.WorkItemRevisions();
            BatchOfWorkItemRevisionsResponse.WorkItemRevisions viewModel = new BatchOfWorkItemRevisionsResponse.WorkItemRevisions();
            HttpResponseMessage response;
            List<BatchOfWorkItemRevisionsResponse.Value> list = new List<BatchOfWorkItemRevisionsResponse.Value>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                response = client.GetAsync("_apis/wit/reporting/workItemRevisions?api-version=2.0").Result;

                if (!response.IsSuccessStatusCode)
                {
                    viewModel.HttpStatusCode = response.StatusCode;
                    return viewModel;
                }
                else
                {
                    //read from response
                    tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;

                    //add values to the list object
                    list.AddRange(tempViewModel.values);

                    //keep looping through the list untill done
                    //loop thru until isLastBatch = true
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
                            //read response
                            tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;

                            //add new batch to my list
                            list.AddRange(tempViewModel.values);
                        }                       
                    }

                    viewModel.HttpStatusCode = response.StatusCode;
                    viewModel.values = list.ToArray<BatchOfWorkItemRevisionsResponse.Value>();

                    return viewModel;
                }                
            }
        }

        /// <summary>
        /// manage tags on a work item
        /// </summary>
        /// <param name="id">work item id</param>
        /// <param name="tags">tags in the tagname; tagname; format</param>
        /// <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem AddWorkItemTags(string id, string tags)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[1];

            //change some values on a few fields
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Tags", value = tags };
           
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                //send the request
                var request = new HttpRequestMessage(method, _account + "_apis/wit/workitems/" + id + "?api-version=1.0") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// Move a work item from one project to another when the projects are the same process (agile to agile)
        /// </summary>
        /// <param name="id">work item id</param>
        /// <param name="teamProject">project name</param>
        /// <param name="areaPath">area path</param>
        /// <param name="iterationPath">iteration path</param>
        /// <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem MoveWorkItem(string id, string teamProject, string areaPath, string iterationPath)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[3];

            //set the required field values for the destination
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.TeamProject", value = teamProject };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = areaPath };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = iterationPath };           

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                //send the request
                var request = new HttpRequestMessage(method, _account + "_apis/wit/workitems/" + id + "?api-version=1.0") { Content = patchValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

        /// <summary>
        /// move a work item from a project in agile to a project in scrum
        /// </summary>
        /// <param name="id">work item id</param>
        /// <param name="teamProject">project name</param>
        /// <param name="areaPath">area path</param>
        /// <param name="iterationPath">iteration path</param>
        /// <returns>WorkItemPatchResponse.WorkItem</returns>
        public WorkItemPatchResponse.WorkItem MoveWorkItemAndChangeType(string id, string teamProject, string areaPath, string iterationPath)
        {
            WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
            WorkItemPatch.Field[] fields = new WorkItemPatch.Field[6];

            //change the required field values in order to do move
            fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.TeamProject", value = teamProject };
            fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.AreaPath", value = areaPath };
            fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.IterationPath", value = iterationPath };

            //change the work item type, state and reason values in order to change the work item type
            fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.WorkItemType", value = "Product Backlog Item" };
            fields[4] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.State", value = "New" };
            fields[5] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Reason", value = "New Backlog Item" };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                //serialize the fields array into a json string          
                var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

                //set the httpmethod to Patch
                var method = new HttpMethod("PATCH");

                //send the request
                var request = new HttpRequestMessage(method, _account + "_apis/wit/workitems/" + id + "?api-version=1.0") { Content = patchValue };
                var response = client.SendAsync(request).Result;

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


