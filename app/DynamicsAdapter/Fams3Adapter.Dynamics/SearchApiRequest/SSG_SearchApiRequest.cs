using System;
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
        public DateTime PersonBirthDate { get; set; }
        public int StatusCode { get; set; }

    }
}
