using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Work
{
    public class PickListPost
    {
        public class PickList
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public Item[] Items { get; set; }
        }

        public class Item
        {
            public string value { get; set; }
        }

    }
}
