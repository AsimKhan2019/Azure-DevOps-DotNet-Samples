namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class ListofWorkItemFieldsResponse
    {
        public class Fields : BaseViewModel
        {
            public int count { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            public string name { get; set; }
            public string referenceName { get; set; }
            public string type { get; set; }
            public bool readOnly { get; set; }
            public Supportedoperation[] supportedOperations { get; set; }
            public string url { get; set; }
        }

        public class Supportedoperation
        {
            public string referenceName { get; set; }
            public string name { get; set; }
        }
    }
}
