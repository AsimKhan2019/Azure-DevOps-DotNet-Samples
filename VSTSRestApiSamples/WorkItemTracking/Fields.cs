using System;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    public class Fields
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public Fields(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        // / <summary>
        // / get list of all the fields in the account
        // / </summary>
        // / <returns>ListofWorkItemFieldsResponse.Fields</returns>
        public ListofWorkItemFieldsResponse.Fields GetListOfWorkItemFields()
        {
            ListofWorkItemFieldsResponse.Fields viewModel = new ListofWorkItemFieldsResponse.Fields();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync("_apis/wit/fields?api-version=2.2").Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<ListofWorkItemFieldsResponse.Fields>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
