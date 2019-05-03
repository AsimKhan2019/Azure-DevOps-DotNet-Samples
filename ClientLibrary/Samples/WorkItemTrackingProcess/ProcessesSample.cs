using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.WorkItemTrackingProcess
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
        private string _refName = "fabrikam.MyNewAgileProcessThree";
        private string _witRefName = "MyNewAgileProcess.ChangeRequest";
        private string _fieldRefName = "Custom.Fields.Colors";

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
                Name = "My New Agile Process",
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
        public ProcessInfo Process_GetById()
        {

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();
            List<ProcessInfo> processes = this.Process_List();

            var processInfo = client.GetProcessByItsIdAsync(Context.GetValue<Guid>("$processId")).Result;

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
        public List<ProcessWorkItemType> WorkItemTypes_List_Expand_State()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            Console.Write("Getting list of work item types for '" + processId.ToString() + "'...");
            List<ProcessWorkItemType> list = client.GetProcessWorkItemTypesAsync(processId, expand: GetWorkItemTypeExpand.States).Result;
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
        public ProcessWorkItemType WorkItemType_Get()
        {
            ProcessWorkItemType processWorkItemType = null;

            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            //load the process by id
            processWorkItemType = client.GetProcessWorkItemTypeAsync(processId, _witRefName).Result;

            Console.WriteLine("Getting work item type for " + _refName);

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
        public PickListMetadata Field_CreatePicklist()
        {           
            List<PickListMetadata> pickListMetadata = null;
            PickList picklist = null;
            string pickListName = "colorsPicklist";

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();
          
            Console.Write("Searching to see if picklist '{0}' exists....", pickListName);
               
            pickListMetadata = client.GetListsMetadataAsync().Result;
            PickListMetadata item = pickListMetadata.Find(x => x.Name == pickListName);
                        
            if (item != null)
            {
                Context.SetValue<Guid>("$picklistId", item.Id);
                Console.WriteLine("picklist found");
                
                return item;
            }
            else
            { 
                Console.WriteLine("picklist not found");
                Console.Write("Creating new picklist....");

                IList<string> list = new List<string>();

                list.Add("Blue");
                list.Add("Green");
                list.Add("Red");
                list.Add("Purple");

                picklist = new PickList()
                {
                    Name = pickListName,
                    Items = list,
                    Type = "String",                
                    IsSuggested = false
                };               

                PickList result = client.CreateListAsync(picklist).Result;
                Context.SetValue<Guid>("$picklistId", result.Id);

                Console.WriteLine("done");
                return result;
            }           
        }

        [ClientSampleMethod]
        public WorkItemField Field_CreatePicklistField()
        {
            //get process id stored in cache so we don't have to load it each time
            System.Guid processId = Context.GetValue<Guid>("$processId");
            System.Guid picklistId = Context.GetValue<Guid>("$picklistId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();
            WorkItemField field = null;

            Console.Write("Searching to see if field '{0}' exists....", _fieldRefName);

            try
            {
                field = client.GetFieldAsync(_fieldRefName).Result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("TF51535: Cannot find field"))
                {
                    field = null;
                }
            }

            if (field == null)
            {
                Console.WriteLine("field not found");
                Console.Write("Creating new picklist field and setting it to former picklistId....");

                field = new WorkItemField()
                {
                    ReferenceName = _fieldRefName,
                    Name = "Colors",
                    Description = "My new field",
                    Type = TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.String,
                    IsPicklist = true,
                    PicklistId = picklistId,
                    Usage = FieldUsage.WorkItem,
                    ReadOnly = false,
                    IsIdentity = false,
                    IsQueryable = true
                };

                WorkItemField newField = client.CreateFieldAsync(field).Result;

                Console.WriteLine("Done");
                return newField;
            }

            Console.WriteLine("field found");
            return field;
        }

        [ClientSampleMethod]
        public ProcessWorkItemTypeField Field_AddFieldToWorkItemType()
        {
            ProcessWorkItemTypeField processWorkItemTypeField = null;
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            //get the list of fields on the work item item
            Console.Write("Loading list of fields on the work item and checking to see if field '{0}' already exists...", _fieldRefName);

            List<ProcessWorkItemTypeField> list = client.GetAllWorkItemTypeFieldsAsync(processId, _witRefName).Result;

            //check to see if the field already exists on the work item
            processWorkItemTypeField = list.Find(x => x.ReferenceName == _fieldRefName);

            //field is already on the work item, so just return it
            if (processWorkItemTypeField != null)
            {
                Console.WriteLine("field found");
                return processWorkItemTypeField;
            }
            else
            {
                //the field is not on the work item, so we best add it
                Console.WriteLine("field not found");
                Console.Write("Adding field to work item...");

                AddProcessWorkItemTypeFieldRequest fieldRequest = new AddProcessWorkItemTypeFieldRequest()
                {
                    AllowGroups = false,
                    DefaultValue = String.Empty,
                    ReadOnly = false,
                    ReferenceName = _fieldRefName,
                    Required = false
                };

                processWorkItemTypeField = client.AddFieldToWorkItemTypeAsync(fieldRequest, processId, _witRefName).Result;

                Console.WriteLine("done");

                return processWorkItemTypeField;
            }
        }

        [ClientSampleMethod]
        public ProcessWorkItemTypeField Field_AddSystemFieldToWorkItemType()
        {
            string fieldName = "Microsoft.VSTS.Common.Activity";

            ProcessWorkItemTypeField processWorkItemTypeField = null;
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            //get the list of fields on the work item item
            Console.Write("Loading list of fields on the work item and checking to see if field '{0}' already exists...", fieldName);

            List<ProcessWorkItemTypeField> list = client.GetAllWorkItemTypeFieldsAsync(processId, _witRefName).Result;

            //check to see if the field already exists on the work item
            processWorkItemTypeField = list.Find(x => x.ReferenceName == fieldName);

            //field is already on the work item, so just return it
            if (processWorkItemTypeField != null)
            {
                Console.WriteLine("field found");
                return processWorkItemTypeField;
            }
            else
            {
                //the field is not on the work item, so we best add it
                Console.WriteLine("field not found");
                Console.Write("Adding field to work item...");

                AddProcessWorkItemTypeFieldRequest fieldRequest = new AddProcessWorkItemTypeFieldRequest()
                {
                    AllowGroups = false,
                    DefaultValue = String.Empty,
                    ReadOnly = false,
                    ReferenceName = fieldName,
                    Required = false
                };

                processWorkItemTypeField = client.AddFieldToWorkItemTypeAsync(fieldRequest, processId, _witRefName).Result;

                Console.WriteLine("done");

                return processWorkItemTypeField;
            }
        }

        [ClientSampleMethod]
        public Group Group_AddWithFields()
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

                List<Control> controlList = new List<Control>()
                {
                    new Control() { Id = _fieldRefName, Order = 1, Label = "Colors", Visible = true, Name = "Colors", Watermark = "Select a color" },
                    new Control() { Id = "Microsoft.VSTS.Common.Activity", Order = 2, Label = "Activity", Visible = true }
                };

                Group newGroup = new Group()
                {
                    Controls = controlList,
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

        [ClientSampleMethod]
        public List<ProcessWorkItemTypeField> Field_GetAllWorkItemTypeFieldsAsync()
        {
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            //get the list of fields on the work item item
            Console.Write("Loading list of fields on the work item and checking to see if field '{0}' already exists...", _fieldRefName);

            List<ProcessWorkItemTypeField> list = client.GetAllWorkItemTypeFieldsAsync(processId, _witRefName).Result;

            return list;
        }

        [ClientSampleMethod]
        public ProcessWorkItemTypeField Field_GetWorkItemTypeField()
        {
            ProcessWorkItemTypeField processWorkItemTypeField = null;
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            processWorkItemTypeField = client.GetWorkItemTypeFieldAsync(processId, _witRefName, _fieldRefName).Result;

            return processWorkItemTypeField;
        }

        [ClientSampleMethod]
        public ProcessWorkItemTypeField Field_UpdateWorkItemTypeField()
        {
            UpdateProcessWorkItemTypeFieldRequest newfieldRequest = new UpdateProcessWorkItemTypeFieldRequest()
            {
                DefaultValue = "Blue"
            };
 
            System.Guid processId = Context.GetValue<Guid>("$processId");

            VssConnection connection = Context.Connection;
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            ProcessWorkItemTypeField processWorkItemTypeField = client.UpdateWorkItemTypeFieldAsync(newfieldRequest, processId, _witRefName, _fieldRefName).Result;

            return processWorkItemTypeField;
        }
    }
}
