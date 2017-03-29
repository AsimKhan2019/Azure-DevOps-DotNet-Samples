using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTracking
{
    /// <summary>
    /// 
    /// Samples for accessing work item field metadata.
    /// 
    /// See https://www.visualstudio.com/docs/integrate/api/wit/fields for more details.
    /// 
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.Fields)]
    public class FieldsSample : ClientSample
    {

        [ClientSampleMethod]
        public WorkItemField GetFieldDetails()
        {
            string fieldName = "System.Title";

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemField workitemField = workItemTrackingClient.GetFieldAsync(fieldName).Result;

            Console.WriteLine("Name: " + workitemField.Name);
            Console.WriteLine("Ref name: " + workitemField.ReferenceName);
            Console.WriteLine("Read only? " + workitemField.ReadOnly);

            return workitemField;
        }

        [ClientSampleMethod]
        public void GetReadOnlyWorkItemFields()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> result = workItemTrackingClient.GetFieldsAsync().Result;

            Console.WriteLine("Read only fields:");
            foreach (var workitemField in result.Where(field => field.ReadOnly))
            {
                Console.WriteLine(" * {0} ({1})", workitemField.Name, workitemField.ReferenceName);
            }
        }
    }
}
