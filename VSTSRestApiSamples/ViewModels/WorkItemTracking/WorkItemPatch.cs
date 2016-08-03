namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class WorkItemPatch
    {
        public class Field
        {
            public string op { get; set; }
            public string path { get; set; }
            public object value { get; set; }
        }
    }
}
