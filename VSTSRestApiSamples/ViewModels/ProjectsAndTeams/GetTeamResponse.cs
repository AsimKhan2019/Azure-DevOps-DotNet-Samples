namespace VstsRestApiSamples.ViewModels.ProjectsAndTeams
{
    public class GetTeamResponse
    {
        public class Team : BaseViewModel
        {
            public string id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string description { get; set; }
            public string identityUrl { get; set; }
        }

    }
}
