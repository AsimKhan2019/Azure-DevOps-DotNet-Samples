using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Git
{
    public class GetFolderAndChildrenResponse
    {
        public class FolderAndChildren : BaseViewModel
        {
            public int count { get; set; }
            public List<Value> value { get; set; }
        }

        public class ContentMetadata
        {
            public string fileName { get; set; }
        }

        public class Value
        {
            public string objectId { get; set; }
            public string gitObjectType { get; set; }
            public string commitId { get; set; }
            public string path { get; set; }
            public bool isFolder { get; set; }
            public ContentMetadata contentMetadata { get; set; }
            public string url { get; set; }
        }


    }
}