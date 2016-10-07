using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Git
{
    public class GetCommitsByRepositoryIdResponse
    {
        public class Commits : BaseViewModel
        {
            public int count { get; set; }
            public List<Value> value { get; set; }
        }
        public class Author
        {
            public string name { get; set; }
            public string email { get; set; }
            public string date { get; set; }
        }

        public class Committer
        {
            public string name { get; set; }
            public string email { get; set; }
            public string date { get; set; }
        }

        public class ChangeCounts
        {
            public int Add { get; set; }
            public int Edit { get; set; }
        }

        public class Value
        {
            public string commitId { get; set; }
            public Author author { get; set; }
            public Committer committer { get; set; }
            public string comment { get; set; }
            public ChangeCounts changeCounts { get; set; }
            public string url { get; set; }
            public string remoteUrl { get; set; }
        }


    }
}