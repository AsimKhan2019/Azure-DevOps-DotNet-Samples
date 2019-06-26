using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Audit.WebApi;

namespace Microsoft.Azure.DevOps.ClientSamples.Audit
{
    [ClientSample(ResourceLocationIds.AuditAreaName, ResourceLocationIds.AuditLogDownloadResourceName)]
    public class DownloadLogSample : ClientSample
    {
        [ClientSampleMethod]
        public async Task DownloadSampleAsync()
        {
            // Get the audit log client
            VssConnection connection = Context.Connection;
            AuditHttpClient auditClient = await connection.GetClientAsync<AuditHttpClient>();

            // Download the log to a file
            foreach (string format in new[] { "json", "csv" })
            {
                string fileName = $"{Path.GetTempFileName()}.{format}";
                using (FileStream fileStream = File.Create(fileName))
                using (Stream logStream = await auditClient.DownloadLogAsync(format))
                {
                    await logStream.CopyToAsync(fileStream);
                }
                Context.Log($"Log downloaded to {fileName}");
            }
        }
    }
}
