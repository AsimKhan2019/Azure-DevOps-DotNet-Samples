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

    public void CreateNewBug()
    {
        var projectName = _configuration.Project;

        WorkItemPatchResponse.WorkItem viewModel = new WorkItemPatchResponse.WorkItem();
        WorkItemPatch.Field[] fields = new WorkItemPatch.Field[4];

        //set some field values like title and description
        fields[0] = new WorkItemPatch.Field() { op = "add", path = "/fields/System.Title", value = "Authorization Errors" };
        fields[1] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.TCM.ReproSteps", value = "Our authorization logic needs to allow for users with Microsoft accounts (formerly Live Ids) - http://msdn.microsoft.com/en-us/library/live/hh826547.aspx" };
        fields[2] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Priority", value = "1" };
        fields[3] = new WorkItemPatch.Field() { op = "add", path = "/fields/Microsoft.VSTS.Common.Severity", value = "2 - High" };
            
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

            //serialize the fields array into a json string          
            var patchValue = new StringContent(JsonConvert.SerializeObject(fields), Encoding.UTF8, "application/json-patch+json"); //mediaType needs to be application/json-patch+json for a patch call

            //set the httpmethod to Patch
            var method = new HttpMethod("PATCH");

            //send the request
            var request = new HttpRequestMessage(method, _configuration.UriString + projectName + "/_apis/wit/workitems/$Bug?api-version=1.0") { Content = patchValue };
            var response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                viewModel = response.Content.ReadAsAsync<WorkItemPatchResponse.WorkItem>().Result;
            }             
        }
    }

        public void UdateExistingWorkItem()
        {

        }

    }
}
