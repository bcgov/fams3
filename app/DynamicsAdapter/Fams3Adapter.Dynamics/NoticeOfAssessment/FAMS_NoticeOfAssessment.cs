using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.NoticeOfAssessment
{
    public class NoticeOfAssessmentEntity : DynamicsEntity
    {
        [JsonProperty("fams_name")]
        [Description("Tax Year")]
        public string TaxYear { get; set; }

        [JsonProperty("fams_taxamount")]
        [Description("Tax Amount")]
        public string TaxAmount { get; set; }

        [JsonProperty("fams_description")]
        [Description("Description text")]
        public string Description { get; set; }

        [JsonProperty("fams_datadate")]
        [Description("Date")]
        public DateTime? Date { get; set; }

        [JsonProperty("fams_suppliedby")]
        [Description("Supplied By")]
        public int? InformationSource { get; set; }

        [JsonProperty("fams_notes")] // This is saved to Data Provider - Notes
        [Description("Text for the Trace Status Code")]
        public string TaxTraceStatusText { get; set; }

        [JsonProperty("fams_jcacode")]
        [Description("JCA Code")]
        public string JCACode { get; set; }

        [JsonProperty("fams_searchrequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        /// <summary>OData v4 lookup backing field — used for $filter queries without navigation property expansion.</summary>
        [JsonProperty("_fams_searchrequest_value")]
        public Guid SearchRequestId { get; set; }
    }

    public class FAMS_NoticeOfAssessment : NoticeOfAssessmentEntity
    {
        [JsonProperty("fams_noticeofassessmentid")]
        public Guid NoticeOfAssessmentId { get; set; }
    }
}
