using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ClientSamples.ProjectsAndTeams
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.ProcessesRouteName)]
    public class ProcessesSample : ClientSample
    {
 
        [ClientSampleMethod]
        public List<Process> ListProcesses()
        {
            VssConnection connection = Context.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            List<Process> processes = processClient.GetProcessesAsync().Result;

            foreach(var process in processes)
            {
                Console.WriteLine("{0} {1} {2}", (process.IsDefault ? "*" : " "), process.Name.PadRight(12), process.Id);
            }

            return processes;
        }

        [ClientSampleMethod]
        public Process GetProcess()
        {
            Guid scrumProcessId = Guid.Parse("adcc42ab-9882-485e-a3ed-7678f01f66bc");

            VssConnection connection = Context.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            Process process = processClient.GetProcessByIdAsync(scrumProcessId).Result;

            Console.WriteLine("Name:      {0}", process.Name);
            Console.WriteLine("Default?:  {0}", process.IsDefault);
            Console.WriteLine("Type:      {0}", process.Type);
            Console.WriteLine("Description:\n{0}", process.Description);

            return process;
        }
    }
}
