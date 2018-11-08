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
        private string _refName = "fabrikam.MyNewAgileProcess";
        private string _witRefName = "MyNewAgileProcess.ChangeRequest";

        [ClientSampleMethod]
        public List<ProcessInfo> Process_List()        
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
        public ProcessInfo Process_Create()
        {            
            ProcessInfo processInfo = Process_Get();
            
            if (processInfo != null)
            {
                return processInfo;
            }
                       
            //create process model record object that will be used to create the process
            CreateProcessModel processModel = new CreateProcessModel
            {                
                Name = "MyNewAgileProcess",
                ParentProcessTypeId = new System.Guid("adcc42ab-9882-485e-a3ed-7678f01f66bc"),
                ReferenceName = _refName,
                Description = "My new process"
            };

            Console.Write("Creating new processes '" + _refName + "'...");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();
            
            try
            {
                //save type id for later use
                processInfo = client.CreateNewProcessAsync(processModel).Result;

                Console.WriteLine("success");
                Console.WriteLine("Process Id: {0}", processInfo.TypeId);

                Context.SetValue<Guid>("$processId", processInfo.TypeId);
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
        public ProcessInfo Process_Get()
        {            
            ProcessInfo processInfo = null;
            
            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();                                 

            //extra step to get the process by name
            //you should not have to do this
            List<ProcessInfo> list = client.GetListOfProcessesAsync().Result;
            ProcessInfo item = list.Find(x => x.ReferenceName == _refName);

            //we did not find the process by name
            if (item == null)
            {
                Console.WriteLine("Process '{0}' not found", _refName);
                return null;
            }

            //we found something, so lets store the id in cache
            Context.SetValue<Guid>("$processId", item.TypeId);           

            //load the process by id
            processInfo = client.GetProcessByItsIdAsync(item.TypeId).Result;

            Console.WriteLine("Get process...success");
            Console.WriteLine("{0}    {1}", _refName, processInfo.TypeId);

            return processInfo;
        }

        [ClientSampleMethod]
        public List<ProcessWorkItemType> WorkItemTypes_List()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Getting list of work item types for '" + processId.ToString() + "'...");
            List<ProcessWorkItemType> list = client.GetProcessWorkItemTypesAsync(processId).Result;
            Console.WriteLine("success");

            foreach (var item in list)
            {
                Console.WriteLine("{0} : {1}", item.Name, item.ReferenceName);
            }

            return list;
        }

        [ClientSampleMethod]
        public ProcessWorkItemType WorkItemTypes_Create()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");

            ProcessWorkItemType processWorkItemType = null;

            CreateProcessWorkItemTypeRequest createWorkItemType = new CreateProcessWorkItemTypeRequest()
            {
                Name = "Change Request",
                Description = "Change request to track requests for changes :)",
                Color = "f6546a",
                Icon = "icon_airplane"
                //InheritsFrom = "Microsoft.VSTS.WorkItemTypes.UserStory"
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Does work item type '{0}' already exists? ... ", createWorkItemType.Name);

            //get list of work item types and see if wit exists
            List<ProcessWorkItemType> list = client.GetProcessWorkItemTypesAsync(processId).Result;
            processWorkItemType = list.Find(x => x.Name == "Change Request");

            if (processWorkItemType == null)
            {
                Console.WriteLine("No");
                Console.WriteLine("");
                Console.Write("Creating new work item type '" + createWorkItemType.Name + "'...");

                try
                {
                    //create new work item type
                    processWorkItemType = client.CreateProcessWorkItemTypeAsync(createWorkItemType, processId).Result;

                    Console.WriteLine("success");
                    Console.WriteLine("{0} : {1}", processWorkItemType.Name, processWorkItemType.ReferenceName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed");
                    Console.WriteLine("Error creating work item type: " + ex.InnerException.Message);
                }
            }
            else
            {
                Console.WriteLine("Yes");
                Console.WriteLine("{0} : {1}", processWorkItemType.Name, processWorkItemType.ReferenceName);
            }

            Context.SetValue<ProcessWorkItemType>("$newWorkItemType", processWorkItemType);

            return processWorkItemType;
        }

        [ClientSampleMethod]
        public ProcessWorkItemType WorkItemTypes_Update()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");

            //create UpdateProcessWorkItemTypeRequest object and set properties for whatever you want to change
            UpdateProcessWorkItemTypeRequest updateWorkItemType = new UpdateProcessWorkItemTypeRequest()
            {
                Description = "This is my description"
            };

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Updating description for 'Change Request' work item type...");
            ProcessWorkItemType result = client.UpdateProcessWorkItemTypeAsync(updateWorkItemType, processId, _witRefName).Result;
            Console.WriteLine("success");

            return result;
        }

        [ClientSampleMethod]
        public FormLayout Layout_Get()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Getting layout for '{0}'....", _witRefName);

            FormLayout layout = client.GetFormLayoutAsync(processId, _witRefName).Result;

            Console.WriteLine("success");
            Console.WriteLine("");

            List<Page> pages = layout.Pages as List<Page>;

            foreach(Page page in pages)
            {
                Console.WriteLine("{0} ({1})", page.Label, page.Id);

                foreach(Section section in page.Sections)
                {
                    Console.WriteLine("    {0}", section.Id);

                    foreach(Group group in section.Groups)
                    {
                        Console.WriteLine("        {0} ({1})", group.Label, group.Id);
                    }
                }
            }            

            return layout;
        }

        [ClientSampleMethod]
        public Group Group_Add()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");
          
            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Getting form layout to find all pages, sections, and groups...");

            FormLayout layout = client.GetFormLayoutAsync(processId, _witRefName).Result;

            //searching through the layout page to find the right page, section, and group           
            Page page = ProcessHelper.getPage(layout, "Details");
            Group group = ProcessHelper.getGroup(layout, "Details", "Section2", "NewGroup");           

            Console.WriteLine("done");
            
            if (group != null)
            {
                Console.WriteLine("Group '{0}' already exists on section '{1}' on page '{2}'", group.Label, "Section2", page.Label);
            }
            else
            {
                Console.Write("Creating new group 'NewGroup'...");

                Group newGroup = new Group()
                {
                    Controls = null,
                    Id = null,
                    Label = "NewGroup",
                    Overridden = false,
                    Visible = true,
                    Order = 1
                };

                group = client.AddGroupAsync(newGroup, processId, _witRefName, page.Id, "Section2").Result;

                Console.WriteLine("done");
            }                      

            return group;
        }
    }
}
