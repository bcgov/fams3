using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
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
        public void SSG_Identifier_should_map_to_PersonalIdentifier_correctly()
        {
            SSG_Identifier sSG_Identifier = new SSG_Identifier()
            {
                Identification = "testIdentification",
                IdentificationEffectiveDate = new DateTime(2001, 1, 1),
                IdentificationExpirationDate = new DateTime(2001, 1, 1),
                IdentifierType = IdentificationType.SocialInsuranceNumber.Value,
                InformationSource = InformationSourceType.Employer.Value

            };
            PersonalIdentifier identifier = _mapper.Map<PersonalIdentifier>(sSG_Identifier);
            Assert.AreEqual("testIdentification", identifier.SerialNumber);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2001, 1, 1)), identifier.EffectiveDate);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2001, 1, 1)), identifier.ExpirationDate);
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
                EffectiveDate = new DateTimeOffset(new DateTime(2001,1,1)),
                EndDate= new DateTimeOffset(new DateTime(2002,2,1))
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
            Assert.AreEqual("AddressLine1 AddressLine2 AddressLine3 testCity Manitoba canada p3p3p3", ssg_addr.FullText);
            Assert.AreEqual(1, ssg_addr.StatusCode);
            Assert.AreEqual(0, ssg_addr.StateCode);
            Assert.AreEqual(new DateTime(2001, 1, 1), ssg_addr.EffectiveDate);
            Assert.AreEqual(new DateTime(2002, 2, 1), ssg_addr.EndDate);
            Assert.AreEqual("Effective Date", ssg_addr.EffectiveDateLabel);
            Assert.AreEqual("End Date", ssg_addr.EndDateLabel);
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
        public void PersonalIdentifier_should_map_to_SSG_Identifier_correctly()
        {
            PersonalIdentifier identifier = new PersonalIdentifierActual()
            {
                SerialNumber = "1111111",
                ExpirationDate = new DateTimeOffset(new DateTime(2003, 3, 3)),
                EffectiveDate = new DateTimeOffset(new DateTime(2002, 2, 2)),
                Type = PersonalIdentifierType.DriverLicense,
                IssuedBy = "BC"
            };
            SSG_Identifier sSG_Identifier = _mapper.Map<SSG_Identifier>(identifier);
            Assert.AreEqual("1111111", sSG_Identifier.Identification);
            Assert.AreEqual(new DateTime(2002, 2, 2), sSG_Identifier.IdentificationEffectiveDate);
            Assert.AreEqual(new DateTime(2003, 3, 3), sSG_Identifier.IdentificationExpirationDate);
            Assert.AreEqual(IdentificationType.DriverLicense.Value, sSG_Identifier.IdentifierType);
            Assert.AreEqual(1, sSG_Identifier.StatusCode);
            Assert.AreEqual(0, sSG_Identifier.StateCode);
        }

        [Test]
        public void PersonalPhoneNumber_should_map_to_SSG_PhoneNumber_correctly()
        {
            PhoneNumber phoneNumber = new PhoneNumberActual()
            {
               
                Date = new DateTimeOffset(new DateTime(2003, 3, 3)),
                PhoneNumber1 = "6904005678",
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
        public void SSG_PhoneNumber_should_map_to_PersonalPhoneNumber_correctly()
        {
            SSG_PhoneNumber ssg_PhoneNumber = new SSG_PhoneNumber()
            {
                
                DateData = new DateTime(2001, 1, 1),
                DateType = "Effective Date",
                TelePhoneNumber  = "6504005678",
                TelephoneNumberType = TelephoneNumberType.Cell.Value,
                InformationSource = InformationSourceType.ICBC.Value

            };
            PhoneNumber phoneNumber = _mapper.Map<PhoneNumber>(ssg_PhoneNumber);
            Assert.AreEqual("Effective Date", phoneNumber.DateType);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2001, 1, 1)), phoneNumber.Date);
            Assert.AreEqual(InformationSourceType.ICBC.Name, phoneNumber.SuppliedBy);
            Assert.AreEqual(TelephoneNumberType.Cell.Name, phoneNumber.PhoneNumberType);
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
                EffectiveDate = new DateTimeOffset(new DateTime(2001, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2002, 2, 1)),
                Description = "test name"
            };
            SSG_Alias ssg_name = _mapper.Map<SSG_Alias>(name);
            Assert.AreEqual("FirstName", ssg_name.FirstName);
            Assert.AreEqual("LastName", ssg_name.LastName);
            Assert.AreEqual("MiddleName", ssg_name.MiddleName);
            Assert.AreEqual(PersonNameCategory.LegalName.Value, ssg_name.Type);
            Assert.AreEqual("test name", ssg_name.Comments);
            Assert.AreEqual(1, ssg_name.StatusCode);
            Assert.AreEqual(0, ssg_name.StateCode);
            Assert.AreEqual(new DateTime(2001, 1, 1), ssg_name.EffectiveDate);
            Assert.AreEqual(new DateTime(2002, 2, 1), ssg_name.EndDate);
            Assert.AreEqual("Effective Date", ssg_name.EffectiveDateLabel);
            Assert.AreEqual("End Date", ssg_name.EndDateLabel);
        }
    }
}

