using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Wit
{
    public class WorkItemPatch
    {
        public class Field
        {
            public string op { get; set; }
            public string path { get; set; }
            public object value { get; set; }
        }
    }
}
