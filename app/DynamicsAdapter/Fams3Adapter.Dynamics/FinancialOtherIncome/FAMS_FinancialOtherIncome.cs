using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.FinancialOtherIncome
{
    public class FinancialOtherIncomeEntity : DynamicsEntity
    {
        [JsonProperty("fams_couldnotlocate")]
        [Description("Could Not Locate")]
        public bool CouldNotLocate { get; set; }

        [JsonProperty("fams_description")]
        [Description("Description text")]
        public string Description { get; set; }

        [JsonProperty("fams_taxamount")]
        [Description("Tax Amount")]
        public string TaxAmount { get; set; }

        [JsonProperty("fams_form")]
        [Description("Tax Form")]
        public string Form { get; set; }

        [JsonProperty("fams_name")]
        [Description("Tax Year")]
        public string TaxYear { get; set; }

        [JsonProperty("fams_datadate")]
        [Description("Date")]
        public System.DateTime? Date { get; set; }

        [JsonProperty("fams_datadatelabel")]
        [Description("Date Type")]
        public string DateLabel { get; set; }

        [JsonProperty("fams_personid")]
        [Description("Person")]
        public virtual SSG_Person Person { get; set; }

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
    }

    public class FAMS_FinancialOtherIncome : FinancialOtherIncomeEntity
    {
        [JsonProperty("fams_financialotherincomeid")]
        public Guid FinancialOtherIncomeId { get; set; }
    }
}
