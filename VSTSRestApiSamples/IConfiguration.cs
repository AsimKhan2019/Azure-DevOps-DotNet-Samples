
namespace VstsRestApiSamples
{
    public interface IConfiguration
    {        
        string PersonalAccessToken { get; set; }
        string Project { get; set; }
        string MoveToProject { get; set; }
        string UriString { get; set; }        
        string Query { get; set; }
        string Identity { get; set; }
        string WorkItemIds { get; set; }
        string WorkItemId { get; set; }
        string ProcessId { get; set; }
        string PickListId { get; set; }
        string QueryId { get; set; }
    }
}