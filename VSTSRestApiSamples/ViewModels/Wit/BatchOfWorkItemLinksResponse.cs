using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Wit
{
    public class BatchOfWorkItemLinksResponse
    {
        public class WorkItemLinks : BaseViewModel
        {
            public Value[] values { get; set; }
            public string nextLink { get; set; }
            public bool isLastBatch { get; set; }
        }

        public class Value
        {
            public string rel { get; set; }
            public int sourceId { get; set; }
            public int targetId { get; set; }
            public DateTime changedDate { get; set; }
            public bool isActive { get; set; }
        }
    }
}
