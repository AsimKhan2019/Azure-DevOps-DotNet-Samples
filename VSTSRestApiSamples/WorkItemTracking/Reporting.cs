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
    public class Reporting
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Reporting(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }
                       
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
    }
}


