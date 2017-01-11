using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class GetItemsFromRecycleBinResponse
    {
        public class WorkItems : BaseViewModel
        {
            public Value[] value { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string type { get; set; }
            public string Name { get; set; }
            public string Project { get; set; }
            public DateTime DeletedDate { get; set; }
            public string DeletedBy { get; set; }
            public string url { get; set; }
        }

    }
}
