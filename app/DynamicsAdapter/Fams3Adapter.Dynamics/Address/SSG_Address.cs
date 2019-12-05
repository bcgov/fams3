using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.Address
{
    public class SSG_Address : DynamicsEntity
    {
        [JsonProperty("ssg_addressfulltext")]
        public string AddressFullText { get; set; }

        [JsonProperty("ssg_addresscategorytext")]
        public int AddressCategory { get; set; }

        [JsonProperty("ssg_address")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_locationcanadianprovincecode")]
        public int Province { get; set; }

        [JsonProperty("ssg_locationcityname")]
        public string City { get; set; }

        [JsonProperty("ssg_locationcountry")]
        public int SSG_Country { get; set; }

        [JsonProperty("ssg_locationname")]
        public string Name { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_locationstatename")]
        public string NonCanadianState { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SSG_SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }
    }
}
