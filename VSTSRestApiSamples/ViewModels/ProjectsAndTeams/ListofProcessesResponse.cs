namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class ListofProcessesResponse
    {
        public class Projects : BaseViewModel  
        {
            public int count { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string description { get; set; }
            public bool isDefault { get; set; }
            public string url { get; set; }
            public string name { get; set; }
        }
    }
}
