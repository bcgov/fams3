using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Update;
using Fams3Adapter.Dynamics.Vehicle;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.Person
{
    //if we add PersonId in this class, when we do odataClient insert, it will fail with message personID cannot be null.
    //so, we have to move PersonId to SSG_Person class. When we do insert request, there is no personID (as we use SSG_Person_Upload)
    //and when we get the result, it has personId (as SSG_Person has personId.) 
    public class PersonEntity : DynamicsEntity
    {
        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_dateofbirth")]
        [DisplayName("DOB")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("ssg_dateofdeath")]
        [DisplayName("Date of Death")]
        public DateTime? DateOfDeath { get; set; }

        [JsonProperty("ssg_dateofdeathconfirmed")]
        [DisplayName("Date of Death Confirmed")]
        public bool? DateOfDeathConfirmed { get; set; }

        [JsonProperty("ssg_firstname")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_gender")]
        public string Gender { get; set; }

        [JsonProperty("ssg_genderoptionset")]
        [DisplayName("Gender")]
        public int? GenderOptionSet { get; set; }

        [JsonProperty("ssg_incarcerated")]
        public int? Incacerated { get; set; }

        [JsonProperty("ssg_lastname")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [JsonProperty("ssg_middlename")]
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_notes")]
        [DisplayName("Notes")]
        public string Notes { get; set; }


        [JsonProperty("ssg_thirdgivenname")]
        [DisplayName("Middle Name 2")]
        public string ThirdGivenName { get; set; }

        [JsonProperty("ssg_distinguishingfeatures")]
        [DisplayName("Distinguishing Features")]
        public string DistinguishingFeatures { get; set; }

        [JsonProperty("ssg_eyecolour")]
        [DisplayName("Eye Colour")]
        public string EyeColor { get; set; }

        [JsonProperty("ssg_haircolour")]
        [DisplayName("Hair Colour")]
        public string HairColor { get; set; }

        [JsonProperty("ssg_glassesflag")]
        [DisplayName("Glasses")]
        public string WearGlasses { get; set; }

        [JsonProperty("ssg_height")]
        [DisplayName("Height")]
        public string Height { get; set; }

        [JsonProperty("ssg_weight")]
        [DisplayName("Weight")]
        public string Weight { get; set; }

        [JsonProperty("ssg_complexion")]
        [DisplayName("Complexion")]
        public string Complexion { get; set; }

        [JsonProperty("ssg_duplicatedetectionhash")]
        public string DuplicateDetectHash { get; set; }

        [JsonProperty("ssg_createdbyagency")]
        [UpdateIgnore]
        public bool IsCreatedByAgency { get; set; }

        [JsonProperty("ssg_isprimary")]
        [UpdateIgnore]
        public bool IsPrimary { get; set; }
    }

    public class SSG_Person : PersonEntity
    {
        [JsonProperty("ssg_personid")]
        public Guid PersonId { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_address")]
        public SSG_Address[] SSG_Addresses { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_identifier")]
        public SSG_Identifier[] SSG_Identifiers { get; set; }

        [JsonProperty("ssg_person_ssg_asset_bankinginformation_Person")]
        public SSG_Asset_BankingInformation[] SSG_Asset_BankingInformations { get; set; }

        [JsonProperty("ssg_person_ssg_asset_other_Person")]
        public SSG_Asset_Other[] SSG_Asset_Others { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_alias_PersonId")]
        public SSG_Aliase[] SSG_Aliases { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_asset_icbcclaim_personid")]
        public SSG_Asset_ICBCClaim[] SSG_Asset_ICBCClaims { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_asset_vehicle_PersonId")]
        public SSG_Asset_Vehicle[] SSG_Asset_Vehicles { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_asset_worksafebcclaim_PersonId")]
        public SSG_Asset_WorkSafeBcClaim[] SSG_Asset_WorkSafeBcClaims { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_employment")]
        public SSG_Employment[] SSG_Employments { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_identity")]
        public SSG_Identity[] SSG_Identities { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_phonenumber")]
        public SSG_PhoneNumber[] SSG_PhoneNumbers { get; set; }

        [JsonProperty("ssg_ssg_person_ssg_safetyconcerndetail_PersonId")]
        public SSG_SafetyConcernDetail[] sSG_SafetyConcernDetails { get; set; }

        public bool IsDuplicated { get; set; }
        public override string ToString()
        {
            return PersonId.ToString();
        }
    }
}
