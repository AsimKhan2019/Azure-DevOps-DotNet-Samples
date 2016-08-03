namespace VstsRestApiSamples.ViewModels.WorkItemTracking.Queries
{
    public class ListofQueriesResponse 
    {
        public class Queries : BaseViewModel
        {
            public int count { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public bool isFolder { get; set; }
            public bool hasChildren { get; set; }
            public Child[] children { get; set; }
            public bool isPublic { get; set; }
            public string url { get; set; }
        }

        public class Child
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public bool isPublic { get; set; }
            public string url { get; set; }
            public bool isFolder { get; set; }
            public bool hasChildren { get; set; }
        }
    }
}
