using System;
using System.Net.Http;
using System.Net.Http.Headers;
using VstsRestApiSamples.ViewModels.Work;

namespace VstsRestApiSamples.Work.ProcessConfiguration
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

        /// <summary>
        /// add fields to a picklist
        /// </summary>
        /// <param name="processId">process id</param>
        /// <param name="picklistId">picklist id</param>
        /// <returns>FieldsPostResponse.Field</returns>
        public FieldsPostResponse.Field CreatePickListField(string processId, string picklistId)
        {
            FieldsPostResponse.Field viewModel = new FieldsPostResponse.Field();

            //create field object and set values
            FieldsPost.Field data = new FieldsPost.Field()
            {
                Name = "Favorite Color",
                Type = "String",
                Description = "These are my favorite colors",
                ListId = picklistId //id from when we created a picklist
            };           

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.PostAsJsonAsync("_apis/work/processdefinitions/" + processId + "/fields?api-version=2.1-preview", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<FieldsPostResponse.Field>().Result;
                }

                viewModel.HttpStatusCode = response.StatusCode;

                return viewModel;
            }
        }
    }
}
