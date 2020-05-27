using Fams3Adapter.Dynamics.SearchRequest;
using Newtonsoft.Json;
using System;

namespace Fams3Adapter.Dynamics.Person
{
    //if we add PersonId in this class, when we do odataClient insert, it will fail with message personID cannot be null.
    //so, we have to move PersonId to SSG_Person class. When we do insert request, there is no personID (as we use SSG_Person_Upload)
    //and when we get the result, it has personId (as SSG_Person has personId.) 
    public class PersonEntity
    {
        [JsonProperty("statecode")]
        public int StateCode { get; set; }

        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_SearchRequest")]
        public virtual SSG_SearchRequest SearchRequest { get; set; }

        [JsonProperty("ssg_informationsourcetext")]
        public int? InformationSource { get; set; }

        [JsonProperty("ssg_dateofbirth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("ssg_dateofdeath")]
        public DateTime? DateOfDeath { get; set; }

        [JsonProperty("ssg_dateofdeathconfirmed")]
        public bool? DateOfDeathConfirmed { get; set; }

        [JsonProperty("ssg_firstname")]
        public string FirstName { get; set; }

        [JsonProperty("ssg_gender")]
        public string Gender { get; set; }

        [JsonProperty("ssg_incarcerated")]
        public int? Incacerated { get; set; }

        [JsonProperty("ssg_lastname")]
        public string LastName { get; set; }

        [JsonProperty("ssg_middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("ssg_notes")]
        public string Notes { get; set; }

        [JsonProperty("ssg_thirdgivenname")]
        public string ThirdGivenName { get; set; }

        [JsonProperty("ssg_distinguishingfeatures")]
        public string DistinguishingFeatures { get; set; }

        [JsonProperty("ssg_eyecolour")]
        public string EyeColor { get; set; }

        [JsonProperty("ssg_haircolour")]
        public string HairColor { get; set; }

        [JsonProperty("ssg_glassesflag")]
        public string WearGlasses { get; set; }

        [JsonProperty("ssg_height")]
        public string Height { get; set; }

        [JsonProperty("ssg_weight")]
        public string Weight { get; set; }

        [JsonProperty("ssg_complexion")]
        public string Complexion { get; set; }
    }

    public class SSG_Person : PersonEntity
    {
        [JsonProperty("ssg_personid")]
        public Guid PersonId { get; set; }
    }
}
