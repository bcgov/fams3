using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SearchRequestEntity
    {
        [JsonProperty("ssg_requestpriority")]
        [DisplayName("Priority")]
        public int? RequestPriority { get; set; }

        [JsonProperty("ssg_searchrequestnotestext")]
        [UpdateIgnore]
        public string Notes { get; set; }

        [JsonProperty("ssg_applicantaddress")]
        [DisplayName("Applicant Address Line 1")]
        public string ApplicantAddressLine1 { get; set; }

        [JsonProperty("ssg_applicantaddresssecondaryunittext")]
        [DisplayName("Applicant Address Line 2")]
        public string ApplicantAddressLine2 { get; set; }

        [JsonProperty("ssg_applicantaddressline3")]
        [DisplayName("Applicant Address Line 3")]
        public string ApplicantAddressLine3 { get; set; }

        [JsonProperty("ssg_applicantlocationcityname")]
        [DisplayName("Applicant City")]
        public string ApplicantCity { get; set; }

        [JsonProperty("ssg_applicantlocationpostalcode")]
        [DisplayName("Applicant Postal Code")]
        public string ApplicantPostalCode { get; set; }

        [JsonProperty("ssg_applicantpersontelephonenumber")]
        [CompareOnlyNumber]
        [DisplayName("Applicant Phone Number")]
        public string ApplicantPhoneNumber { get; set; }

        [JsonProperty("ssg_applicantsocialinsurancenumber")]
        [DisplayName("Applicant Social Insurance Number")]
        public string ApplicantSIN { get; set; }

        [JsonProperty("ssg_applicantpersongivenname")]
        [DisplayName("Applicant First Name")]
        public string ApplicantFirstName { get; set; }

        [JsonProperty("ssg_applicantpersonsurname")]
        [DisplayName("Applicant Last Name")]
        public string ApplicantLastName { get; set; }

        [JsonProperty("ssg_applicantpersonmiddlename")]
        [DisplayName("Applicant Middle Name")]
        public string ApplicantMiddleName { get; set; }

        [JsonProperty("ssg_applicantmiddlename2")]
        [DisplayName("Applicant Middle Name 2")]
        public string ApplicantOtherName { get; set; }

        [JsonProperty("ssg_applicantcountrysubdivisiontext")]
        [DisplayName("Applicant Province/State")]
        public string ApplicantProvince { get; set; }

        [JsonProperty("ssg_locationcountrytext")]
        [DisplayName("Applicant Country")]
        public string ApplicantCountry { get; set; }

        [JsonProperty("ssg_driverslicensenumber")]
        [DisplayName("Driver's License Number")]
        public string PersonSoughtBCDL { get; set; }

        [JsonProperty("ssg_personalhealthnumber")] //todo: need confirm from FSO.
        [DisplayName("Personal Health Number")]
        public string PersonSoughtBCID { get; set; }

        [JsonProperty("ssg_socialinsurancenumber")]
        [DisplayName("Social Insurance number")]
        public string PersonSoughtSIN { get; set; }

        [JsonProperty("ssg_personsurname")]
        [DisplayName("Last Name")]
        public string PersonSoughtLastName { get; set; }

        [JsonProperty("ssg_persongivenname")]
        [DisplayName("First Name")]
        public string PersonSoughtFirstName { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        [DisplayName("DOB")]
        public DateTime? PersonSoughtDateOfBirth { get; set; }

        [JsonProperty("ssg_inforequestedlocation")]
        [DisplayName("Info Requested Location")]
        public bool LocationRequested { get; set; }

        [JsonProperty("ssg_inforequestedpersonalhealthnumber")]
        [DisplayName("Info Requested Personal Health Number")]
        public bool PHNRequested { get; set; }

        [JsonProperty("ssg_inforequestedtelephone")]
        [DisplayName("Info Requested Telephone")]
        public bool PhoneNumberRequested { get; set; }

        [JsonProperty("ssg_inforequestediastatus")]
        [DisplayName("Info Requested IA Status")]
        public bool IAStatusRequested { get; set; }

        [JsonProperty("ssg_inforequestedsocialinsurancenumber")]
        [DisplayName("Info Requested Social Insurance Number")]
        public bool SINRequested { get; set; }

        [JsonProperty("ssg_inforequestedemployment")]
        [DisplayName("Info Requested Employment")]
        public bool EmploymentRequested { get; set; }

        [JsonProperty("ssg_inforequestedsafetyconcern")]
        [DisplayName("Info Requested Safety Concern")]
        public bool SafetyConcernRequested { get; set; }

        [JsonProperty("ssg_inforequesteddateofdeath")]
        [DisplayName("Info Requested Date of Death")]
        public bool DateOfDeathRequested { get; set; }

        [JsonProperty("ssg_inforequesteddriverslicense")]
        [DisplayName("Info Requested Driver's License")]
        public bool DriverLicenseRequested { get; set; }

        [JsonProperty("ssg_inforequestedassets")]
        [DisplayName("Info Requested Assets")]
        public bool AssetRequested { get; set; }

        [JsonProperty("ssg_inforequestedincarcerationstatus")]
        [DisplayName("Info Requested Incarceration Status")]
        public bool CarcerationStatusRequested { get; set; }

        [JsonProperty("ssg_contactpersonemailid")]
        [DisplayName("Agent Email Address")]
        public string AgentEmail { get; set; }

        [JsonProperty("ssg_contactpersontelephonenumber")]
        [CompareOnlyNumber]
        [DisplayName("Agent Phone Number")]
        public string AgentPhoneNumber { get; set; }

        [JsonProperty("ssg_contactpersontelephonesuffixid")]
        [DisplayName("Agent Phone Extension")]
        public string AgentPhoneExtension { get; set; }

        [JsonProperty("ssg_agentfax")]
        [CompareOnlyNumber]
        [DisplayName("Agent Phone Extension")]
        public string AgentFax { get; set; }

        [JsonProperty("ssg_agentpersonsurname")]
        [DisplayName("Agent Last Name")]
        public string AgentLastName { get; set; }

        [JsonProperty("ssg_agentpersongivenname")]
        [DisplayName("Agent First Name")]
        public string AgentFirstName { get; set; }

        [JsonProperty("ssg_scheduledate")]
        [DisplayName("Requested Date")]
        public DateTime RequestDate { get; set; }

        [JsonProperty("ssg_payorid")]
        [DisplayName("Payor/Recipient ID")]
        public string PayerId { get; set; }

        [JsonProperty("ssg_organizationcasetrackingid")]
        [DisplayName("Agency Ref")]
        public string CaseTrackingId { get; set; }

        [JsonProperty("ssg_fmeprequest_id")]
        [DisplayName("FMEP REQUEST_ID")]
        public string OriginalRequestorReference { get; set; }

        [JsonProperty("ssg_payororreceiver")]
        [DisplayName("Payor or Recipient")]
        public int? PersonSoughtRole { get; set; }

        [JsonProperty("ssg_personsextext")]
        [DisplayName("Gender")]
        public int? PersonSoughtGender { get; set; }

        [JsonProperty("ssg_personsoughthaircolor")]
        [DisplayName("Hair Colour")]
        public string PersonSoughtHairColor { get; set; }

        [JsonProperty("ssg_personsoughteyecolor")]
        [DisplayName("Eye Colour")]
        public string PersonSoughtEyeColor { get; set; }

        [JsonProperty("ssg_createdbyopenshift")]
        [UpdateIgnore]
        public bool CreatedByApi { get; set; }

        [JsonProperty("ssg_updatedbyagency")]
        [UpdateIgnore]
        public bool UpdatedByApi { get; set; }

        [JsonProperty("ssg_notifydynadaptoroncreation")]
        [UpdateIgnore]
        public bool SendNotificationOnCreation { get; set; }

        public string AgencyCode { get; set; } //used to link ssg_agency entity

        [JsonProperty("ssg_Agency")]
        [DisplayName("Agency Code")]
        public virtual SSG_Agency Agency { get; set; }

        [DisplayName("Reason")]
        public string SearchReasonCode { get; set; } //used to link ssg_searchrequestreason

        [JsonProperty("ssg_RequestCategoryText")]
        public virtual SSG_SearchRequestReason SearchReason { get; set; }

        [DisplayName("Agency Location")]
        public string AgencyOfficeLocationText { get; set; } //used to link ssg_AgencyLocation

        [JsonProperty("ssg_AgencyLocation")]
        public virtual SSG_AgencyLocation AgencyLocation { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        [DisplayName("Middle Name")]
        public string PersonSoughtMiddleName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        [DisplayName("Middle Name 2")]
        public string PersonSoughtThirdGiveName { get; set; }

        [JsonProperty("ssg_durationrequestwasopen")]
        public int MinsOpen { get; set; }
    }

    public class SSG_SearchRequest : SearchRequestEntity, IUpdatableObject
    {
        [JsonProperty("ssg_searchrequestid")]
        public Guid SearchRequestId { get; set; }

        [JsonProperty("ssg_name")]
        public string FileId { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_ssg_searchrequest_ssg_person_SearchRequest")]
        public SSG_Person[] SSG_Persons { get; set; }

        [JsonProperty("ssg_ssg_searchrequest_ssg_notes_SearchRequest")]
        public SSG_Notese[] SSG_Notes { get; set; }

        public bool IsDuplicated { get; set; }

        public override string ToString()
        {
            return SearchRequestId.ToString();
        }
    }
}
