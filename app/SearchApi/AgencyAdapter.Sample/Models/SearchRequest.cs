using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgencyAdapter.Sample.Models
{

    public class SearchRequest
    {

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("@SchemaVersion")]
        public string SchemaVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestPriority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestorAgencyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestorReference { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestAction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SearchReasonCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RequestedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AgentName AgentName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AgencyOfficeLocation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AgentPhone AgentPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AgentFax AgentFax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AgentEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InformationRequested { get; set; }

        public List<string> InformationRequestedList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PersonSought PersonSought { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Applicant Applicant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InvolvedPersons InvolvedPersons { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Notes { get; set; }
    }

    public class AgentName
    {

        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FirstGivenName { get; set; }
    }

    public class AgentPhone
    {

        /// <summary>
        /// 
        /// </summary>
        public double Number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Extension { get; set; }
    }

    public class AgentFax
    {

        /// <summary>
        /// 
        /// </summary>
        public double Number { get; set; }
    }

    public class KnownAddress
    {

        /// <summary>
        /// 
        /// </summary>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EffectiveDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; }
    }

    public class EmployerContactAddress
    {

        /// <summary>
        /// 
        /// </summary>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AddressLine2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }
    }

    public class Employer
    {

        /// <summary>
        /// 
        /// </summary>
        public string EmployerName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EmployerContactAddress EmployerContactAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Phone Phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Fax { get; set; }
    }

    public class Employment
    {

        /// <summary>
        /// 
        /// </summary>
        public string Occupation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EffectiveDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Employer Employer { get; set; }
    }

    public class PersonSought
    {

        /// <summary>
        /// 
        /// </summary>
        public string RecipientOrPayor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FirstGivenName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SIN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BCDL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BCID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HairColour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EyeColour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KnownAddress KnownAddress { get; set; }

        public List<KnownAddress> KnownAddresses { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Employment Employment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Notes { get; set; }
    }

    public class Address
    {

        /// <summary>
        /// 
        /// </summary>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }
    }

    public class Phone
    {

        /// <summary>
        /// 
        /// </summary>
        public double Number { get; set; }
    }

    public class Applicant
    {

        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FirstGivenName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SIN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Address Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Phone Phone { get; set; }
    }

    public class InvolvedPersons
    {

        /// <summary>
        /// 
        /// </summary>
        public string InvolvementRole { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FirstGivenName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DateOfBirth { get; set; }
    }

}
