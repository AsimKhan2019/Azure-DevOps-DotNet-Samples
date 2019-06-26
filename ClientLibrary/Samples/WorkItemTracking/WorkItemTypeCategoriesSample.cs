using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    /// <summary>
    /// 
    /// Samples for accessing work item type category metadata.
    /// 
    /// See https://www.visualstudio.com/docs/integrate/api/wit/categories for more details.
    /// 
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, WitConstants.WorkItemTrackingRestResources.WorkItemTypeCategories)]
    public class WorkItemTypeCategoriesSample : ClientSample
    {

        [ClientSampleMethod]
        public List<WorkItemTypeCategory> GetListOfWorkItemTypeCategories()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemTypeCategory> results = workItemTrackingClient.GetWorkItemTypeCategoriesAsync(projectId).Result;

            Console.WriteLine("Work Item Type Categories:");

            foreach (WorkItemTypeCategory category in results)
            {
                Console.WriteLine("   {0} <{1}>", category.Name, category.ReferenceName);
            }

            return results;
        }

        [ClientSampleMethod]
        public WorkItemTypeCategory GetWorkItemCategory()
        {
            Guid projectId = ClientSampleHelpers.FindAnyProject(this.Context).Id;
            string category = "Microsoft.RequirementCategory";

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemTypeCategory result = workItemTrackingClient.GetWorkItemTypeCategoryAsync(projectId, category).Result;

            Console.WriteLine("Name: {0}", result.Name);
            Console.WriteLine("Reference Name: {0}", result.ReferenceName);
            Console.WriteLine("Work Item Types:");

            foreach(var wit in result.WorkItemTypes)
            {
                Console.WriteLine("    {0}", wit.Name);
            }

            return result;
        }
    }
}
