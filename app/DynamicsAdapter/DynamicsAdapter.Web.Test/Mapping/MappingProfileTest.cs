using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
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
            PersonalIdentifier identifier = _mapper.Map<PersonalIdentifier>(sSG_Identifier);
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
                },
                DataProviders = new SSG_SearchapiRequestDataProvider[]
                {
                    new SSG_SearchapiRequestDataProvider(){Name="ICBC"},
                    new SSG_SearchapiRequestDataProvider(){Name="BC Hydro"}
                }
            };
            PersonSearchRequest personSearchRequest = _mapper.Map<PersonSearchRequest>(sSG_SearchApiRequest);
            Assert.AreEqual("firstName", personSearchRequest.FirstName);
            Assert.AreEqual("lastName", personSearchRequest.LastName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2002, 2, 2)), personSearchRequest.DateOfBirth);
            Assert.AreEqual(2, personSearchRequest.Identifiers.Count);
            Assert.AreEqual(2, personSearchRequest.DataProviders.Count);
        }

        [Test]
        public void Employment_should_map_to_SSG_Employment_correctly()
        {
            var employment = new Employment()
            {
                Employer = new Employer()
                {
                    ContactPerson = "Person",
                    Name = "Name",
                    OwnerName = "OwnerName",
                    
                    Address = new Address
                    {
                        AddressLine1 = "AddressLine1",
                        AddressLine2 = "AddressLine2",
                        AddressLine3 = "AddressLine3",
                        StateProvince = "Manitoba",
                        City = "testCity",
                        Type = "residence",
                        CountryRegion = "canada",
                        ZipPostalCode = "p3p3p3",
                    }
                },
               IncomeAssistance = false,
               EmploymentConfirmed = true,
               IncomeAssistanceStatus = "Status",
               Occupation = "Occupation",
               Website = "Website",
                ReferenceDates = new List<ReferenceDate>(){
                                new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                            },
                Description = "description",
                Notes = "notes"
            };
            EmploymentEntity ssg_empl = _mapper.Map<EmploymentEntity>(employment);
            Assert.AreEqual("Person", ssg_empl.ContactPerson);
            Assert.AreEqual("Name", ssg_empl.BusinessName);
            Assert.AreEqual("OwnerName", ssg_empl.BusinessOwner);
            Assert.AreEqual(true, ssg_empl.EmploymentConfirmed);
            Assert.AreEqual("Occupation", ssg_empl.Occupation);
            Assert.AreEqual("Website", ssg_empl.Website);
            Assert.AreEqual("Status", ssg_empl.IncomeAssistanceStatus);
            Assert.AreEqual("AddressLine1", ssg_empl.AddressLine1);
            Assert.AreEqual("AddressLine2", ssg_empl.AddressLine2);
            Assert.AreEqual("AddressLine3", ssg_empl.AddressLine3);
            Assert.AreEqual("Manitoba", ssg_empl.CountrySubdivisionText);
            Assert.AreEqual("testCity", ssg_empl.City);
            Assert.AreEqual("canada", ssg_empl.CountryText);
            Assert.AreEqual("p3p3p3", ssg_empl.PostalCode);
            Assert.AreEqual(1, ssg_empl.StatusCode);
            Assert.AreEqual(0, ssg_empl.StateCode);
            Assert.AreEqual("Start Date", ssg_empl.Date1Label);
            Assert.AreEqual("End Date", ssg_empl.Date2Label);
            Assert.AreEqual(new DateTime(2019, 9, 1), ssg_empl.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), ssg_empl.Date2);
        }

        [Test]
        public void Employment_with_null_employer_should_map_to_SSG_Employment_correctly()
        {
            var employment = new Employment()
            {
                Employer = null,
                IncomeAssistance = false,
                EmploymentConfirmed = true,
                IncomeAssistanceStatus = "Status",
                Occupation = "Occupation",
                Website = "Website",
                ReferenceDates = new List<ReferenceDate>(){
                                new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                            },
                Description = "description",
                Notes = "notes"
            };
            EmploymentEntity ssg_empl = _mapper.Map<EmploymentEntity>(employment);
            Assert.AreEqual(string.Empty, ssg_empl.ContactPerson);
            Assert.AreEqual(string.Empty, ssg_empl.BusinessName);
            Assert.AreEqual(string.Empty, ssg_empl.BusinessOwner);
            Assert.AreEqual(true, ssg_empl.EmploymentConfirmed);
            Assert.AreEqual(false, ssg_empl.IncomeAssistance);
            Assert.AreEqual("Occupation", ssg_empl.Occupation);
            Assert.AreEqual("Website", ssg_empl.Website);
            Assert.AreEqual("Status", ssg_empl.IncomeAssistanceStatus);
            Assert.AreEqual(string.Empty, ssg_empl.AddressLine1);
            Assert.AreEqual(string.Empty, ssg_empl.AddressLine2);
            Assert.AreEqual(string.Empty, ssg_empl.AddressLine3);
            Assert.AreEqual(string.Empty, ssg_empl.CountrySubdivisionText);
            Assert.AreEqual(string.Empty, ssg_empl.City);
            Assert.AreEqual(string.Empty, ssg_empl.CountryText);
            Assert.AreEqual(string.Empty, ssg_empl.PostalCode);
            Assert.AreEqual(1, ssg_empl.StatusCode);
            Assert.AreEqual(0, ssg_empl.StateCode);
            Assert.AreEqual("Start Date", ssg_empl.Date1Label);
            Assert.AreEqual("End Date", ssg_empl.Date2Label);
            Assert.AreEqual(new DateTime(2019, 9, 1), ssg_empl.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), ssg_empl.Date2);
        }

        [Test]
        public void Address_should_map_to_SSG_Address_correctly()
        {
            var address = new Address()
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                AddressLine3 = "AddressLine3",
                StateProvince = "Manitoba",
                City = "testCity",
                Type = "residence",
                CountryRegion = "canada",
                ZipPostalCode = "p3p3p3",
                ReferenceDates = new List<ReferenceDate>(){
                                new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                            },
                Description = "description",
                Notes = "notes"
            };
            SSG_Address ssg_addr = _mapper.Map<SSG_Address>(address);
            Assert.AreEqual("AddressLine1", ssg_addr.AddressLine1);
            Assert.AreEqual("AddressLine2", ssg_addr.AddressLine2);
            Assert.AreEqual("AddressLine3", ssg_addr.AddressLine3);
            Assert.AreEqual("Manitoba", ssg_addr.CountrySubdivisionText);
            Assert.AreEqual("testCity", ssg_addr.City);
            Assert.AreEqual("canada", ssg_addr.CountryText);
            Assert.AreEqual(LocationType.Residence.Value, ssg_addr.Category);
            Assert.AreEqual("p3p3p3", ssg_addr.PostalCode);
            Assert.AreEqual(1, ssg_addr.StatusCode);
            Assert.AreEqual(0, ssg_addr.StateCode);
            Assert.AreEqual("Start Date", ssg_addr.Date1Label);
            Assert.AreEqual("End Date", ssg_addr.Date2Label);
            Assert.AreEqual(new DateTime(2019, 9, 1), ssg_addr.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), ssg_addr.Date2);
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
                    Identifiers = new PersonalIdentifier[]
                    {
                        new PersonalIdentifier(){ },
                        new PersonalIdentifier(){ }
                    },
                    Addresses = new Address[]
                    {
                        new Address(){ },
                        new Address(){ }
                    },
                    Phones = new Phone[]
                    {
                        new Phone(){ },
                        new Phone(){ }
                    },
                    Names = new Name[]
                    {
                        new Name() {},
                        new Name () {}
                    },
                    Employments = new Employment[]
                    {
                        new Employment(){}
                    }
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. 2 identifier(s) found.  2 addresses found. 2 phone number(s) found. 2 name(s) found. 1 employment(s) found. 0 related person(s) found.", searchEvent.Message);
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
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual(Keys.SEARCH_API_EVENT_NAME, searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. 2 identifier(s) found.  0 addresses found. 0 phone number(s) found. 0 name(s) found. 0 employment(s) found. 0 related person(s) found.", searchEvent.Message);
        }

        [Test]
        public void PersonalIdentifierActual_should_map_to_SSG_Identifier_correctly()
        {
            PersonalIdentifier identifier = new PersonalIdentifier()
            {
                Value = "1111111",
                Type = PersonalIdentifierType.DriverLicense,
                IssuedBy = "BC",
                Description = "description",
                Notes = "notes",
                TypeCode="BCDL",
                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="startDate", Value=new DateTime(2012,1,1) },
                    new ReferenceDate(){Index=1, Key="endDate", Value=new DateTime(2014,1,1) },
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
        public void PhoneNumber_should_map_to_SSG_PhoneNumber_correctly()
        {
            Phone phoneNumber = new Phone()
            {               
               
                PhoneNumber = "6904005678",
                Type = "home",
                Extension ="123",
                Description = "Description"

               
            };
            SSG_PhoneNumber sSG_PhoneNumber = _mapper.Map<SSG_PhoneNumber>(phoneNumber);
            Assert.AreEqual("6904005678", sSG_PhoneNumber.TelePhoneNumber);
            Assert.AreEqual("123", sSG_PhoneNumber.PhoneExtension);
            Assert.AreEqual("home", sSG_PhoneNumber.SupplierTypeCode);
            Assert.AreEqual(TelephoneNumberType.Home.Value, sSG_PhoneNumber.TelephoneNumberType);
            Assert.AreEqual(1, sSG_PhoneNumber.StatusCode);
            Assert.AreEqual(0, sSG_PhoneNumber.StateCode);
        }

        [Test]
        public void PhoneNumber_with_unknown_type_and_no_descriptionshould_map_to_SSG_PhoneNumber_correctly()
        {
            Phone phoneNumber = new Phone()
            {

                PhoneNumber = "6904005678",
                Type = "Phone",
                Extension = "123"


            };
            SSG_PhoneNumber sSG_PhoneNumber = _mapper.Map<SSG_PhoneNumber>(phoneNumber);
            Assert.AreEqual("6904005678", sSG_PhoneNumber.TelePhoneNumber);
            Assert.AreEqual("123", sSG_PhoneNumber.PhoneExtension);
            Assert.AreEqual("Phone", sSG_PhoneNumber.SupplierTypeCode);
            Assert.AreEqual(TelephoneNumberType.Other.Value, sSG_PhoneNumber.TelephoneNumberType);
            Assert.AreEqual(1, sSG_PhoneNumber.StatusCode);
            Assert.AreEqual(0, sSG_PhoneNumber.StateCode);
        }



        [Test]
        public void Person_should_map_to_SSG_Person_Upload_correctly()
        {
            var person = new Person()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                OtherName = "OtherName",
               DateOfBirth = new DateTimeOffset(new DateTime(2011, 1, 1)),
               DateDeathConfirmed = true,
               DateOfDeath = new DateTimeOffset(new DateTime(2011, 1, 1)),
                Gender = "M",
                Notes = "Some notes",
               Incacerated = "Yes"
               
            };
            PersonEntity ssg_person = _mapper.Map<PersonEntity>(person);
            Assert.AreEqual("FirstName", ssg_person.FirstName);
            Assert.AreEqual("LastName", ssg_person.LastName);
            Assert.AreEqual("MiddleName", ssg_person.MiddleName);
            Assert.AreEqual("OtherName", ssg_person.ThirdGivenName);
            Assert.AreEqual(new DateTime(2011, 1, 1), ssg_person.DateOfBirth);
            Assert.AreEqual(new DateTime(2011, 1, 1), ssg_person.DateOfDeath);
            Assert.AreEqual(true, ssg_person.DateOfDeathConfirmed);
            Assert.AreEqual("M", ssg_person.Gender);
            Assert.AreEqual("Some notes", ssg_person.Notes);
            Assert.AreEqual(NullableBooleanType.Yes.Value, ssg_person.Incacerated);
          
        }

        [Test]
        public void Person_should_map_null_dates_to_SSG_Person_Upload_correctly()
        {
            var person = new Person()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                OtherName = "OtherName",
                DateOfBirth = null,
                DateOfDeath = null,
                DateDeathConfirmed = false,
                Gender = "M",
                Notes = "Some notes",
                

            };
            PersonEntity ssg_person = _mapper.Map<PersonEntity>(person);
            Assert.AreEqual("FirstName", ssg_person.FirstName);
            Assert.AreEqual("LastName", ssg_person.LastName);
            Assert.AreEqual("MiddleName", ssg_person.MiddleName);
            Assert.AreEqual("OtherName", ssg_person.ThirdGivenName);
            Assert.AreEqual(false, ssg_person.DateOfDeathConfirmed);
            Assert.AreEqual(null, ssg_person.DateOfBirth);
            Assert.AreEqual(null, ssg_person.DateOfDeath);
            Assert.AreEqual("M", ssg_person.Gender);
            Assert.AreEqual("Some notes", ssg_person.Notes);

        }


        [Test]
        public void Person_should_map_null_incacerated_datedealthconfirmed_to_SSG_Person_Upload_correctly()
        {
            var person = new Person()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                OtherName = "OtherName",
                DateOfBirth = new DateTimeOffset(new DateTime(2011, 1, 1)),
                DateOfDeath = new DateTimeOffset(new DateTime(2011, 1, 1)),
                DateDeathConfirmed = null,
                Gender = "M",
                Notes = "Some notes",
                Incacerated = null


            };
            PersonEntity ssg_person = _mapper.Map<PersonEntity>(person);
            Assert.AreEqual("FirstName", ssg_person.FirstName);
            Assert.AreEqual("LastName", ssg_person.LastName);
            Assert.AreEqual("MiddleName", ssg_person.MiddleName);
            Assert.AreEqual("OtherName", ssg_person.ThirdGivenName);
            Assert.AreEqual(null, ssg_person.DateOfDeathConfirmed);
            Assert.AreEqual(new DateTime(2011, 1, 1), ssg_person.DateOfBirth);
            Assert.AreEqual(new DateTime(2011, 1, 1), ssg_person.DateOfDeath);
            Assert.AreEqual("M", ssg_person.Gender);
            Assert.AreEqual(null, ssg_person.Incacerated);

        }

        [Test]
        public void Name_should_map_to_SSG_Name_correctly()
        {
            var name = new Name()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                OtherName ="OtherName",
                Type = "Former",
                Description = "test name",
                Notes = "notes",
                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="startDate", Value=new DateTime(2012,1,1) },
                    new ReferenceDate(){Index=1, Key="endDate", Value=new DateTime(2014,1,1) },
                }
            };
            SSG_Aliase ssg_name = _mapper.Map<SSG_Aliase>(name);
            Assert.AreEqual("FirstName", ssg_name.FirstName);
            Assert.AreEqual("LastName", ssg_name.LastName);
            Assert.AreEqual("MiddleName", ssg_name.MiddleName);
            Assert.AreEqual("OtherName", ssg_name.ThirdGivenName);
            Assert.AreEqual(PersonNameCategory.Other.Value, ssg_name.Type);
            Assert.AreEqual("test name", ssg_name.Comments);
            Assert.AreEqual("notes", ssg_name.Notes);
            Assert.AreEqual("Former", ssg_name.SupplierTypeCode);
            Assert.AreEqual(1, ssg_name.StatusCode);
            Assert.AreEqual(0, ssg_name.StateCode);
            Assert.AreEqual(new DateTime(2012, 1, 1), ssg_name.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), ssg_name.Date2);
            Assert.AreEqual("startDate", ssg_name.Date1Label);
            Assert.AreEqual("endDate", ssg_name.Date2Label);
        }

        [Test]
        public void RelatedPerson_should_map_to_SSG_Identity_correctly()
        {
            var relatedPerson = new RelatedPerson()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                OtherName = "OtherName",
                Type = "Aunt",
                Description = "test description",
                Notes = "notes",
                Gender = "U",
                DateOfBirth = new DateTimeOffset(new DateTime(2012,3,4)),
                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="relation start date", Value=new DateTimeOffset(new DateTime(2012,1,1)) },
                    new ReferenceDate(){Index=1, Key="relation end date", Value=new DateTimeOffset(new DateTime(2014,1,1) )}
                }
            };
            SSG_Identity ssg_relatedPerson = _mapper.Map<SSG_Identity>(relatedPerson);
            Assert.AreEqual("FirstName", ssg_relatedPerson.FirstName);
            Assert.AreEqual("LastName", ssg_relatedPerson.LastName);
            Assert.AreEqual("MiddleName", ssg_relatedPerson.MiddleName);
            Assert.AreEqual("OtherName", ssg_relatedPerson.ThirdGivenName);
            Assert.AreEqual("test description", ssg_relatedPerson.Description);
            Assert.AreEqual("notes", ssg_relatedPerson.Notes);
            Assert.AreEqual(867670002, ssg_relatedPerson.Type);
            Assert.AreEqual("Aunt", ssg_relatedPerson.SupplierRelationType);
            Assert.AreEqual(new DateTime(2012, 3, 4), ssg_relatedPerson.DateOfBirth);
            Assert.AreEqual(867670002, ssg_relatedPerson.Gender);
            Assert.AreEqual(1, ssg_relatedPerson.StatusCode);
            Assert.AreEqual(0, ssg_relatedPerson.StateCode);
            Assert.AreEqual(new DateTime(2012, 1, 1), ssg_relatedPerson.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), ssg_relatedPerson.Date2);
            Assert.AreEqual("relation start date", ssg_relatedPerson.Date1Label);
            Assert.AreEqual("relation end date", ssg_relatedPerson.Date2Label);
        }

        [Test]
        public void SSG_SearchapiRequestDataProvider_should_map_to_DataProvider_correctly()
        {
            var dp = new SSG_SearchapiRequestDataProvider()
            {
                Name="dp1",
                SuppliedByValue=1
            };
            DataProvider provider = _mapper.Map<DataProvider>(dp);
            Assert.AreEqual("dp1", provider.Name);
        }
    }
}

