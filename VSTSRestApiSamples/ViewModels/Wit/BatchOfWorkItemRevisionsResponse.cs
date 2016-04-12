using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VstsRestApiSamples.ViewModels.Wit
{
    public class BatchOfWorkItemRevisionsResponse 
    {

        public class WorkItemRevisions : BaseViewModel
        {
            public Value[] values { get; set; }
            public string nextLink { get; set; }
            public string continuationToken { get; set; }
            public bool isLastBatch { get; set; }
        }

        public class Value
        {
            public int id { get; set; }
            public int rev { get; set; }
            public Fields fields { get; set; }
        }

        public class Fields
        {
            [JsonProperty(PropertyName = "System.Id")]
            public int SystemId { get; set; }

            [JsonProperty(PropertyName = "System.AreaPath")]
            public string SystemAreaPath { get; set; }

            [JsonProperty(PropertyName = "System.TeamProject")]
            public string SystemTeamProject { get; set; }

            [JsonProperty(PropertyName = "System.Rev")]
            public int SystemRev { get; set; }

            [JsonProperty(PropertyName = "System.RevisedDate")]
            public DateTime SystemRevisedDate { get; set; }

            [JsonProperty(PropertyName = "System.IterationPath")]
            public string SystemIterationPath { get; set; }

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

            [JsonProperty(PropertyName = "System.ChangedBy")]
            public string SystemChangedBy { get; set; }

            [JsonProperty(PropertyName = "System.IsDeleted")]
            public bool SystemIsDeleted { get; set; }

            [JsonProperty(PropertyName = "System.Title")]
            public string SystemTitle { get; set; }

            [JsonProperty(PropertyName = "Microsoft.VSTS.Common.Priority")]
            public int MicrosoftVSTSCommonPriority { get; set; }

            [JsonProperty(PropertyName = "WEF_DC53D4B8040948DCBF9B6360B7EA8857_Kanban.Column.Done")]
            public bool WEF_DC53D4B8040948DCBF9B6360B7EA8857_KanbanColumnDone { get; set; }

            [JsonProperty(PropertyName = "System.BoardColumn")]
            public string SystemBoardColumn { get; set; }

            [JsonProperty(PropertyName = "System.BoardColumnDone")]
            public bool SystemBoardColumnDone { get; set; }

            [JsonProperty(PropertyName = "WEF_DC53D4B8040948DCBF9B6360B7EA8857_Kanban.Column")]
            public string WEF_DC53D4B8040948DCBF9B6360B7EA8857_KanbanColumn { get; set; }
        }
    }
}
