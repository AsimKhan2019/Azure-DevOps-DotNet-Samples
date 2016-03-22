using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Work
{
    public class FieldsPost
    {
        public class Field
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string ListId { get; set; }
        }
    }
}
