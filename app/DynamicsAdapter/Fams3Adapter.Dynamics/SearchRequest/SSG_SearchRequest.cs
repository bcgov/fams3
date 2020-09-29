using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SearchRequestEntity
    {
        [JsonProperty("ssg_requestpriority")]
        public int? RequestPriority { get; set; }

        [JsonProperty("ssg_searchrequestnotestext")]
        [UpdateIgnore]
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
        [CompareOnlyNumber]
        public string ApplicantPhoneNumber { get; set; }

        [JsonProperty("ssg_applicantsocialinsurancenumber")]
        public string ApplicantSIN { get; set; }

        [JsonProperty("ssg_applicantpersongivenname")]
        public string ApplicantFirstName { get; set; }

        [JsonProperty("ssg_applicantpersonsurname")]
        public string ApplicantLastName { get; set; }

        [JsonProperty("ssg_applicantcountrysubdivisiontext")]
        public string ApplicantProvince { get; set; }

        [JsonProperty("ssg_locationcountrytext")]
        public string ApplicantCountry { get; set; }

        [JsonProperty("ssg_driverslicensenumber")]
        public string PersonSoughtBCDL { get; set; }

        [JsonProperty("ssg_personalhealthnumber")] //todo: need confirm from FSO.
        public string PersonSoughtBCID { get; set; }

        [JsonProperty("ssg_socialinsurancenumber")]
        public string PersonSoughtSIN { get; set; }

        [JsonProperty("ssg_personsurname")]
        public string PersonSoughtLastName { get; set; }

        [JsonProperty("ssg_persongivenname")]
        public string PersonSoughtFirstName { get; set; }

        [JsonProperty("ssg_personbirthdate")]
        public DateTime? PersonSoughtDateOfBirth { get; set; }

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

        [JsonProperty("ssg_contactpersonemailid")]
        public string AgentEmail { get; set; }

        [JsonProperty("ssg_contactpersontelephonenumber")]
        [CompareOnlyNumber]
        public string AgentPhoneNumber { get; set; }

        [JsonProperty("ssg_contactpersontelephonesuffixid")]
        public string AgentPhoneExtension { get; set; }

        [JsonProperty("ssg_agentfax")]
        [CompareOnlyNumber]
        public string AgentFax { get; set; }

        [JsonProperty("ssg_agentpersonsurname")]
        public string AgentLastName { get; set; }

        [JsonProperty("ssg_agentpersongivenname")]
        public string AgentFirstName { get; set; }

        [JsonProperty("ssg_scheduledate")]
        public DateTime RequestDate { get; set; }

        [JsonProperty("ssg_payorid")]
        public string PayerId { get; set; }

        [JsonProperty("ssg_organizationcasetrackingid")]
        public string CaseTrackingId { get; set; }

        [JsonProperty("ssg_fmeprequest_id")]
        public string OriginalRequestorReference { get; set; }

        [JsonProperty("ssg_payororreceiver")]
        public int? PersonSoughtRole { get; set; }

        [JsonProperty("ssg_personsextext")]
        public int? PersonSoughtGender { get; set; }

        [JsonProperty("ssg_personsoughthaircolor")]
        public string PersonSoughtHairColor { get; set; }

        [JsonProperty("ssg_personsoughteyecolor")]
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
        public virtual SSG_Agency Agency { get; set; }

        public string SearchReasonCode { get; set; } //used to link ssg_searchrequestreason

        [JsonProperty("ssg_RequestCategoryText")]
        public virtual SSG_SearchRequestReason SearchReason { get; set; }

        public string AgencyOfficeLocationText { get; set; } //used to link ssg_AgencyLocation

        [JsonProperty("ssg_AgencyLocation")]
        public virtual SSG_AgencyLocation AgencyLocation { get; set; }

        [JsonProperty("ssg_personmiddlename")]
        public string PersonSoughtMiddleName { get; set; }

        [JsonProperty("ssg_personthirdgivenname")]
        public string PersonSoughtThirdGiveName { get; set; }

        [JsonProperty("ssg_durationrequestwasopen")]
        public int DaysOpen { get; set; }
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

        [UpdateIgnore]
        public bool Updated { get; set; }

        public bool IsDuplicated { get; set; }

        public override string ToString()
        {
            return SearchRequestId.ToString();
        }
    }
}
