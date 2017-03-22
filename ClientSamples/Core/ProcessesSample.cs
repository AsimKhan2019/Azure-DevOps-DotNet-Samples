using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace VstsSamples.Client.Core
{
    [ClientSample(CoreConstants.AreaName, CoreConstants.ProcessesRouteName)]
    public class ProcessesSample : ClientSample
    {
        public ProcessesSample(ClientSampleConfiguration configuration) : base(configuration)
        {
        }

        [ClientSampleMethod]
        public List<Process> GetProcesses()
        {
            VssConnection connection = this.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            List<Process> processes = processClient.GetProcessesAsync().Result;

            return processes;
        }

        [ClientSampleMethod]
        public Process GetProcess(System.Guid processId)
        {
            VssConnection connection = this.Connection;
            ProcessHttpClient processClient = connection.GetClient<ProcessHttpClient>();

            Process process = processClient.GetProcessByIdAsync(processId).Result;

            return process;
        }
    }
}
