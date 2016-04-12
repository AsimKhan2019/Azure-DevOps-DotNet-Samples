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

        public ListofWorkItemsResponse.WorkItems GetListOfWorkItemsByIDsWithSpecificFields(string ids)
        {
            ListofWorkItemsResponse.WorkItems viewModel = new ListofWorkItemsResponse.WorkItems();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Scheduling.RemainingWork&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }

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
        /// Get all of the work item links
        /// </summary>
        /// <returns></returns>
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

                //get from rest api endpoint 
                response = client.GetAsync("_apis/wit/reporting/workitemlinks?api-version=2.0").Result;

                if (!response.IsSuccessStatusCode)
                {
                    viewModel.HttpStatusCode = response.StatusCode;
                    return viewModel;
                }
                else
                {
                    //read from response and add values to list object
                    tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemLinksResponse.WorkItemLinks>().Result;
                    list.AddRange(tempViewModel.values);

                    //loop thru untill isLastBatch = true
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
                    tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;   
                    list.AddRange(tempViewModel.values);

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
                            tempViewModel = response.Content.ReadAsAsync<BatchOfWorkItemRevisionsResponse.WorkItemRevisions>().Result;                            
                            list.AddRange(tempViewModel.values);
                        }                       
                    }

                    viewModel.HttpStatusCode = response.StatusCode;
                    viewModel.values = list.ToArray<BatchOfWorkItemRevisionsResponse.Value>();

                    return viewModel;
                }                
            }
        }

    }
}


