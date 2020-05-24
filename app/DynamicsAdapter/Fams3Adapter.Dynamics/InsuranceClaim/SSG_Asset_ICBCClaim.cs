using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.InsuranceClaim
{
    public class ICBCClaimEntity : DynamicsEntity
    {
        [JsonProperty("ssg_address")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_addresslinethree")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_locationcityname")]
        public string City { get; set; }

        public string SupplierCountryCode { get; set; }
        public string SupplierCountrySubdivisionCode { get; set; }

        [JsonProperty("ssg_CountrySubdivision")]
        public virtual SSG_CountrySubdivision CountrySubdivision { get; set; }

        [JsonProperty("ssg_LocationCountry")]
        public virtual SSG_Country Country { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_adjusterfirstname")]
        public string AdjusterFirstName { get; set; }

        [JsonProperty("ssg_adjusterlastname")]
        public string AdjusterLastName { get; set; }

        [JsonProperty("adjustersecondgivenname")]
        public string AdjusterMiddleName { get; set; }

        [JsonProperty("ssg_adjusterthirdgivenname")]
        public string AdjusterOtherName { get; set; }

        [JsonProperty("ssg_adjusterphonenumber")]
        public string AdjusterPhoneNumber { get; set; }

        [JsonProperty("ssg_adjusterphonenumberextension")]
        public string AdjusterPhoneNumberExt { get; set; }

        [JsonProperty("ssg_bcdlstatus")]
        public string BCDLStatus { get; set; }

        [JsonProperty("ssg_bcdlnumber")]
        public string BCDLNumber { get; set; }

        [JsonProperty("ssg_bcdlexpiry")]
        public string BCDLExpiryDate { get; set; }

        [JsonProperty("ssg_phnnumber")]
        public string PHNNumber { get; set; }

        [JsonProperty("ssg_claimcentre")]
        public string ClaimCenterLocationCode { get; set; }

        [JsonProperty("ssg_suppliertypecode")]
        public string ClaimType { get; set; }

        [JsonProperty("ssg_claimstatus")]
        public string ClaimStatus { get; set; }

        [JsonProperty("ssg_name")]
        public string ClaimNumber { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_description")]
        public string Description { get; set; }

        [JsonProperty("ssg_suppliedby")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }

    }

    public class SSG_Asset_ICBCClaim : ICBCClaimEntity
    {
        [JsonProperty("ssg_asset_icbcclaimid")]
        public Guid ICBCClaimId { get; set; }
    }
}
