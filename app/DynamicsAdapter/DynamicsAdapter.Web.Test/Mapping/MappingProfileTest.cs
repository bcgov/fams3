﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
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
        public void SSG_Identifier_should_map_to_Identifier_correctly()
        {
            SSG_Identifier sSG_Identifier = new SSG_Identifier() {
                Identification = "testIdentification",
                IdentificationEffectiveDate = new DateTime(2001, 1, 1),
                IdentificationExpirationDate = new DateTime(2001, 1, 1),
                IdentifierType = IdentificationType.SocialInsuranceNumber.Value,
                InformationSource = InformationSourceType.Employer.Value

            };
            Identifier identifier = _mapper.Map<Identifier>(sSG_Identifier);
            Assert.AreEqual("testIdentification", identifier.SerialNumber);
            Assert.AreEqual(new DateTime(2001, 1, 1), identifier.EffectiveDate );
            Assert.AreEqual(new DateTimeOffset(2001, 1, 1,0,0,0, new TimeSpan(-8,0,0)), identifier.ExpirationDate);
            Assert.AreEqual((int)PersonalIdentifierType.SocialInsuranceNumber, identifier.Type);
            Assert.AreEqual(InformationSourceType.Employer.Name, identifier.IssuedBy);
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
            Assert.AreEqual("firstName", personSearchRequest.FirstName );
            Assert.AreEqual("lastName", personSearchRequest.LastName );
            Assert.AreEqual(new DateTimeOffset(2002, 2, 2,0,0,0, new TimeSpan(-8, 0, 0)), personSearchRequest.DateOfBirth );
            Assert.AreEqual(2, personSearchRequest.Identifiers.Count);
        }

        [Test]
        public void Address_should_map_to_SSG_Address_correctly()
        {
            Address address = new Address()
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                Province = "Manitoba",
                City = "testCity",
                Type = "residence",
                Country="canada",
                PostalCode = "p3p3p3",
                SuppliedBy = "Employer"
            };
            SSG_Address ssg_addr = _mapper.Map<SSG_Address>(address);
            Assert.AreEqual("AddressLine1", ssg_addr.AddressLine1);
            Assert.AreEqual("AddressLine2", ssg_addr.AddressLine2);
            Assert.AreEqual(CanadianProvinceType.Manitoba.Value, ssg_addr.Province);
            Assert.AreEqual("testCity", ssg_addr.City );
            Assert.AreEqual("canada", ssg_addr.Country.Name);
            Assert.AreEqual(LocationType.Residence.Value, ssg_addr.Category);
            Assert.AreEqual("p3p3p3", ssg_addr.PostalCode);
            Assert.AreEqual((int)InformationSourceType.Employer.Value, ssg_addr.InformationSource );
            Assert.AreEqual("AddressLine1 AddressLine2 testCity Manitoba canada p3p3p3", ssg_addr.FullText);
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
                ProviderProfile = new ProviderProfile() { 
                      Name="acceptedProfile"
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(accepted);
            Assert.AreEqual("acceptedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp );
            Assert.AreEqual(Keys.EVENT_ACCEPTED, searchEvent.EventType);
            Assert.AreEqual(Keys.EVENT_NAME, searchEvent.Name);
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
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp );
            Assert.AreEqual(Keys.EVENT_REJECTED, searchEvent.EventType);
            Assert.AreEqual(Keys.EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search has been rejected. Reasons: property1 : errMsg1, property2 : errMsg2", searchEvent.Message );
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
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp );
            Assert.AreEqual(Keys.EVENT_REJECTED, searchEvent.EventType);
            Assert.AreEqual(Keys.EVENT_NAME, searchEvent.Name);
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
                Cause="failedCause"
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(failed);
            Assert.AreEqual("failedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_FAILED, searchEvent.EventType);
            Assert.AreEqual(Keys.EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing failed. Reason: failedCause", searchEvent.Message );
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
                    FirstName="firstName",
                    LastName="lastName",
                    DateOfBirth = new DateTime(2019,3,5),
                    Identifiers = new PersonalIdentifier[]
                    {
                        new PersonalIdentifier(){ },
                        new PersonalIdentifier(){ }
                    },
                    Addresses = new Address[]
                    {
                        new Address(){ },
                        new Address(){ }
                    }
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName );
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual(Keys.EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. 2 identifiers found.  2 addresses found.", searchEvent.Message);
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
                    Identifiers = new PersonalIdentifier[]
                    {
                        new PersonalIdentifier(){ },
                        new PersonalIdentifier(){ }
                    },
                    Addresses = null
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName );
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp );
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual(Keys.EVENT_NAME, searchEvent.Name );
            Assert.AreEqual("Auto search processing completed successfully. 2 identifiers found.  0 addresses found.",searchEvent.Message);
        }

        [Test]
        public void PersonalIdentifier_should_map_to_SSG_Identifier_correctly()
        {
            PersonalIdentifier identifier = new PersonalIdentifier()
            {
                SerialNumber = "1111111",
                ExpirationDate = new DateTime(2003, 3, 3),
                EffectiveDate = new DateTime(2002, 2, 2),
                Type = PersonalIdentifierType.DriverLicense,
                IssuedBy = "ICBC"
            };
            SSG_Identifier sSG_Identifier = _mapper.Map<SSG_Identifier>(identifier);
            Assert.AreEqual("1111111", sSG_Identifier.Identification);
            Assert.AreEqual(new DateTime(2002, 2, 2), sSG_Identifier.IdentificationEffectiveDate );
            Assert.AreEqual(new DateTime(2003, 3, 3), sSG_Identifier.IdentificationExpirationDate );
            Assert.AreEqual(IdentificationType.DriverLicense.Value, sSG_Identifier.IdentifierType);
            Assert.AreEqual(InformationSourceType.ICBC.Value, sSG_Identifier.InformationSource);
            Assert.AreEqual(1, sSG_Identifier.StatusCode);
            Assert.AreEqual(0, sSG_Identifier.StateCode);
        }
    }
}

