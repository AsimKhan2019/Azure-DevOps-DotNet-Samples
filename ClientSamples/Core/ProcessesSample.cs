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
        public List<Process> GetProcesses()
        {
            VssConnection connection = Context.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            List<Process> processes = processClient.GetProcessesAsync().Result;

            return processes;
        }

        [ClientSampleMethod]
        public Process GetProcess(System.Guid processId)
        {
            VssConnection connection = Context.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            Process process = processClient.GetProcessByIdAsync(processId).Result;

            return process;
        }
    }
}
