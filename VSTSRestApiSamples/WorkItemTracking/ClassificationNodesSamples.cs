using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VstsRestApiSamples.ViewModels;
using VstsRestApiSamples.ViewModels.WorkItemTracking;

namespace VstsRestApiSamples.WorkItemTracking
{
    /// <summary>
    /// otherwise known as area paths
    /// </summary>
    public class ClassificationNodesSamples
    {
        readonly IConfiguration _configuration;
        readonly string _credentials;

        public ClassificationNodesSamples(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _configuration.PersonalAccessToken)));
        }

        public List<string> GetAreaTree(string project)
        {
            GetNodesResponse.Nodes nodes = new GetNodesResponse.Nodes();
            List<string> list = new List<string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/areas?api-version=2.2&$depth=2").Result;

                if (response.IsSuccessStatusCode)
                {
                    nodes = response.Content.ReadAsAsync<GetNodesResponse.Nodes>().Result;
                                     
                    //list.Add(result.name);
                    walkTreedNode(client, project, nodes, "", list);
                }

                return list;
            }
        }
        
        public List<string> GetIterationTree(string project)
        {
            GetNodesResponse.Nodes result = new GetNodesResponse.Nodes();
            List<string> list = new List<string>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/iterations?api-version=2.2&$depth=2").Result;

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsAsync<GetNodesResponse.Nodes>().Result;

                    //list.Add(result.name);
                    walkTreedNode(client, project, result, "", list);
                }

                return list;
            }
        }

        private void walkTreedNode(HttpClient client, string project, GetNodesResponse.Nodes node, string nodePath, List<string> list)
        {
            HttpResponseMessage response;
            GetNodesResponse.Nodes result;
            string name = string.Empty;

            foreach (var item in node.children)
            {
                if (String.IsNullOrEmpty(nodePath))
                {
                    name = item.name;
                }
                else
                {
                    name = nodePath + "/" + item.name;
                }

                list.Add(name);

                if (item.hasChildren)
                {                                                    
                    response = client.GetAsync(project + "/_apis/wit/classificationNodes/iterations/" + name + "?api-version=2.2&$depth=2").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsAsync<GetNodesResponse.Nodes>().Result;                       

                        walkTreedNode(client, project, result, name, list);
                    }
                }
            }          
        }
    }
}
