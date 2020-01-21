using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.Address
{
    public class SSG_Address : DynamicsEntity
    {

        [JsonProperty("ssg_addresscategorytext")]
        public int? Category { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_address")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_addresslinethree")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_countrysubdivision")]
        public int? CountrySubdivision { get; set; }

        [JsonProperty("ssg_countrysubdivisiontext")]
        public string CountrySubdivisionText { get; set; }

        [JsonProperty("ssg_locationcityname")]
        public string City { get; set; }

        [JsonProperty("ssg_LocationCountry")]
        public virtual SSG_Country Country { get; set; }

        [JsonProperty("ssg_countrytext")]
        public string CountryText { get; set; }

        [JsonProperty("ssg_locationname")]
        public string Name { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }
    }
}
