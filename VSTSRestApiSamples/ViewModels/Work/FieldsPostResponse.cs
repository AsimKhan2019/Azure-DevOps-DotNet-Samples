using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Work
{
    class FieldsPostResponse
    {
        public class Field
        {
            public string id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string description { get; set; }
            public string listId { get; set; }
            public string url { get; set; }
        }

    }
}
