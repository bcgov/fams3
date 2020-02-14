using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.Employment
{
    public class SSG_Employment :  DynamicsEntity
    {

        [JsonProperty("ssg_website")]
        public string Website { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_employmentcontactname")]
        public string ContactPerson { get; set; }

        [JsonProperty("ssg_employmentconfirmed")]
        public bool? EmploymentConfirmed { get; set; }

        [JsonProperty("ssg_incomeassistance")]
        public bool? IncomeAssistance { get; set; }

        [JsonProperty("ssg_incomeassistancestatus")]
        public string IncomeAssistanceStatus { get; set; }

        [JsonProperty("ssg_occupation")]
        public string Occupation { get; set; }

        [JsonProperty("ssg_occupationdescription")]
        public string OccupationDescription { get; set; }

        [JsonProperty("ssg_employmentowner")]
        public string BusinessOwner { get; set; }

        [JsonProperty("ssg_employmentname")]
        public string BusinessName { get; set; }

        [JsonProperty("ssg_address")]
        public string AddressLine1 { get; set; }

        [JsonProperty("ssg_addresssecondaryunittext")]
        public string AddressLine2 { get; set; }

        [JsonProperty("ssg_addresslinethree")]
        public string AddressLine3 { get; set; }

        [JsonProperty("ssg_CountrySubdivision")]
        public virtual SSG_CountrySubdivision CountrySubdivision { get; set; }

        [JsonProperty("ssg_countrysubdivisiontext")]
        public string CountrySubdivisionText { get; set; }

        [JsonProperty("ssg_locationcityname")]
        public string City { get; set; }

        [JsonProperty("ssg_PersonId")]
        public virtual SSG_Person Person { get; set; }


        [JsonProperty("ssg_LocationCountry")]
        public virtual SSG_Country Country { get; set; }

        [JsonProperty("ssg_countrytext")]
        public string CountryText { get; set; }

        [JsonProperty("ssg_locationpostalcode")]
        public string PostalCode { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }
    }
}
