using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamServices.Samples.Client.WorkItemTrackingProcess
{
    /// <summary>
    /// 
    /// Samples showing how to work with work item attachments.
    /// 
    /// See https://www.visualstudio.com/docs/integrate/api/wit/attachments for more details.
    /// 
    /// </summary>
    [ClientSample(Microsoft.TeamFoundation.WorkItemTracking.WebApi.WitConstants.WorkItemTrackingWebConstants.RestAreaName, "process")]
    public class ProcessesSample : ClientSample
    {
        readonly string _refname = "fabrikam.MyNewAgileProcess";
        
        [ClientSampleMethod]
        public List<ProcessInfo> List()        
        {          
            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Getting list of processes....");

            List<ProcessInfo> list = client.GetListOfProcessesAsync().Result;

            if (list == null || list.Count == 0)
            {
                Console.WriteLine("No processes found");
            }
            else
            {
                Console.WriteLine("Done");

                foreach (var item in list)
                {
                    Console.WriteLine("{0}    {1}    {2}", item.Name, item.TypeId, item.ReferenceName);
                }
            }
            
            return list;
        }              

        [ClientSampleMethod]
        public ProcessInfo Create()
        {            
            ProcessInfo processInfo = null;

            //create process model record object that will be used to create the process
            CreateProcessModel processModel = new CreateProcessModel
            {                
                Name = "MyNewAgileProcess",
                ParentProcessTypeId = new System.Guid("adcc42ab-9882-485e-a3ed-7678f01f66bc"),
                ReferenceName = _refname,
                Description = "My new process"
            };

            Console.Write("Creating new processes '" + _refname + "'...");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();
            
            try
            {
                //save type id for later use
                processInfo = client.CreateNewProcessAsync(processModel).Result;

                Console.WriteLine("success");
                Console.WriteLine("Process Id: {0}", processInfo.TypeId);
            }
            catch (Exception ex) //exception will be thrown if process already exists
            {               
                Console.WriteLine("failed");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.InnerException.Message);               
            }       
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            return processInfo; 
        }

        [ClientSampleMethod]
        public ProcessInfo Get()
        {
            System.Guid processTypeId; 
            ProcessInfo processInfo = null;

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Get process....");

            //get a list of the procesess so i can pull the right process and get the id
            //shouldn't need to do this in production code. Just save the typeId to load process directly
            List<ProcessInfo> list = client.GetListOfProcessesAsync().Result;
            
            //get the process for a specific refname
            var item = list.Find(x => x.ReferenceName == _refname);
            
            if (item == null)
            {
                Console.WriteLine("Failed");
                Console.WriteLine("No process found for '{0}'", _refname);
            }
            else
            {
                processTypeId = item.TypeId;

                //load the process by id
                processInfo = client.GetProcessByItsIdAsync(processTypeId).Result;

                Console.WriteLine("success");
                Console.WriteLine("{0}    {1}", _refname, processInfo.TypeId);
            }          
            
            return processInfo;
        }
    }
}
