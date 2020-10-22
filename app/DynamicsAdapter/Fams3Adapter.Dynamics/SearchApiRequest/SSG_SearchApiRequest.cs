using System;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.SearchApiRequest
{
    public class SSG_SearchApiRequest
    {
        [JsonProperty("ssg_searchapirequestid")]
        public Guid SearchApiRequestId { get; set; }

        [JsonProperty("ssg_persongivenname")]
        public string PersonGivenName { get; set; }

        [JsonProperty("ssg_personsurname")]
        public string PersonSurname { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        public string PersonMiddleName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        public string PersonThirdGivenName { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        public DateTime? PersonBirthDate { get; set; }

        [JsonProperty("_ssg_searchrequest_value")]
        public Guid SearchRequestId { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty(Keys.DYNAMICS_STATUS_CODE_FIELD)]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_name")]
        public string SequenceNumber { get; set; }

        [JsonProperty("ssg_isprescreeningautosearch")]
        public bool IsPrescreenSearch { get; set; }

        [JsonProperty("ssg_ssg_identifier_ssg_searchapirequest")]
        public SSG_Identifier[] Identifiers { get; set; }

        [JsonProperty("ssg_ssg_searchapirequest_ssg_sapirdataprovide")]
        public SSG_SearchapiRequestDataProvider[] DataProviders { get; set; }

        public bool IsFailed { get; set; }

        [JsonProperty("ssg_jcafirstname")]
        public string JCAFirstName { get; set; }

        [JsonProperty("ssg_jcalastname")]
        public string JCALastName { get; set; }

        [JsonProperty("ssg_jcamiddlename")]
        public string JCAMiddleName { get; set; }

        [JsonProperty("ssg_jcamotherssurnameatbirth")]
        public string JCAMotherBirthSurname { get; set; }

        [JsonProperty("ssg_jcadateofbirth")]
        public DateTime? JCAPersonBirthDate { get; set; }

        [JsonProperty("ssg_jcanotes")]
        public string JCANotes { get; set; }

        [JsonProperty("ssg_jcagender")]
        public int? JCAGender { get; set; }

    }
}
