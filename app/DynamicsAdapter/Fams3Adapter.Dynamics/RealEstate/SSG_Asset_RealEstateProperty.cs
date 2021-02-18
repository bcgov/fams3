using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.RealEstate
{
    public class RealEstateEntity : DynamicsEntity
    {
        [JsonProperty("ssg_propertyid")]
        public string PID { get; set; }

        [JsonProperty("ssg_titlenumber")]
        public string TitleNumber { get; set; }

        [JsonProperty("ssg_legaldescription")]
        public string LegalDescription { get; set; }

        [JsonProperty("ssg_numberofowners")]
        public string NumberOfOwners { get; set; }

        [JsonProperty("ssg_landtitledistrict")]
        public string LandTitleDistrict { get; set; }

        [JsonProperty("ssg_propertyaddressline1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_propertyaddressline2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_propertyaddressline3")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_propertycity")]
        public string City { get; set; }

        [JsonProperty("ssg_propertyprovince")]
        public string ProvinceState { get; set; }

        [JsonProperty("ssg_propertycountry")]
        public string Country { get; set; }

        [JsonProperty("ssg_propertypostalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_couldnotlocate")]
        public bool CouldNotLocate { get; set; }
    }

    public class SSG_Asset_RealEstateProperty : RealEstateEntity
    {
        [JsonProperty("ssg_asset_realestatepropertyid")]
        public Guid PropertyId { get; set; }
    }
}
