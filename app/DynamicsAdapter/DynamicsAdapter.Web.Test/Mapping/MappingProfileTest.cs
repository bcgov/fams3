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
            Assert.AreEqual(identifier.SerialNumber, "testIdentification");
            Assert.AreEqual(identifier.EffectiveDate, new DateTime(2001, 1, 1));
            Assert.AreEqual(identifier.ExpirationDate, new DateTime(2001, 1, 1));
            Assert.AreEqual(identifier.Type,(int)PersonalIdentifierType.SocialInsuranceNumber);
            Assert.AreEqual(identifier.IssuedBy,InformationSourceType.Employer.Name);
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
            Assert.AreEqual(personSearchRequest.FirstName, "firstName");
            Assert.AreEqual(personSearchRequest.LastName, "lastName");
            Assert.AreEqual(personSearchRequest.DateOfBirth, new DateTime(2002, 2, 2));
            Assert.AreEqual(personSearchRequest.Identifiers.Count, 2);
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
            Assert.AreEqual(ssg_addr.AddressLine1, "AddressLine1");
            Assert.AreEqual(ssg_addr.AddressLine2, "AddressLine2");
            Assert.AreEqual(ssg_addr.Province, CanadianProvinceType.Manitoba.Value);
            Assert.AreEqual(ssg_addr.City, "testCity");
            Assert.AreEqual(ssg_addr.Country.Name, "canada");
            Assert.AreEqual(ssg_addr.Category, LocationType.Residence.Value);
            Assert.AreEqual(ssg_addr.PostalCode, "p3p3p3");
            Assert.AreEqual(ssg_addr.InformationSource, (int)InformationSourceType.Employer.Value);
            Assert.AreEqual(ssg_addr.FullText, "AddressLine1 AddressLine2 testCity Manitoba canada p3p3p3");
            Assert.AreEqual(ssg_addr.StatusCode, 1);
            Assert.AreEqual(ssg_addr.StateCode, 0);
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
            Assert.AreEqual(searchEvent.ProviderName, "acceptedProfile");
            Assert.AreEqual(searchEvent.TimeStamp, new DateTime(2003, 3, 3));
            Assert.AreEqual(searchEvent.EventType, "Accepted");
            Assert.AreEqual(searchEvent.Name, "Person Search");
            Assert.AreEqual(searchEvent.Message, "Auto search has been accepted for processing");
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
            Assert.AreEqual(searchEvent.ProviderName, "rejectedProfile");
            Assert.AreEqual(searchEvent.TimeStamp, new DateTime(2003, 3, 3));
            Assert.AreEqual(searchEvent.EventType, "Rejected");
            Assert.AreEqual(searchEvent.Name, "Person Search");
            Assert.AreEqual(searchEvent.Message, "Auto search has been rejected. Reasons: property1 : errMsg1, property2 : errMsg2");
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
            Assert.AreEqual(searchEvent.ProviderName, "rejectedProfile");
            Assert.AreEqual(searchEvent.TimeStamp, new DateTime(2003, 3, 3));
            Assert.AreEqual(searchEvent.EventType, "Rejected");
            Assert.AreEqual(searchEvent.Name, "Person Search");
            Assert.AreEqual(searchEvent.Message, "Auto search has been rejected.");
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
            Assert.AreEqual(searchEvent.ProviderName, "failedProfile");
            Assert.AreEqual(searchEvent.TimeStamp, new DateTime(2003, 3, 3));
            Assert.AreEqual(searchEvent.EventType, "Failed");
            Assert.AreEqual(searchEvent.Name, "Person Search");
            Assert.AreEqual(searchEvent.Message, "Auto search processing failed. Reason: failedCause");
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
            Assert.AreEqual(searchEvent.ProviderName, "completedProfile");
            Assert.AreEqual(searchEvent.TimeStamp, new DateTime(2003, 3, 3));
            Assert.AreEqual(searchEvent.EventType, "Completed");
            Assert.AreEqual(searchEvent.Name, "Person Search");
            Assert.AreEqual(searchEvent.Message, "Auto search processing completed successfully. 2 identifiers found.  2 addresses found.");
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
            Assert.AreEqual(searchEvent.ProviderName, "completedProfile");
            Assert.AreEqual(searchEvent.TimeStamp, new DateTime(2003, 3, 3));
            Assert.AreEqual(searchEvent.EventType, "Completed");
            Assert.AreEqual(searchEvent.Name, "Person Search");
            Assert.AreEqual(searchEvent.Message, "Auto search processing completed successfully. 2 identifiers found.  0 addresses found.");
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
            Assert.AreEqual(sSG_Identifier.Identification, "1111111");
            Assert.AreEqual(sSG_Identifier.IdentificationEffectiveDate, new DateTime(2002, 2, 2));
            Assert.AreEqual(sSG_Identifier.IdentificationExpirationDate, new DateTime(2003, 3, 3));
            Assert.AreEqual(sSG_Identifier.IdentifierType, IdentificationType.DriverLicense.Value);
            Assert.AreEqual(sSG_Identifier.InformationSource, InformationSourceType.ICBC.Value);
            Assert.AreEqual(sSG_Identifier.StatusCode, 1);
            Assert.AreEqual(sSG_Identifier.StateCode, 0);
        }
    }
}

