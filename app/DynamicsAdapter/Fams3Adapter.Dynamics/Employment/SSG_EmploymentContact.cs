using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Employment
{
    public class EmploymentContactEntity
    {

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_emailaddress")]
        public string Email { get; set; }

        [JsonProperty("ssg_faxnumber")]
        public string FaxNumber { get; set; }

        [JsonProperty("ssg_phoneextension")]
        public string PhoneExtension { get; set; }

        [JsonProperty("ssg_phonenumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("ssg_phonetype")]
        public int? PhoneType { get; set; }

        [JsonProperty("ssg_phonetypetext")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_EmploymentId")]
        public virtual SSG_Employment Employment { get; set; }

        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }
    }

    public class SSG_EmploymentContact : EmploymentContactEntity
    {
        [JsonProperty("ssg_employmentcontactid")]
        public Guid EmploymentContactId { get; set; }
    }

}
