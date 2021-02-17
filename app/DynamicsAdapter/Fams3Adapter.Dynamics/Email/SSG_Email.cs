using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Email
{
    public class EmailEntity : DynamicsEntity
    {
        [JsonProperty("ssg_emailcategorytext")]
        public int? EmailType { get; set; }

        [JsonProperty("ssg_email")]
        public string Email { get; set; }

        //[JsonProperty("ssg_suppliedby")]
        //public int? InformationSource { get; set; }

        [JsonProperty("ssg_suppliedbytext")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

    }

    public class SSG_Email : EmailEntity
    {
        [JsonProperty("ssg_emailid")]
        public Guid EmailId { get; set; }

    }
}
