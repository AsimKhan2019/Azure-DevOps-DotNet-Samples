namespace VstsRestApiSamples.ViewModels.Work
{
    public class FieldsPostResponse
    {
        public class Field : BaseViewModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string description { get; set; }
            public string listId { get; set; }
            public string url { get; set; }
        }

    }
}
