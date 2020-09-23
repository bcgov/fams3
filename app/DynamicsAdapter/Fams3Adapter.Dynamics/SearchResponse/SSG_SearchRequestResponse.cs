using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics.SearchResponse
{
    public class SSG_SearchRequestResponse
    {
        [JsonProperty("ssg_searchrequestresponseid")]
        public Guid SearchRequestResponseId { get; set; }

        [JsonProperty("ssg_searchrequestresponse_ssg_asset_bankinginformation_Response")]
        public SSG_Asset_BankingInformation[] SSG_BankInfos { get; set; }

        [JsonProperty("ssg_searchrequestresponse_ssg_asset_other_Response")]
        public SSG_Asset_Other[] SSG_Asset_Others { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_address")]
        public SSG_Address[] SSG_Addresses { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_alias")]
        public SSG_Aliase[] SSG_Aliases { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_asset_icbcclaim_responseid")]
        public SSG_Asset_ICBCClaim[] SSG_Asset_ICBCClaims { get; set; }

        //[JsonProperty("ssg_ssg_searchrequestresponse_ssg_asset_investment_ResponseId")]
        //public SSG_Asset_Investment[] SSG_Asset_Investments { get; set; }

        //[JsonProperty("ssg_ssg_searchrequestresponse_ssg_asset_pensiondisability_ResponseId")]
        //public SSG_Asset_Pension[] SSG_Asset_Pensions{ get; set; }

        //[JsonProperty("ssg_ssg_searchrequestresponse_ssg_asset_realestateproperty_ResponseId")]
        //public SSG_Asset_RealEstate[] SSG_Asset_RealEstates{ get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_asset_vehicle_ResponseId")]
        public SSG_Asset_Vehicle[] SSG_Asset_Vehicles { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_asset_worksafebcclaim_ResponseId")]
        public SSG_Asset_WorkSafeBcClaim[] SSG_Asset_WorkSafeBcClaims { get; set; }

        //[JsonProperty("ssg_ssg_searchrequestresponse_ssg_electronica")]
        //public SSG_Electronica[] SSG_Electronicas{ get; set; }

        //[JsonProperty("ssg_ssg_searchrequestresponse_ssg_email")]
        //public SSG_Email[] SSG_Emails{ get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_employment")]
        public SSG_Employment[] SSG_Employments { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_identifier")]
        public SSG_Identifier[] SSG_Identifiers { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_identity")]
        public SSG_Identity[] SSG_Identities { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_notes")]
        public SSG_Notese[] SSG_Noteses { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_person")]
        public SSG_Person[] SSG_Persons { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_phonenumber")]
        public SSG_PhoneNumber[] SSG_PhoneNumbers { get; set; }

        //[JsonProperty("ssg_ssg_searchrequestresponse_ssg_safetyconcerndetail_ResponseId")]
        //public SSG_SafetyConcernDetail[] SSG_SafetyConcernDetails { get; set; }

        [JsonProperty("ssg_ssg_searchrequestresponse_ssg_searchrequest_SearchRequestResponse")]
        public SSG_SearchRequest[] SSG_SearchRequest { get; set; }
    }
}
