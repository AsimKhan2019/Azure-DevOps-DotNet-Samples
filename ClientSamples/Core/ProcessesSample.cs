using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Vsts.ClientSamples.Core
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

            return processes;
        }

        [ClientSampleMethod]
        public Process GetProcess()
        {
            Guid scrumProcessId = Guid.Parse("adcc42ab-9882-485e-a3ed-7678f01f66bc");

            VssConnection connection = Context.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            Process process = processClient.GetProcessByIdAsync(scrumProcessId).Result;

            return process;
        }
    }
}
