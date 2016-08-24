namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class GetQueryResponse
    {
        public class Queries : BaseViewModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string path { get; set; }
            public bool isPublic { get; set; }
            public _Links _links { get; set; }
            public string url { get; set; }
        }

        public class _Links
        {
            public Self self { get; set; }
            public Html html { get; set; }
            public Parent parent { get; set; }
            public Wiql wiql { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Html
        {
            public string href { get; set; }
        }

        public class Parent
        {
            public string href { get; set; }
        }

        public class Wiql
        {
            public string href { get; set; }
        }

    }
}
