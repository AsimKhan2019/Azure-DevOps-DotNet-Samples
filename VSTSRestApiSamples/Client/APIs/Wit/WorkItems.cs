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

        public HttpStatusCode GetListOfWorkItemsByIDs(string ids)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                }

                return response.StatusCode;
            }
        }

        public HttpStatusCode GetListOfWorkItemsByIDsWithSpecificFields(string ids)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

                HttpResponseMessage response = client.GetAsync("_apis/wit/workitems?ids=" + ids + "&fields=System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Scheduling.RemainingWork&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    var vm = response.Content.ReadAsAsync<ListofWorkItemsResponse.WorkItems>().Result;
                    //var vm = response.Content.ReadAsStringAsync().Result;
                    
                }

                return response.StatusCode;
            }
        }
    }
}


