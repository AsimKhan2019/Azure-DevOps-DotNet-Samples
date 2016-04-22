using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Wit
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
