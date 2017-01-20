using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class GetRestoreMultipleWorkItemsResponse
    {
        public class Items : BaseViewModel
        {
            public int count { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            public int code { get; set; }
            public Headers headers { get; set; }
            public string body { get; set; }
        }

        public class Headers
        {
            public string ContentType { get; set; }
        }
    }
}
