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

        public class Value
        {
            public string rel { get; set; }
            public string url { get; set; }
            public Attributes attributes { get; set; }
        }

        public class Attributes
        {
            public string comment { get; set; }
        }
    }
}
