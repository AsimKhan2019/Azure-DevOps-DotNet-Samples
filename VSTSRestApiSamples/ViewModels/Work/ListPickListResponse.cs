namespace VstsRestApiSamples.ViewModels.Work
{
    public class ListPickListResponse
    {
        public class PickList : BaseViewModel
        {
            public int count { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }
    }
}
