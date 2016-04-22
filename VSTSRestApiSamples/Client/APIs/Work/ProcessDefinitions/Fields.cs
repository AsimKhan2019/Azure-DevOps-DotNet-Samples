using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VstsRestApiSamples.Client.Helpers;
using VstsRestApiSamples.ViewModels.Work;

namespace VstsRestApiSamples.Client.APIs.Work.ProcessDefinitions
{
    public class Fields
    {
        private string _account;
        private string _login;

        public Fields(IAuth auth)
        {
            _account = auth.Account;
            _login = auth.Login;
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
                client.BaseAddress = new Uri(_account);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _login);

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
