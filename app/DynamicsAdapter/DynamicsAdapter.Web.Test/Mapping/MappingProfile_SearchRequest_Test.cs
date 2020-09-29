using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DynamicsAdapter.Web.Test.Mapping
{
    [System.Runtime.InteropServices.Guid("2EF2EBF7-5CD8-4C03-A254-D6281F355654")]
    public class MappingProfile_SearchRequest_Test
    {

        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void normal_SearchRequestOrdered_should_map_to_SearchRequestEntity_correctly()
        {
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "requestId",
                SearchRequestKey = "requestKey",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2001, 1, 1),
                Person = new Person()
                {
                    Agency = new Agency()
                    {
                        Agent = new Name() { FirstName = "agentFirstName", LastName = "agentLastName", Owner = OwnerType.NotApplicable },
                        AgentContact = new List<Phone> {
                            new Phone() { PhoneNumber = "agentPhoneNumber", Extension = "agentExt", Type = "Phone" },
                            new Phone() { PhoneNumber = "agentFaxNumber", Type = "Fax" }
                        },
                        Notes = "agency notes",
                        RequestPriority = RequestPriority.Rush,
                        Code = "FMEP",
                        RequestId = "QFP-12422509096920180928083433",
                        ReasonCode = SearchReasonCode.EnfPayAgr,
                        RequestDate = new DateTimeOffset(2018, 9, 28, 0, 0, 0, new TimeSpan(1, 0, 0)),
                        Email = "agencyemail@test.com",
                        InformationRequested = new List<string> { "location", "phn", "DL" },
                        LocationAddress = "NORTHERN AND INTERIOR CLIENT OFFICE, KAMLOOPS, BC"
                    },
                    FirstName = "personSoughtFirstName",
                    LastName = "personSoughtLastName",
                    Gender = "M",
                    DateOfBirth = new DateTimeOffset(1995, 1, 1, 0, 0, 0, new TimeSpan(1, 0, 0)),
                    Identifiers = new List<PersonalIdentifier>()
                    {
                        new PersonalIdentifier(){Value="123456", TypeCode="SIN", Type=PersonalIdentifierType.SocialInsuranceNumber,Owner=OwnerType.PersonSought},
                        new PersonalIdentifier(){Value="113456", TypeCode="BCID", Type=PersonalIdentifierType.BCID,Owner=OwnerType.PersonSought},
                        new PersonalIdentifier(){Value="12222456", TypeCode="BCDL", Type=PersonalIdentifierType.BCDriverLicense,Owner=OwnerType.PersonSought},
                        new PersonalIdentifier(){Value="33333456", TypeCode="SIN", Type=PersonalIdentifierType.SocialInsuranceNumber,Owner=OwnerType.Applicant},
                        new PersonalIdentifier(){Value="4444456", TypeCode="BCDL", Type=PersonalIdentifierType.BCDriverLicense,Owner=OwnerType.InvolvedPerson}
                    },
                    HairColour = "hairColor",
                    EyeColour = "eyeColor",
                    Names = new List<Name>()
                    {
                        new Name(){FirstName="applicantFirstName", LastName="applicantLastName", Owner=OwnerType.Applicant}
                    },
                    Addresses = new List<Address>()
                    {
                        new Address(){
                            AddressLine1="applicantAddressLine1",
                            AddressLine2="applicantAddressLine2",
                            City="applicantCity",
                            StateProvince="applicantProvince",
                            ZipPostalCode="applicantPostalCode",
                            Owner = OwnerType.Applicant
                        },
                        new Address(){
                            AddressLine1="involvedPersonAddressLine1",
                            AddressLine2="involvedPersonAddressLine2",
                            City="involvedPersonCity",
                            StateProvince="involvedPersonProvince",
                            ZipPostalCode="involvedPersonPostalCode",
                            Owner = OwnerType.InvolvedPerson
                        },
                    },
                    Phones = new List<Phone>()
                    {
                        new Phone()
                        {
                            PhoneNumber="11111111",
                            Owner =  OwnerType.Applicant
                        }
                    }

                },
            };
            SearchRequestEntity entity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            Assert.AreEqual("agentFirstName", entity.AgentFirstName);
            Assert.AreEqual("agentLastName", entity.AgentLastName);
            Assert.AreEqual("agentPhoneNumber", entity.AgentPhoneNumber);
            Assert.AreEqual("agentExt", entity.AgentPhoneExtension);
            Assert.AreEqual("NORTHERN AND INTERIOR CLIENT OFFICE, KAMLOOPS, BC", entity.AgencyOfficeLocationText);
            Assert.AreEqual("agentFaxNumber", entity.AgentFax);
            Assert.AreEqual("agency notes", entity.Notes);
            Assert.AreEqual("QFP-12422509096920180928083433", entity.OriginalRequestorReference);
            Assert.AreEqual("124225", entity.PayerId);
            Assert.AreEqual("090969", entity.CaseTrackingId);
            Assert.AreEqual(new DateTime(2018, 9, 28), entity.RequestDate);
            Assert.AreEqual("agencyemail@test.com", entity.AgentEmail);
            Assert.AreEqual(true, entity.LocationRequested);
            Assert.AreEqual(true, entity.PhoneNumberRequested);
            Assert.AreEqual(true, entity.PHNRequested);
            Assert.AreEqual(true, entity.DriverLicenseRequested);
            Assert.AreEqual("FMEP", entity.AgencyCode);
            Assert.AreEqual("EnfPayAgr", entity.SearchReasonCode);
            Assert.AreEqual(RequestPriorityType.Rush.Value, entity.RequestPriority);
            Assert.AreEqual(PersonSoughtType.P.Value, entity.PersonSoughtRole);
            Assert.AreEqual("personSoughtFirstName", entity.PersonSoughtFirstName);
            Assert.AreEqual("personSoughtLastName", entity.PersonSoughtLastName);
            Assert.AreEqual(GenderType.Male.Value, entity.PersonSoughtGender);
            Assert.AreEqual(new DateTime(1995, 1, 1), entity.PersonSoughtDateOfBirth);
            Assert.AreEqual("12222456", entity.PersonSoughtBCDL);
            Assert.AreEqual("113456", entity.PersonSoughtBCID);
            Assert.AreEqual("eyeColor", entity.PersonSoughtEyeColor);
            Assert.AreEqual("hairColor", entity.PersonSoughtHairColor);
            Assert.AreEqual("123456", entity.PersonSoughtSIN);

            Assert.AreEqual("applicantAddressLine1", entity.ApplicantAddressLine1);
            Assert.AreEqual("applicantAddressLine2", entity.ApplicantAddressLine2);
            Assert.AreEqual("applicantCity", entity.ApplicantCity);
            Assert.AreEqual("canada", entity.ApplicantCountry);
            Assert.AreEqual("applicantFirstName", entity.ApplicantFirstName);
            Assert.AreEqual("applicantLastName", entity.ApplicantLastName);
            Assert.AreEqual("11111111", entity.ApplicantPhoneNumber);
            Assert.AreEqual("applicantPostalCode", entity.ApplicantPostalCode);
            Assert.AreEqual("applicantProvince", entity.ApplicantProvince);
            Assert.AreEqual("33333456", entity.ApplicantSIN);
        }

        [Test]
        public void Agency_null_SearchRequestOrdered_should_throw_null_exception()
        {
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "requestId",
                SearchRequestKey = "requestKey",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                Person = new Person()
                {
                    Agency = null
                },
            };
            Assert.Throws<ArgumentNullException>(() => _mapper.Map<SearchRequestEntity>(searchRequestOrdered));
        }

        [Test]
        public void Person_null_SearchRequestOrdered_should_return_throw_null_exception()
        {
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "requestId",
                SearchRequestKey = "requestKey",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                Person = null,
            };
            Assert.Throws<ArgumentNullException>(() => _mapper.Map<SearchRequestEntity>(searchRequestOrdered));
        }

        [Test]
        public void Agent_phone_null_SearchRequestOrdered_should_map_normally()
        {
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "requestId",
                SearchRequestKey = "requestKey",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    Agency = new Agency()
                    {
                        Agent = new Name() { },
                        AgentContact = new List<Phone>
                        {
                        },
                        Code = "FMEP",
                        RequestId = "QFP-12422509096920180928083433",

                    },
                },
            };
            SearchRequestEntity entity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            Assert.AreEqual(null, entity.AgentFirstName);
            Assert.AreEqual(null, entity.AgentLastName);
            Assert.AreEqual(null, entity.AgentPhoneNumber);
            Assert.AreEqual(null, entity.AgentPhoneExtension);
            Assert.AreEqual(null, entity.AgentFax);
            Assert.AreEqual(null, entity.Notes);
        }

        [Test]
        public void Agent_invliad_request_ID_SearchRequestOrdered_should_map_normally()
        {
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                Person = new Person()
                {
                    Agency = new Agency()
                    {
                        Agent = new Name() { },
                        AgentContact = new List<Phone>
                        {
                        },
                        Code = "FMEP",
                        RequestId = "12222393288",

                    },
                },
            };
            SearchRequestEntity entity = _mapper.Map<SearchRequestEntity>(searchRequestOrdered);
            Assert.AreEqual("12222393288", entity.OriginalRequestorReference);
            Assert.AreEqual(null, entity.PayerId);
            Assert.AreEqual(null, entity.CaseTrackingId);
            Assert.AreEqual(null, entity.PersonSoughtRole);
        }
    }
}
