using Newtonsoft.Json;
using System;

namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class AttachmentReference : BaseViewModel
    {       
        public string id { get; set; }
        public string url { get; set; }        
    }
}
