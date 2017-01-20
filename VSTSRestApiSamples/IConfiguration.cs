namespace VstsRestApiSamples
{
    public interface IConfiguration
    {
        string AccountName { get; set; }
        // This is the ID of the application registerd with the Azure portal
        // Requirements: Application must have permissions to access the VSTS Resource
        //      Since this is currently not possible through the UX, using the VS client AppId
        string ApplicationId { get; set; }
        string CollectionId { get; set; }
        string PersonalAccessToken { get; set; }
        string Project { get; set; }
        string Team { get; set; }
        string MoveToProject { get; set; }
        string UriString { get; }        
        string Query { get; set; }
        string Identity { get; set; }
        string WorkItemIds { get; set; }
        string WorkItemId { get; set; }
        string ProcessId { get; set; }
        string PickListId { get; set; }
        string QueryId { get; set; }
        string FilePath { get; set; }
        string GitRepositoryId { get; set; }
    }
}