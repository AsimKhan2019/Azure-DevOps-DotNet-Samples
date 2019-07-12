using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Serviceendpoint
{
    /// <summary>
    /// 
    /// Samples for interacting with Azure DevOps service endpoints (also known as service connections)
    ///
    /// Package: https://www.nuget.org/packages/Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
    /// 
    /// </summary>
    [ClientSample(ServiceEndpointResourceIds.AreaName, ServiceEndpointResourceIds.EndpointResource.Name)]
    public class EndpointsSample : ClientSample
    {
        [ClientSampleMethod]
        public IEnumerable<ServiceEndpointType> ListEndpointTypes()
        {
            // Get a service endpoint client instance
            VssConnection connection = Context.Connection;
            ServiceEndpointHttpClient endpointClient = connection.GetClient<ServiceEndpointHttpClient>();

            // Get a list of all available service endpoint types
            List<ServiceEndpointType> types = endpointClient.GetServiceEndpointTypesAsync().Result;

            // Show details about each type in the console
            foreach(ServiceEndpointType t in types)
            {
                Context.Log(t.Name);

                Context.Log("Inputs:");
                foreach (InputDescriptor input in t.InputDescriptors)
                {
                    Context.Log("- {0}", input.Id);
                }

                Context.Log("Schemes:");
                foreach (ServiceEndpointAuthenticationScheme scheme in t.AuthenticationSchemes)
                {
                    Context.Log("- {0}", scheme.Scheme);
                    Context.Log("  Inputs:");
                    foreach (InputDescriptor input in scheme.InputDescriptors)
                    {
                        Context.Log("  - {0}", input.Id);
                    }
                }

                Context.Log("================================");
            }

            return types;
        }

        [ClientSampleMethod]
        public ServiceEndpoint CreateGenericEndpoint()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get a service endpoint client instance
            VssConnection connection = Context.Connection;
            ServiceEndpointHttpClient endpointClient = connection.GetClient<ServiceEndpointHttpClient>();
           
            // Create a generic service endpoint 
            ServiceEndpoint endpoint = endpointClient.CreateServiceEndpointAsync(project.Id, new ServiceEndpoint()
            {
                Name = "MyNewServiceEndpoint",
                Type = ServiceEndpointTypes.Generic,
                Url = new Uri("https://myserver"),
                Authorization = new EndpointAuthorization()
                {
                    Scheme = EndpointAuthorizationSchemes.UsernamePassword,
                    Parameters = new Dictionary<string, string>()
                    {
                        { "username", "myusername" },
                        { "password", "mysecretpassword" }
                    }
                }
            }).Result;

            Context.Log("Created endpoint: {0} {1} in {2}", endpoint.Id, endpoint.Name, project.Name);

            // Save new endpoint so it can be deleted later
            Context.SetValue<Guid>("$newServiceEndpointId", endpoint.Id);
            Context.SetValue<Guid>("$newServiceEndpointProjectId", project.Id);

            return endpoint;
        }

        [ClientSampleMethod]
        public ServiceEndpoint CreateAzureRMEndpoint()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get a service endpoint client instance
            VssConnection connection = Context.Connection;
            ServiceEndpointHttpClient endpointClient = connection.GetClient<ServiceEndpointHttpClient>();

            // Create a generic service endpoint 
            ServiceEndpoint endpoint = endpointClient.CreateServiceEndpointAsync(project.Id, new ServiceEndpoint()
            {
                Name = "MyNewARMServiceEndpoint",
                Type = ServiceEndpointTypes.AzureRM,
                Url = new Uri("https://management.azure.com/"),
                Data = new Dictionary<string, string>() {
                    {"subscriptionId", "1272a66f-e2e8-4e88-ab43-487409186c3f" },
                    {"subscriptionName", "subscriptionName" },
                    {"environment", "AzureCloud"},
                    {"scopeLevel", "Subscription"},
                    {"creationMode", "Manual" }
                },
                Authorization = new EndpointAuthorization()
                {
                    Scheme = EndpointAuthorizationSchemes.ServicePrincipal,
                    Parameters = new Dictionary<string, string>()
                    {
                        { "tenantid", "1272a66f-e2e8-4e88-ab43-487409186c3f" },
                        { "serviceprincipalid", "1272a66f-e2e8-4e88-ab43-487409186c3f" },
                        { "authenticationType", "spnKey" },
                        { "serviceprincipalkey", "SomePassword" }
                    }
                }
            }).Result;

            Context.Log("Created endpoint: {0} {1} in {2}", endpoint.Id, endpoint.Name, project.Name);

            return endpoint;
        }

        [ClientSampleMethod]
        public IEnumerable<ServiceEndpoint> ListEndpoints()
        {
            TeamProjectReference project = ClientSampleHelpers.FindAnyProject(this.Context);

            // Get a service endpoint client instance
            VssConnection connection = Context.Connection;
            ServiceEndpointHttpClient endpointClient = connection.GetClient<ServiceEndpointHttpClient>();

            // Get a list of all "generic" service endpoints in the specified project
            List<ServiceEndpoint> endpoints = endpointClient.GetServiceEndpointsAsync(
                project: project.Id,
                type: ServiceEndpointTypes.Generic).Result;

            // Show the endpoints
            Context.Log("Endpoints in project: {0}", project.Name);
            foreach (ServiceEndpoint endpoint in endpoints)
            {
                Context.Log("- {0} {1}", endpoint.Id.ToString().PadLeft(6), endpoint.Name);
            }

            return endpoints;           
        }

        [ClientSampleMethod]
        public void DeleteServiceEndpoint()
        {
            // Get ID of previously-created service endpoint
            Guid endpointId = Context.GetValue<Guid>("$newServiceEndpointId");
            Guid projectId = Context.GetValue<Guid>("$newServiceEndpointProjectId");

            // Get a service endpoint client instance
            VssConnection connection = Context.Connection;
            ServiceEndpointHttpClient endpointClient = connection.GetClient<ServiceEndpointHttpClient>();

            try
            {
                endpointClient.DeleteServiceEndpointAsync(projectId, endpointId).SyncResult();

                Context.Log("Sucecssfully deleted endpoint {0} in {1}", endpointId, projectId);
            }
            catch (Exception ex)
            {
                Context.Log(ex.Message);
            }
        }
    }
}

