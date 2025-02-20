using Newtonsoft.Json;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.TaxIncomeInformation
{
    public class FAMS_TaxCode
    {
        [JsonProperty("fams_cracode")]
        [Description("CRA Code")]
        public string TaxCode { get; set; }


        [JsonProperty("fams_description")]
        [Description("Description")]
        public string Description { get; set; }

        [JsonProperty("fams_formschedule")]
        [Description("Form Schedule")]
        public string FormSchedule { get; set; }

        [JsonProperty("fams_name")]
        [Description("Name")]
        public string Name { get; set; }

        [JsonProperty("fams_newfieldnumber")]
        [Description("New Field Number")]
        public string NewFieldNumber { get; set; }

        [JsonProperty("fams_oldfieldnumber")]
        [Description("Old Field Number")]
        public string OldFieldNumber { get; set; }
    }
}
