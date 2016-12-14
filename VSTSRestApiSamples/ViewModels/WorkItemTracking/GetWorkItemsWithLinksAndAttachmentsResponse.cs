using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.WorkItemTracking
{
    public class GetWorkItemsWithLinksAndAttachmentsResponse
    {
        public class WorkItems : BaseViewModel
        {
            public int count { get; set; }
            public Value[] value { get; set; }
        }

        public class Value
        {
            public int id { get; set; }
            public int rev { get; set; }
            public Fields fields { get; set; }
            public Relation[] relations { get; set; }
            public _Links _links { get; set; }
            public string url { get; set; }
        }

        public class Fields
        {
            [JsonProperty(PropertyName = "System.Id")]
            public int SystemId { get; set; }

            [JsonProperty(PropertyName = "System.AreaId")]
            public int SystemAreaId { get; set; }

            [JsonProperty(PropertyName = "System.AreaPath")]
            public string SystemAreaPath { get; set; }

            [JsonProperty(PropertyName = "System.NodeName")]
            public string SystemNodeName { get; set; }

            [JsonProperty(PropertyName = "System.TeamProject")]
            public string SystemTeamProject { get; set; }

            [JsonProperty(PropertyName = "System.AreaLevel1")]
            public string SystemAreaLevel1 { get; set; }

            [JsonProperty(PropertyName = "System.Rev")]
            public int SystemRev { get; set; }

            [JsonProperty(PropertyName = "System.AuthorizedDate")]
            public DateTime SystemAuthorizedDate { get; set; }

            [JsonProperty(PropertyName = "System.RevisedDate")]
            public DateTime SystemRevisedDate { get; set; }

            [JsonProperty(PropertyName = "System.IterationId")]
            public int SystemIterationId { get; set; }

            [JsonProperty(PropertyName = "System.IterationPat")]
            public string SystemIterationPath { get; set; }

            [JsonProperty(PropertyName = "System.IterationLevel1")]
            public string SystemIterationLevel1 { get; set; }

            [JsonProperty(PropertyName = "System.WorkItemType")]
            public string SystemWorkItemType { get; set; }

            [JsonProperty(PropertyName = "System.State")]
            public string SystemState { get; set; }

            [JsonProperty(PropertyName = "System.Reason")]
            public string SystemReason { get; set; }

            [JsonProperty(PropertyName = "System.CreatedDate")]
            public DateTime SystemCreatedDate { get; set; }

            [JsonProperty(PropertyName = "System.CreatedBy")]
            public string SystemCreatedBy { get; set; }

            [JsonProperty(PropertyName = "System.ChangedDate")]
            public DateTime SystemChangedDate { get; set; }

            [JsonProperty(PropertyName = "")]
            public string SystemChangedBy { get; set; }

            [JsonProperty(PropertyName = "System.AuthorizedAs")]
            public string SystemAuthorizedAs { get; set; }

            [JsonProperty(PropertyName = "System.PersonId")]
            public int SystemPersonId { get; set; }

            [JsonProperty(PropertyName = "System.Watermark")]
            public int SystemWatermark { get; set; }

            [JsonProperty(PropertyName = "System.Title")]
            public string SystemTitle { get; set; }

            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.Effort")]
            public int MicrosoftVSTSSchedulingEffort { get; set; }

            [JsonProperty(PropertyName = "System.Description")]
            public string SystemDescription { get; set; }

            [JsonProperty(PropertyName = "System.AssignedTo")]
            public string SystemAssignedTo { get; set; }

            [JsonProperty(PropertyName = "Microsoft.VSTS.Scheduling.RemainingWork")]
            public int MicrosoftVSTSSchedulingRemainingWork { get; set; }

            [JsonProperty(PropertyName = "System.Tags")]
            public string SystemTags { get; set; }
        }

        public class _Links
        {
            public Self self { get; set; }
            public Workitemupdates workItemUpdates { get; set; }
            public Workitemrevisions workItemRevisions { get; set; }
            public Workitemhistory workItemHistory { get; set; }
            public Html html { get; set; }
            public Workitemtype workItemType { get; set; }
            public Fields1 fields { get; set; }
        }

        public class Self
        {
            public string href { get; set; }
        }

        public class Workitemupdates
        {
            public string href { get; set; }
        }

        public class Workitemrevisions
        {
            public string href { get; set; }
        }

        public class Workitemhistory
        {
            public string href { get; set; }
        }

        public class Html
        {
            public string href { get; set; }
        }

        public class Workitemtype
        {
            public string href { get; set; }
        }

        public class Fields1
        {
            public string href { get; set; }
        }

        public class Relation
        {
            public string rel { get; set; }
            public string url { get; set; }
            public Attributes attributes { get; set; }
        }

        public class Attributes
        {
            public bool isLocked { get; set; }
            public string comment { get; set; }
            public DateTime authorizedDate { get; set; }
            public int id { get; set; }
            public DateTime resourceCreatedDate { get; set; }
            public DateTime resourceModifiedDate { get; set; }
            public DateTime revisedDate { get; set; }
            public string name { get; set; }
        }

    }
}
