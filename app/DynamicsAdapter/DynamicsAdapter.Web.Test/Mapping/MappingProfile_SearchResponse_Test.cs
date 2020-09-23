using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.BankInfo;
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
using Fams3Adapter.Dynamics.SearchResponse;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicsAdapter.Web.Test.Mapping
{
    [System.Runtime.InteropServices.Guid("2EF2EBF7-5CD8-4C03-A254-D6281F355654")]
    public class MappingProfile_SearchResponse_Test
    {

        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void normal_SearchResponse_should_map_to_Person_correctly()
        {
            SSG_SearchRequestResponse response = new SSG_SearchRequestResponse()
            {
                SSG_BankInfos = new List<SSG_Asset_BankingInformation>
                {
                    new SSG_Asset_BankingInformation { AccountNumber = "accountNumber" }
                }.ToArray(),
                SSG_Asset_Others = new List<SSG_Asset_Other>
                {
                    new SSG_Asset_Other{Description="other Asset"}
                }.ToArray(),
                SSG_Addresses = new List<SSG_Address>
                {
                    new SSG_Address{AddressLine1="line1"}
                }.ToArray(),
                SSG_Aliases = new List<SSG_Aliase>
                {
                    new SSG_Aliase{ FirstName="aliasFirstName"}
                }.ToArray(),
                SSG_Asset_ICBCClaims = new List<SSG_Asset_ICBCClaim>
                {
                    new SSG_Asset_ICBCClaim{ClaimNumber="claimNumber"}
                }.ToArray(),
                SSG_Asset_Vehicles = new List<SSG_Asset_Vehicle>
                {
                    new SSG_Asset_Vehicle{PlateNumber="vehiclePlatNumber"}
                }.ToArray(),
                SSG_Employments = new List<SSG_Employment>
                {
                    new SSG_Employment{ BusinessName="employment"}
                }.ToArray(),
                SSG_Identifiers = new List<SSG_Identifier>
                {
                    new SSG_Identifier{ Identification="identification"}
                }.ToArray(),
                SSG_Identities = new List<SSG_Identity>
                {
                    new SSG_Identity{ FirstName="relatedPerson"}
                }.ToArray(),
                SSG_Noteses = new List<SSG_Notese>
                {
                    new SSG_Notese{ Description="notes" }
                }.ToArray(),
                SSG_Persons = new List<SSG_Person>
                {
                    new SSG_Person{ FirstName="personFirstName" }
                }.ToArray(),
                SSG_PhoneNumbers = new List<SSG_PhoneNumber>
                {
                    new SSG_PhoneNumber{ TelePhoneNumber="phoneNumber" }
                }.ToArray(),
                SSG_SearchRequests = new List<SSG_SearchRequest>
                {
                    new SSG_SearchRequest{Agency = new SSG_Agency{AgencyCode="FMEP" } }
                }.ToArray(),
            };
            Person person = _mapper.Map<Person>(response);
            Assert.AreEqual(1, person.Names.Count);
            Assert.AreEqual(1, person.Identifiers.Count);
            Assert.AreEqual(1, person.Addresses.Count);
            Assert.AreEqual(1, person.Phones.Count);
        }

        [Test]
        public void SSG_SearchRequest_should_map_to_Person_Agency_correctly()
        {
            SSG_SearchRequestResponse response = new SSG_SearchRequestResponse()
            {
                SSG_SearchRequests = new List<SSG_SearchRequest>
                {
                    new SSG_SearchRequest{
                        Agency = new SSG_Agency{AgencyCode="FMEP" },
                        RequestDate=new DateTime(2001,1,1),
                        ResponsePersonFirstName="firstName",
                        ResponsePersonMiddleName="middleName",
                        ResponsePersonSurName="surname",
                        ResponsePersonThirdGiveName="thirdGivenName",
                        PersonSoughtDateOfBirth=new DateTime(2001,1,1),
                        PersonSoughtGender = GenderType.Male.Value,
                        AgentFirstName="agentFirstName",
                        AgentLastName="agentLastName",
                        SearchReason = new SSG_SearchRequestReason{ ReasonCode="reasonCode"},
                        OriginalRequestorReference="originalRef",
                        RequestPriority = RequestPriorityType.Rush.Value,
                        PersonSoughtRole = PersonSoughtType.P.Value,
                        DaysOpen = 100
                    }
                }.ToArray(),
                SSG_Persons = new List<SSG_Person>
                {
                    new SSG_Person{ FirstName="personFirstName" }
                }.ToArray(),
            };
            Person person = _mapper.Map<Person>(response);
            Assert.AreEqual("firstName", person.Agency.PersonSoughtInRequest_FirstName);
            Assert.AreEqual("middleName", person.Agency.PersonSoughtInRequest_MiddleName);
            Assert.AreEqual("thirdGivenName", person.Agency.PersonSoughtInRequest_SecondMiddleName);
            Assert.AreEqual("surname", person.Agency.PersonSoughtInRequest_LastName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2001, 1, 1)), person.Agency.PersonSoughtInRequest_DateOfBirth);
            Assert.AreEqual("m", person.Agency.PersonSoughtInRequest_Gender);
            Assert.AreEqual("agentFirstName", person.Agency.Agent.FirstName);
            Assert.AreEqual("agentLastName", person.Agency.Agent.LastName);
            Assert.AreEqual("reasonCode", person.Agency.ReasonCode);
            Assert.AreEqual("originalRef", person.Agency.RequestId);
            Assert.AreEqual(RequestPriority.Rush, person.Agency.RequestPriority);
            Assert.AreEqual("P", person.Type);
            Assert.AreEqual("FMEP", person.Agency.Code);
            Assert.AreEqual(100, person.Agency.DaysOpen);
        }

        [Test]
        public void SSG_Person_should_map_to_Person_correctly()
        {
            SSG_SearchRequestResponse response = new SSG_SearchRequestResponse()
            {
                SSG_SearchRequests = new List<SSG_SearchRequest>
                {
                    new SSG_SearchRequest {
                        Agency = new SSG_Agency { AgencyCode = "FMEP" },
                    }
                }.ToArray(),
                SSG_Persons = new List<SSG_Person>
                {
                    new SSG_Person{
                        FirstName="personFirstName",
                        GenderOptionSet = GenderType.Female.Value,
                        Incacerated = NullableBooleanType.Yes.Value,
                        Date1=new DateTime(2000,1,1),
                        Date1Label="test",
                        ResponseComments="responseComments"
                    }
                }.ToArray(),
            };
            Person person = _mapper.Map<Person>(response);
            Assert.AreEqual("personFirstName", person.FirstName);
            Assert.AreEqual("f", person.Gender);
            Assert.AreEqual("yes", person.Incacerated);
            Assert.AreEqual(1, person.ReferenceDates.Count);
            Assert.AreEqual("responseComments", person.ResponseComments);
        }


        [Test]
        public void SSG_Aliase_should_map_to_Person_Name_correctly()
        {
            var alias = new SSG_Aliase
            {
                FirstName = "aliasFirstName",
                Date1 = new DateTime(2000, 1, 1),
                Date1Label = "label",
                ResponseComments = "aliasComments",
                DateOfBirth = new DateTime(2004, 1, 1)
            };

            Name name = _mapper.Map<Name>(alias);
            Assert.AreEqual("aliasFirstName", name.FirstName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2004, 1, 1)), name.DateOfBirth);
        }

        [Test]
        public void SSG_Identifier_should_map_to_Person_Identifier_correctly()
        {
            var id = new SSG_Identifier
            {
                Identification = "1234455",
                IssuedBy = "ICBC",
                Date1Label = "label",
                Date1 = new DateTime(2000, 1, 1),
                ResponseComments = "identifierComments",
                IdentifierType = IdentificationType.BCDriverLicense.Value,
            };

            PersonalIdentifier personalIdentifier = _mapper.Map<PersonalIdentifier>(id);
            Assert.AreEqual("ICBC", personalIdentifier.IssuedBy);
            Assert.AreEqual("1234455", personalIdentifier.Value);
            Assert.AreEqual("identifierComments", personalIdentifier.ResponseComments);
            Assert.AreEqual(PersonalIdentifierType.BCDriverLicense, personalIdentifier.Type);
            Assert.AreEqual(1, personalIdentifier.ReferenceDates.Count);
        }

        [Test]
        public void SSG_Address_should_map_to_Person_Address_correctly()
        {
            var address = new SSG_Address
            {
                AddressLine1 = "addressline1",
                AddressLine2 = "addressline2",
                PostalCode = "postalCode",
                City = "city",
                Date1Label = "label",
                Date1 = new DateTime(2000, 1, 1),
                ResponseComments = "addressComments",
                CountryText = "canada",
                CountrySubdivisionText = "bc",
                Category = LocationType.Business.Value,
                IncarcerationStatus = "incarceration"
            };

            Address addr = _mapper.Map<Address>(address);
            Assert.AreEqual("addressline1", addr.AddressLine1);
            Assert.AreEqual("addressComments", addr.ResponseComments);
            Assert.AreEqual("city", addr.City);
            Assert.AreEqual("canada", addr.CountryRegion);
            Assert.AreEqual("bc", addr.StateProvince);
            Assert.AreEqual("business", addr.Type);
            Assert.AreEqual("incarceration", addr.IncarcerationStatus);
            Assert.AreEqual(1, addr.ReferenceDates.Count);
        }

        [Test]
        public void SSG_PhoneNumber_should_map_to_Person_Phone_correctly()
        {
            SSG_PhoneNumber phoneNumber = new SSG_PhoneNumber
            {
                TelePhoneNumber = "123456",
                PhoneExtension = "123",
                TelephoneNumberType = TelephoneNumberType.Home.Value,
                ResponseComments = "responseComments"
            };

            Phone phone = _mapper.Map<Phone>(phoneNumber);

            Assert.AreEqual("123456", phone.PhoneNumber);
            Assert.AreEqual("123", phone.Extension);
            Assert.AreEqual("home", phone.Type);
            Assert.AreEqual("responseComments", phone.ResponseComments);
        }

        [Test]
        public void SSG_Employment_should_map_to_Employment_correctly()
        {
            SSG_Employment employment = new SSG_Employment
            {
                EmploymentType = EmploymentRecordType.IncomeAssistance.Value,
                BusinessName = "legalBizName",
                DBAName = "dbaName",
                PrimaryPhoneNumber = "primaryPhone",
                PrimaryPhoneExtension = "primaryExt",
                PrimaryFax = "primaryFax",
                Website = "www.website.com",
                AddressLine1 = "employmentAddress1",
                AddressLine2 = "employmentAddress2",
                City = "employmentCity",
                PostalCode = "postalCode",
                CountryText = "country",
                CountrySubdivisionText = "province",
                EmploymentStatus = EmploymentStatusType.NotEmployed.Value,
                SelfEmployComRegistrationNo = "selfComNo",
                SelfEmployComType = SelfEmploymentCompanyType.ExtraprovincialNonShareCorp.Value,
                Occupation = "occupation",
                SelfEmployComRole = SelfEmploymentCompanyRoleType.Officer.Value,
                SelfEmployPercentOfShare = 50,
                IncomeAssistanceStatusOption = IncomeAssistanceStatusType.Closed.Value,
                IncomeAssistanceDesc = "income assistance description",
                ContactPerson = "contact person",
                PrimaryContactPhone = "primaryContactPhoneNumber",
                PrimaryContactPhoneExt = "primaryContactExt",
                SSG_EmploymentContacts = new List<SSG_EmploymentContact>
                {
                    new SSG_EmploymentContact
                    {
                        ContactName="contactName",
                        Description="description",
                        PhoneNumber="phone",
                        PhoneExtension="ext",
                        FaxNumber="faxNumber",
                        PhoneType=TelephoneNumberType.Cell.Value
                    }
                }.ToArray()
            };

            Employment e = _mapper.Map<Employment>(employment);

            Assert.AreEqual(true, e.IncomeAssistance);
            Assert.AreEqual("legalBizName", e.Employer.Name);
            Assert.AreEqual("dbaName", e.Employer.DbaName);
            Assert.AreEqual("employmentAddress1", e.Employer.Address.AddressLine1);
            Assert.AreEqual("Not Employed", e.EmploymentStatus);
            Assert.AreEqual("Extraprovincial Non-Share Corporation", e.SelfEmployComType);
            Assert.AreEqual(5, e.Employer.Phones.Count);

        }

        //[Test]
        //public void Agent_invliad_request_ID_SearchRequestOrdered_should_map_normally()
        //{
        //    SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
        //    {
        //        Action = RequestAction.NEW,
        //        Person = new Person()
        //        {
        //            Agency = new Agency()
        //            {
        //                Agent = new Name() { },
        //                AgentContact = new List<Phone>
        //                {
        //                },
        //                Code = "FMEP",
        //                RequestId = "12222393288",

        //            },
        //        },
        //    };
        //    SearchRequestEntity entity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
        //    Assert.AreEqual("12222393288", entity.OriginalRequestorReference);
        //    Assert.AreEqual(null, entity.PayerId);
        //    Assert.AreEqual(null, entity.CaseTrackingId);
        //    Assert.AreEqual(null, entity.PersonSoughtRole);
        //}
    }
}
