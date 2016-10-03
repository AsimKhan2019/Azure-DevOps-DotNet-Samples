using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System;

namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{ 
    public class WorkItemBatchPostResponse
    {
        public int count { get; set; }
        [JsonProperty("value")]
        public List<Value> values { get; set; }

        public class Value
        {
            public int code { get; set; }
            public Dictionary<string,string> headers { get; set; }
            public string body { get; set; }
        }
    }
}