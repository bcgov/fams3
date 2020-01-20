using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.SearchRequest.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Mapping
{
    public class MappingProfileTest
    {

        private IMapper _mapper;

        [SetUp]
        public void Init()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = config.CreateMapper();
        }

        [Test]
        public void SSG_Identifier_should_map_to_PersonalIdentifierRequest_correctly()
        {
            SSG_Identifier sSG_Identifier = new SSG_Identifier()
            {
                Identification = "testIdentification",
                Date1 = new DateTime(2001, 1, 1),
                Date1Label = "effectiveDate",
                Date2 = new DateTime(2001, 1, 1),
                Date2Label = "expiredDate",
                IdentifierType = IdentificationType.SocialInsuranceNumber.Value,
                InformationSource = InformationSourceType.ICBC.Value,
                IssuedBy = "BC",
                SupplierTypeCode = "TypeCode",
                Description = "description",
                Notes = "note"
            };
            PersonalIdentifierRequest identifier = _mapper.Map<PersonalIdentifierRequest>(sSG_Identifier);
            Assert.AreEqual("testIdentification", identifier.Value);
            Assert.AreEqual("note", identifier.Notes);
            Assert.AreEqual("description", identifier.Description);
            Assert.AreEqual("BC", identifier.IssuedBy);
            Assert.AreEqual("TypeCode", identifier.TypeCode);
            Assert.AreEqual(2, identifier.ReferenceDates.Count());
            Assert.AreEqual(PersonalIdentifierType.SocialInsuranceNumber, identifier.Type);
           
        }

        [Test]
        public void SSG_SearchApiRequest_should_map_to_PersonSearchRequest_correctly()
        {
            SSG_SearchApiRequest sSG_SearchApiRequest = new SSG_SearchApiRequest()
            {
                PersonGivenName = "firstName",
                PersonSurname = "lastName",
                PersonBirthDate = new DateTime(2002, 2, 2),
                Identifiers = new SSG_Identifier[]
                {
                    new SSG_Identifier(){ },
                    new SSG_Identifier(){ }
                }
            };
            PersonSearchRequest personSearchRequest = _mapper.Map<PersonSearchRequest>(sSG_SearchApiRequest);
            Assert.AreEqual("firstName", personSearchRequest.FirstName);
            Assert.AreEqual("lastName", personSearchRequest.LastName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2002, 2, 2)), personSearchRequest.DateOfBirth);
            Assert.AreEqual(2, personSearchRequest.Identifiers.Count);
        }

        [Test]
        public void Address_should_map_to_SSG_Address_correctly()
        {
            var address = new AddressActual()
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                AddressLine3 = "AddressLine3",
                StateProvince = "Manitoba",
                City = "testCity",
                Type = "residence",
                CountryRegion = "canada",
                ZipPostalCode = "p3p3p3",
                SuppliedBy = "Employer",
                EffectiveDate =new DateTime(2001,1,1),
                EndDate= new DateTime(2002,2,1)
            };
            SSG_Address ssg_addr = _mapper.Map<SSG_Address>(address);
            Assert.AreEqual("AddressLine1", ssg_addr.AddressLine1);
            Assert.AreEqual("AddressLine2", ssg_addr.AddressLine2);
            Assert.AreEqual("AddressLine3", ssg_addr.AddressLine3);
            Assert.AreEqual(CanadianProvinceType.Manitoba.Value, ssg_addr.Province);
            Assert.AreEqual("testCity", ssg_addr.City);
            Assert.AreEqual("canada", ssg_addr.Country.Name);
            Assert.AreEqual(LocationType.Residence.Value, ssg_addr.Category);
            Assert.AreEqual("p3p3p3", ssg_addr.PostalCode);
            Assert.AreEqual((int)InformationSourceType.Employer.Value, ssg_addr.InformationSource);
            Assert.AreEqual(1, ssg_addr.StatusCode);
            Assert.AreEqual(0, ssg_addr.StateCode);

        }

        [Test]
        public void PersonSearchAccepted_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchAccepted accepted = new PersonSearchAccepted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "acceptedProfile"
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(accepted);
            Assert.AreEqual("acceptedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_ACCEPTED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search has been accepted for processing", searchEvent.Message);
        }

        [Test]
        public void PersonSearchRejected_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchRejected rejected = new PersonSearchRejected()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "rejectedProfile"
                },
                Reasons = new ValidationResult[] {
                    new ValidationResult(){
                        PropertyName="property1",
                        ErrorMessage="errMsg1"
                    },
                    new ValidationResult(){
                         PropertyName="property2",
                        ErrorMessage="errMsg2"
                    },
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(rejected);
            Assert.AreEqual("rejectedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_REJECTED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search has been rejected. Reasons: property1 : errMsg1, property2 : errMsg2", searchEvent.Message);
        }

        [Test]
        public void PersonSearchRejected_with_null_reasons_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchRejected rejected = new PersonSearchRejected()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "rejectedProfile"
                },
                Reasons = null
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(rejected);
            Assert.AreEqual("rejectedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_REJECTED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search has been rejected.", searchEvent.Message);
        }

        [Test]
        public void PersonSearchFailed_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchFailed failed = new PersonSearchFailed()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "failedProfile"
                },
                Cause = "failedCause"
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(failed);
            Assert.AreEqual("failedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_FAILED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing failed. Reason: failedCause", searchEvent.Message);
        }

        [Test]
        public void PersonSearchCompleted_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchCompleted completed = new PersonSearchCompleted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "completedProfile"
                },
                MatchedPerson = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    DateOfBirth = new DateTime(2019, 3, 5),
                    Identifiers = new PersonalIdentifierActual[]
                    {
                        new PersonalIdentifierActual(){ },
                        new PersonalIdentifierActual(){ }
                    },
                    Addresses = new AddressActual[]
                    {
                        new AddressActual(){ },
                        new AddressActual(){ }
                    },
                    PhoneNumbers = new PhoneNumberActual[]
                    {
                        new PhoneNumberActual(){ },
                        new PhoneNumberActual(){ }
                    }
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. 2 identifier(s) found.  2 addresses found. 2 phone number(s) found.", searchEvent.Message);
        }

        [Test]
        public void PersonSearchCompleted_with_null_addresses_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchCompleted completed = new PersonSearchCompleted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "completedProfile"
                },
                MatchedPerson = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    DateOfBirth = new DateTime(2019, 3, 5),
                    Identifiers = new PersonalIdentifierActual[]
                    {
                        new PersonalIdentifierActual(){ },
                        new PersonalIdentifierActual(){ }
                    },
                    Addresses = null
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. 2 identifier(s) found.  0 addresses found. 0 phone number(s) found.", searchEvent.Message);
        }

        [Test]
        public void PersonalIdentifierActual_should_map_to_SSG_Identifier_correctly()
        {
            PersonalIdentifierActual identifier = new PersonalIdentifierActual()
            {
                Value = "1111111",
                Type = PersonalIdentifierType.DriverLicense,
                IssuedBy = "BC",
                Description = "description",
                Notes = "notes",
                TypeCode="BCDL",
                ReferenceDates = new List<ReferenceDateActual>() {
                    new ReferenceDateActual(){Index=0, Key="startDate", Value=new DateTime(2012,1,1) },
                    new ReferenceDateActual(){Index=1, Key="endDate", Value=new DateTime(2014,1,1) },
                }
            };
            SSG_Identifier sSG_Identifier = _mapper.Map<SSG_Identifier>(identifier);
            Assert.AreEqual("1111111", sSG_Identifier.Identification);
            Assert.AreEqual("description", sSG_Identifier.Description);
            Assert.AreEqual("notes", sSG_Identifier.Notes);
            Assert.AreEqual("BCDL", sSG_Identifier.SupplierTypeCode);
            Assert.AreEqual("BC", sSG_Identifier.IssuedBy);
            Assert.AreEqual(new DateTime(2012, 1, 1), sSG_Identifier.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), sSG_Identifier.Date2);
            Assert.AreEqual("startDate", sSG_Identifier.Date1Label);
            Assert.AreEqual("endDate", sSG_Identifier.Date2Label);
            Assert.AreEqual(IdentificationType.DriverLicense.Value, sSG_Identifier.IdentifierType);
            Assert.AreEqual(1, sSG_Identifier.StatusCode);
            Assert.AreEqual(0, sSG_Identifier.StateCode);
        }

        [Test]
        public void PersonalPhoneNumber_should_map_to_SSG_PhoneNumber_correctly()
        {
            PhoneNumberActual phoneNumber = new PhoneNumberActual()
            {               
                Date = new DateTime(2003, 3, 3),
                PhoneNumber = "6904005678",
                DateType = "Effective Date",
                PhoneNumberType = "Home",
                SuppliedBy = "ICBC"
            };
            SSG_PhoneNumber sSG_PhoneNumber = _mapper.Map<SSG_PhoneNumber>(phoneNumber);
            Assert.AreEqual("6904005678", sSG_PhoneNumber.TelePhoneNumber);
            Assert.AreEqual(TelephoneNumberType.Home.Value, sSG_PhoneNumber.TelephoneNumberType);
            Assert.AreEqual(new DateTime(2003, 3, 3), sSG_PhoneNumber.DateData);
            Assert.AreEqual("Effective Date", sSG_PhoneNumber.DateType);
            Assert.AreEqual(InformationSourceType.ICBC.Value, sSG_PhoneNumber.InformationSource);
            Assert.AreEqual(1, sSG_PhoneNumber.StatusCode);
            Assert.AreEqual(0, sSG_PhoneNumber.StateCode);
        }

        [Test]
        public void Name_should_map_to_SSG_Name_correctly()
        {
            var name = new NameActual()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                Type = "Legal Name",
                EffectiveDate = new DateTime(2001, 1, 1),
                EndDate = new DateTime(2002, 2, 1),
                Description = "test name"
            };
            SSG_Aliase ssg_name = _mapper.Map<SSG_Aliase>(name);
            Assert.AreEqual("FirstName", ssg_name.FirstName);
            Assert.AreEqual("LastName", ssg_name.LastName);
            Assert.AreEqual("MiddleName", ssg_name.MiddleName);
            Assert.AreEqual("FirstName MiddleName LastName", ssg_name.FullName);
            Assert.AreEqual(PersonNameCategory.LegalName.Value, ssg_name.Type);
            Assert.AreEqual("test name", ssg_name.Comments);
            Assert.AreEqual(1, ssg_name.StatusCode);
            Assert.AreEqual(0, ssg_name.StateCode);
            //Assert.AreEqual(new DateTime(2001, 1, 1), ssg_name.EffectiveDate);
            //Assert.AreEqual(new DateTime(2002, 2, 1), ssg_name.EndDate);
            //Assert.AreEqual("Effective Date", ssg_name.EffectiveDateLabel);
            //Assert.AreEqual("End Date", ssg_name.EndDateLabel);
        }
    }
}

