namespace VstsRestApiSamples.Client.Helpers
{
    public interface IAuth
    {
        string Account { get; }
        string Login { get; }
        string Project { get; }
        string ProcessId { get; }
        string PickListId { get; }
    }
}