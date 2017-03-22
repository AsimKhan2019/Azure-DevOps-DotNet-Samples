using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vsts.ClientSamples.WorkItemTracking
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
        public WorkItemField GetFieldDetails(string fieldName = "System.Title")
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> result = workItemTrackingClient.GetFieldsAsync().Result;

            WorkItemField field = result.Find(x => x.Name == fieldName);
            
            return field;
        }

        [ClientSampleMethod]
        public IEnumerable<WorkItemField> GetReadOnlyWorkItemFields()
        {
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> result = workItemTrackingClient.GetFieldsAsync().Result;

            return result.Where(field => field.ReadOnly);
        }
    }
}
