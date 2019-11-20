using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fams3Adapter.Dynamics.SearchRequest
{
        public class SSG_Identifier
        {
            [JsonProperty("ssg_identification")]
            public string SSG_Identification { get; set; }
            [JsonProperty("statuscode")]
            public int StatusCode { get; set; }
            [JsonProperty("statecode")]
            public int StateCode { get; set; }
            [JsonProperty("ssg_identificationeffectivedate")]
            public DateTime ssg_identificationeffectivedate { get; set; }
            [JsonProperty("ssg_SearchRequest")]
            public virtual SSG_SearchRequest SSG_SearchRequest { get; set; }

            [JsonProperty("ssg_identificationcategorytext")]
            public int IdentifierType { get; set; }

            [JsonProperty("ssg_issuedby")]
            public string IssuedBy { get; set; }


        }

}
