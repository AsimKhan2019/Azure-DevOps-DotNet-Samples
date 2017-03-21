using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;


namespace VstsClientLibrariesSamples.ProjectsAndTeams
{
    public class Processes
    {
        readonly IConfiguration _configuration;
        private VssBasicCredential _credentials;
        private Uri _uri;

        public Processes(IConfiguration configuration)
        {
            _configuration = configuration;
            _credentials = new VssBasicCredential("", _configuration.PersonalAccessToken);
            _uri = new Uri(_configuration.UriString);
        }

        public List<Process> GetProcesses()
        {
            // Create instance of VssConnection using passed credentials
            VssConnection connection = new VssConnection(_uri, _credentials);
            ProcessHttpClient processHttpClient = connection.GetClient<ProcessHttpClient>();

            List<Process> processes = processHttpClient.GetProcessesAsync().Result;
            return processes;
        }

        public Process GetProcess(System.Guid processId)
        {
            // create project object
            using (ProcessHttpClient processHttpClient = new ProcessHttpClient(_uri, _credentials))
            {
                Process process = processHttpClient.GetProcessByIdAsync(processId).Result;
                return process;
            }
        }
    }
}
