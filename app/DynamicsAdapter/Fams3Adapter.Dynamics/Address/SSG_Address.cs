using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Address
{
    public class AddressEntity : DynamicsEntity
    {

        [JsonProperty("ssg_addresscategorytext")]
        [DisplayName("Location Type")]
        public int? Category { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string SupplierTypeCode { get; set; }

        [JsonProperty("ssg_address")]
        [DisplayName("Address Line 1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        [DisplayName("Address Line 2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_addresslinethree")]
        [DisplayName("Address Line 3")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_CountrySubdivision")]
        [DisplayName("Province/State")]
        public virtual SSG_CountrySubdivision CountrySubdivision { get; set; }

        [JsonProperty("ssg_countrysubdivisiontext")]
        [DisplayName("Province/State Text")]
        public string CountrySubdivisionText { get; set; }

        [JsonProperty("ssg_locationcityname")]
        [DisplayName("City")]
        public string City { get; set; }

        [JsonProperty("ssg_LocationCountry")]
        [DisplayName("Country")]
        public virtual SSG_Country Country { get; set; }

        [JsonProperty("ssg_countrytext")]
        public string CountryText { get; set; }

        [JsonProperty("ssg_locationname")]
        [DisplayName("Location Name")]
        public string Name { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_description")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_incarcerationstatus")]
        public string IncarcerationStatus { get; set; }

        [UpdateIgnore]
        [JsonProperty("ssg_createdbyagency")]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_agencyupdatedescription")]
        public string UpdateDetails { get; set; }

        [JsonProperty("ssg_couldnotlocate")]
        public bool CouldNotLocate { get; set; }
    }

    public class SSG_Address : AddressEntity
    {
        [JsonProperty("ssg_addressid")]
        public Guid AddressId { get; set; }

    }
}
