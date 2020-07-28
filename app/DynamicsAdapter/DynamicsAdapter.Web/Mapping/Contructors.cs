using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Mapping
{
    public class Contructors
    {
        public static SearchRequestEntity ConstructSearchRequestEntity(SearchRequestOrdered src)
        {
            if (src?.Person?.Agency?.RequestId == null) throw new ArgumentNullException("SearchRequestOrdered.Person, Agency or RequestID are not allowed Null.");
            SearchRequestEntity entity = new SearchRequestEntity();
            #region agency part
            entity.AgentEmail = src.Person.Agency.Email;
            if( src.Person.Agency.AgentContact != null)
            {
                Phone agencyPhone = src.Person.Agency.AgentContact.FirstOrDefault<Phone>(m => String.Equals(m.Type, "Phone", StringComparison.InvariantCultureIgnoreCase));
                entity.AgentPhoneNumber = agencyPhone?.PhoneNumber;
                entity.AgentPhoneExtension = agencyPhone?.Extension;
                Phone agencyFax = src.Person.Agency.AgentContact.FirstOrDefault<Phone>(m => String.Equals(m.Type, "Fax", StringComparison.InvariantCultureIgnoreCase));
                entity.AgentFax = agencyFax?.PhoneNumber;
            }
            if (src.Person.Agency.Agent != null)
            {
                Name agentName = src.Person.Agency.Agent;
                entity.AgentFirstName = agentName.FirstName;
                entity.AgentLastName = agentName.LastName;
            }
            if (src.Person.Agency.InformationRequested != null)
            {
                foreach (string info in src.Person.Agency.InformationRequested)
                {
                    //todo: need to confirm with agency about the string
                    switch (info.ToLower())
                    {
                        case "location" : entity.LocationRequested = true; break;
                        case "employment": entity.EmploymentRequested = true; break;
                        case "asset": entity.AssetRequested = true; break;
                        case "sin": entity.SINRequested = true; break;
                        case "dl": entity.DriverLicenseRequested = true; break;
                        case "phn": entity.PHNRequested = true; break;
                        case "phonenumber": entity.PhoneNumberRequested = true; break;
                        case "carceration": entity.CarcerationStatusRequested = true; break;
                        case "dateofdeath": entity.DateOfDeathRequested = true; break;
                        case "iastatus": entity.IAStatusRequested = true; break;
                        case "safety": entity.SafetyConcernRequested = true; break;
                    };
                }
            }
            if (src.Person?.Agency.RequestId != null)
            {
                if (src.Person.Agency.RequestId.Length >= 30)
                {
                    string[] ids = src.Person.Agency.RequestId.Split('-');
                    if (ids.Length == 2)
                    {
                        entity.PayerId = ids[1].Substring(0, 6);
                        entity.CaseTrackingId = ids[1].Substring(6, 6);
                        string role = ids[0].Substring(2, 1);
                        if ( string.Equals(role,"P",StringComparison.InvariantCultureIgnoreCase) )
                        {
                            entity.PersonSoughtRole = PersonSoughtType.P.Value;
                        }
                        if (string.Equals(role, "R", StringComparison.InvariantCultureIgnoreCase))
                        {
                            entity.PersonSoughtRole = PersonSoughtType.R.Value;
                        }
                    }
                }
            }
            #endregion


            if (src.Person.Addresses != null)
            {
                Address applicantAddress = src.Person.Addresses.FirstOrDefault<Address>(m => m.Owner == OwnerType.Applicant);
                entity.ApplicantAddressLine1 = applicantAddress?.AddressLine1;
                entity.ApplicantAddressLine2 = applicantAddress?.AddressLine2;
                entity.ApplicantCity = applicantAddress?.City;
                entity.ApplicantPostalCode = applicantAddress?.ZipPostalCode;
                entity.ApplicantProvince = applicantAddress?.StateProvince;
                entity.ApplicantCountry = "canada";
            }
            
            if (src.Person.Names != null)
            {
                Name applicantName = src.Person.Names.FirstOrDefault<Name>(m => m.Owner == OwnerType.Applicant);
                entity.ApplicantFirstName = applicantName.FirstName;
                entity.ApplicantLastName = applicantName.LastName;
                
            }
            if(src.Person.Phones != null)
            {
                entity.ApplicantPhoneNumber = src.Person.Phones.FirstOrDefault<Phone>(m=>m.Owner == OwnerType.Applicant)?.PhoneNumber;
            }
            if( src.Person.Identifiers != null)
            {
                entity.ApplicantSIN = src.Person.Identifiers.FirstOrDefault<PersonalIdentifier>(
                    m => m.Owner == OwnerType.Applicant && m.Type==PersonalIdentifierType.SocialInsuranceNumber)?.Value;
                entity.PersonSoughtSIN = src.Person.Identifiers.FirstOrDefault<PersonalIdentifier>(
                    m => m.Owner == OwnerType.PersonSought && m.Type == PersonalIdentifierType.SocialInsuranceNumber)?.Value;
                entity.PersonSoughtBCDL = src.Person.Identifiers.FirstOrDefault<PersonalIdentifier>(
                    m => m.Owner == OwnerType.PersonSought && m.Type == PersonalIdentifierType.BCDriverLicense)?.Value;
                entity.PersonSoughtBCID = src.Person.Identifiers.FirstOrDefault<PersonalIdentifier>(
                    m => m.Owner == OwnerType.PersonSought && m.Type == PersonalIdentifierType.BCID)?.Value;
            }

            return entity;
        }
    }
}
