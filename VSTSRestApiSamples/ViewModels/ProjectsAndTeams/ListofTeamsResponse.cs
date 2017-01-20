namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class ListofTeamsResponse
    {

        public class Teams : BaseViewModel
        {
            public Value[] value { get; set; }
            public int count { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string description { get; set; }
            public string identityUrl { get; set; }
        }

    }
}
