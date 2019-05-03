using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTracking
{
    /// <summary>
    /// Client samples for managing work items in Team Services and Team Foundation Server.
    /// </summary>
    [ClientSample(WitConstants.WorkItemTrackingWebConstants.RestAreaName, "clientomdeprecationsamples")]
    public class ClientOMDeprecationSamples : ClientSample
    {
        [ClientSampleMethod]
        public WorkItem ValidateWorkItem()
        {
            //Setup
            //In order for your wit to fail on validation, you need either set the field required property or create a conditional rule to make a field required

            //Scenerio
            //to determine the work item type you are creating can be saved with the fields set in the patch document
            //first: check to see if there are any requried fields
            //second: do a save using validateOnly flag. this will run the save against the rules and return any validation errors
            
            string projectName = "temp"; 
            
            // Get a client
            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();
            
            //get the work item type
            WorkItemType workItemType = workItemTrackingClient.GetWorkItemTypeAsync(projectName, "Task").Result;

            //get a list of all of the required fields
            List<WorkItemTypeFieldInstance> fields = (List<WorkItemTypeFieldInstance>)workItemType.Fields;
            IEnumerable<WorkItemTypeFieldInstance> reqFields = fields.Where(x => x.AlwaysRequired == true && String.IsNullOrEmpty(x.DefaultValue));

            Console.WriteLine("Required Fields...");

            foreach (WorkItemTypeFieldInstance field in reqFields)
            {
                Console.WriteLine("  {0}", field.ReferenceName);
            }

            Console.WriteLine("");

            // Construct the object containing field values required for the new work item
            JsonPatchDocument patchDocument = new JsonPatchDocument();

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Sample task 1"
                }
            );

            try
            {
                // validate the patch document when trying to create a work item
                WorkItem newWorkItem = workItemTrackingClient.CreateWorkItemAsync(patchDocument, projectName, "Task", true).Result;
            }  
            //get the list of rule validation exceptions when there are only rule validation errors
            catch (Exception ruleValidationException) when (ruleValidationException.InnerException.Message.Contains("TF401320:"))
            {
                IEnumerable<RuleValidationException> ruleValidationErrors = ((RuleValidationException)ruleValidationException.InnerException).RuleValidationErrors;

                Console.WriteLine("Found the following validation errors...");

                foreach (RuleValidationException ruleValidationError in ruleValidationErrors)
                {
                    Console.WriteLine("  {0}", ruleValidationError.ErrorMessage);
                }
            }
            catch (Exception otherException)
            {
                Console.WriteLine("Other Exceptions Found:");
                Console.WriteLine(otherException.InnerException.Message);
            }

            patchDocument = null;
            connection.Dispose();
            connection = null;
            workItemTrackingClient.Dispose();
            workItemTrackingClient = null;

            return null;
        }


    }
}
