using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SearchRequestEntity 
    {
        [JsonProperty("ssg_requestpriority")]
        public int? RequestPriority { get; set; }

        [JsonProperty("ssg_searchrequestnotestext")]
        public string Notes { get; set; }

        [JsonProperty("ssg_applicantaddress")]
        public string ApplicantAddressLine1 { get; set; }

        [JsonProperty("ssg_applicantaddresssecondaryunittext")]
        public string ApplicantAddressLine2 { get; set; }

        [JsonProperty("ssg_applicantlocationcityname")]
        public string ApplicantCity { get; set; }

        [JsonProperty("ssg_applicantlocationpostalcode")]
        public string ApplicantPostalCode { get; set; }

        [JsonProperty("ssg_applicantpersontelephonenumber")]
        public string ApplicantPhoneNumber { get; set; }

        [JsonProperty("ssg_applicantsocialinsurancenumber")]
        public string ApplicantSIN { get; set; }

        [JsonProperty("ssg_applicantpersongivenname")]
        public string ApplicantFirstName { get; set; }

        [JsonProperty("ssg_applicantpersonsurname")]
        public string ApplicantLastName { get; set; }

        [JsonProperty("ssg_driverslicensenumber")]
        public string PersonSoughtBCDL { get; set; }

        [JsonProperty("ssg_??????")]
        public string PersonSoughtBCID { get; set; }

        [JsonProperty("ssg_socialinsurancenumber")]
        public string PersonSoughtSIN { get; set; }

        [JsonProperty("ssg_personsurname")]
        public string PersonSoughtLastName { get; set; }

        [JsonProperty("ssg_persongivenname")]
        public string PersonSoughtFirstName { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        public DateTime PersonSoughtDateOfBirth { get; set; }

        [JsonProperty("ssg_inforequestedlocation")]
        public bool LocationRequested { get; set; }

        [JsonProperty("ssg_inforequestedpersonalhealthnumber")]
        public bool PHNRequested { get; set; }

        [JsonProperty("ssg_inforequestedtelephone")]
        public bool PhoneNumberRequested { get; set; }

        [JsonProperty("ssg_inforequestediastatus")]
        public bool IAStatusRequested { get; set; }

        [JsonProperty("ssg_inforequestedsocialinsurancenumber")]
        public bool SINRequested { get; set; }

        [JsonProperty("ssg_inforequestedemployment")]
        public bool EmploymentRequested { get; set; }

        [JsonProperty("ssg_inforequestedsafetyconcern")]
        public bool SafetyConcernRequested { get; set; }

        [JsonProperty("ssg_inforequesteddateofdeath")]
        public bool DateOfDeathRequested { get; set; }

        [JsonProperty("ssg_inforequesteddriverslicense")]
        public bool DriverLicenseRequested { get; set; }

        [JsonProperty("ssg_inforequestedassets")]
        public bool AssetRequested { get; set; }

        [JsonProperty("ssg_inforequestedincarcerationstatus")]
        public bool CarcerationStatusRequested { get; set; }

        [JsonProperty("contactpersonemailid")]
        public string AgentEmail { get; set; }

        [JsonProperty("ssg_contactpersontelephonenumber")]
        public string AgentPhoneNumber { get; set; }

        [JsonProperty("ssg_contactpersontelephonesuffixid")]
        public string AgentPhoneExtension { get; set; }

        [JsonProperty("ssg_agentpersonsurname")]
        public string AgentLastName { get; set; }

        [JsonProperty("ssg_agentpersongivenname")]
        public string AgentFirstName { get; set; }

        [JsonProperty("ssg_scheduledate")]
        public DateTime RequestDate { get; set; }

        [JsonProperty(" ssg_payorid")]
        public string PayerId { get; set; }

        [JsonProperty("ssg_organizationcasetrackingid")]
        public string CaseTrackingId { get; set; }

        [JsonProperty("ssg_fmeprequest_id")]
        public string OriginalRequestorReference { get; set; }

        [JsonProperty("ssg_payororreceiver")]
        public int? PersonSoughtRole { get; set; }

        [JsonProperty("ssg_personsextext")]
        public int? PersonSoughtGender { get; set; }

        [JsonProperty("ssg_personhaircolorcode")]
        public int? PersonSoughtHairColor { get; set; }

        [JsonProperty("ssg_personeyecolorcode")]
        public int? PersonSoughtEyeColor { get; set; }
    }

    public class SSG_SearchRequest : SearchRequestEntity
    {
        [JsonProperty("ssg_searchrequestid")]
        public Guid SearchRequestId { get; set; }

        [JsonProperty("ssg_name")]
        public string FileId { get; set; }

        public override string ToString()
        {
            return SearchRequestId.ToString();
        }
    }
}
