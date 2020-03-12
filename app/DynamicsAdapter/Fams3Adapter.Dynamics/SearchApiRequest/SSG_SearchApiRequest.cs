using System;
using System.Collections.Generic;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Identifier;
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

        [JsonProperty(Keys.DYNAMICS_STATUS_CODE_FIELD)]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_name")]
        public string Name { get; set; }

       [JsonProperty("ssg_ssg_identifier_ssg_searchapirequest")]
        public SSG_Identifier[] Identifiers { get; set; }

        [JsonProperty("ssg_ssg_searchapirequest_ssg_sapirdataprovide")]
        public SSG_SearchapiRequestDataProvider[] DataProviders { get; set; }
    }
}
