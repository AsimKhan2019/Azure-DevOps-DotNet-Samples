using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Work
{
    class PickListResponse
    {

        public class PickList
        {
            public Item[] items { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public string value { get; set; }
        }

    }
}
