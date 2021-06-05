using AutoMapper;
using DynamicsAdapter.Web.Mapping;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Email;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Fams3Adapter.Dynamics.Vehicle;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
            IdentifierEntity sSG_Identifier = new IdentifierEntity()
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
                    new SSG_SearchapiRequestDataProvider(){AdaptorName="ICBC"},
                    new SSG_SearchapiRequestDataProvider(){AdaptorName="BC Hydro"}
                },
                SearchRequest = new SSG_SearchRequest()
                {
                    FileId = "testFileId",
                    ApplicantFirstName = "applicantFirstName",
                    ApplicantLastName = "applicantLastName",
                    SearchReason = new SSG_SearchRequestReason { ReasonCode = "EnfPayAgr" }
                },
                SequenceNumber = "123456",
                IsPrescreenSearch = true,
                JCAPersonBirthDate = new DateTime(1999, 1, 1),
                JCAMotherBirthSurname = "MotherMaidName",
                JCAGender = GenderType.Female.Value
            };
            PersonSearchRequest personSearchRequest = _mapper.Map<PersonSearchRequest>(sSG_SearchApiRequest);
            Assert.AreEqual("firstName", personSearchRequest.FirstName);
            Assert.AreEqual("lastName", personSearchRequest.LastName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(1999,1,1)), personSearchRequest.JcaPerson.BirthDate);
            Assert.AreEqual("f", personSearchRequest.JcaPerson.Gender);
            Assert.AreEqual("MotherMaidName", personSearchRequest.JcaPerson.MotherMaidName);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2002, 2, 2)), personSearchRequest.DateOfBirth);
            Assert.AreEqual(2, personSearchRequest.Identifiers.Count);
            Assert.AreEqual(2, personSearchRequest.DataProviders.Count);
            Assert.AreEqual(1, personSearchRequest.Names.Count);
            Assert.AreEqual(true, personSearchRequest.IsPreScreenSearch);
            Assert.AreEqual(OwnerType.Applicant, personSearchRequest.Names.ElementAt(0).Owner);
            Assert.AreEqual("testFileId_123456", personSearchRequest.SearchRequestKey);
            Assert.AreEqual(SearchReasonCode.EnfPayAgr, personSearchRequest.Agency.ReasonCode);
        }

        [Test]
        public void SSG_SearchApiRequest_null_applicant_should_map_to_PersonSearchRequest_correctly()
        {
            SSG_SearchApiRequest sSG_SearchApiRequest = new SSG_SearchApiRequest()
            {
                SearchRequest = new SSG_SearchRequest()
                {
                    FileId = "testFileId",
                    ApplicantFirstName = null,
                    ApplicantLastName = null
                },
                SequenceNumber = "123456"
            };
            PersonSearchRequest personSearchRequest = _mapper.Map<PersonSearchRequest>(sSG_SearchApiRequest);
            Assert.AreEqual(0, personSearchRequest.Identifiers.Count);
            Assert.AreEqual(0, personSearchRequest.DataProviders.Count);
            Assert.AreEqual(0, personSearchRequest.Names.Count);
            Assert.AreEqual("testFileId_123456", personSearchRequest.SearchRequestKey);
        }

        [Test]
        public void SSG_SearchApiRequest_null_SearchReason_should_map_to_PersonSearchRequest_correctly()
        {
            SSG_SearchApiRequest sSG_SearchApiRequest = new SSG_SearchApiRequest()
            {
                SearchRequest = new SSG_SearchRequest()
                {
                    SearchReason = null
                },
            };
            PersonSearchRequest personSearchRequest = _mapper.Map<PersonSearchRequest>(sSG_SearchApiRequest);
            Assert.AreEqual(SearchReasonCode.Other, personSearchRequest.Agency.ReasonCode);
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
                InformationSourceCode = "UI00",
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
            Assert.AreEqual("UI00", ssg_empl.InformationSourceCode);
            Assert.AreEqual(IncomeAssistanceStatusType.Unknown.Value, ssg_empl.IncomeAssistanceStatusOption);
            Assert.AreEqual("AddressLine1", ssg_empl.AddressLine1);
            Assert.AreEqual("AddressLine2", ssg_empl.AddressLine2);
            Assert.AreEqual("AddressLine3", ssg_empl.AddressLine3);
            Assert.AreEqual("Manitoba", ssg_empl.CountrySubdivisionText);
            Assert.AreEqual("testCity", ssg_empl.City);
            Assert.AreEqual("canada", ssg_empl.CountryText);
            Assert.AreEqual("p3p3p3", ssg_empl.PostalCode);
            Assert.AreEqual(EmploymentRecordType.Employment.Value, ssg_empl.EmploymentType);
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
                IncomeAssistance = true,
                EmploymentConfirmed = true,
                IncomeAssistanceStatus = "Active",
                InformationSourceCode = "YUI00",
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
            Assert.AreEqual(EmploymentRecordType.IncomeAssistance.Value, ssg_empl.EmploymentType);
            Assert.AreEqual("Occupation", ssg_empl.Occupation);
            Assert.AreEqual("Website", ssg_empl.Website);
            Assert.AreEqual("Active", ssg_empl.IncomeAssistanceStatus);
            Assert.AreEqual("YUI00", ssg_empl.InformationSourceCode);
            Assert.AreEqual(IncomeAssistanceStatusType.Active.Value, ssg_empl.IncomeAssistanceStatusOption);
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
        public void Employment_with_closed_status_employer_should_map_to_SSG_Employment_incomeAssistance_employmentType_correctly()
        {
            var employment = new Employment()
            {
                Employer = null,
                IncomeAssistance = false,
                EmploymentConfirmed = false,
                IncomeAssistanceStatus = "Closed",
            };
            EmploymentEntity ssg_empl = _mapper.Map<EmploymentEntity>(employment);
            Assert.AreEqual(false, ssg_empl.EmploymentConfirmed);
            Assert.AreEqual(EmploymentRecordType.IncomeAssistance.Value, ssg_empl.EmploymentType);
            Assert.AreEqual("Closed", ssg_empl.IncomeAssistanceStatus);
            Assert.AreEqual(IncomeAssistanceStatusType.Closed.Value, ssg_empl.IncomeAssistanceStatusOption);
        }

        [Test]
        public void Phone_should_map_to_SSG_EmploymentContact_correctly()
        {
            Phone phoneNumber = new Phone()
            {
                PhoneNumber = "6904005678",
                Type = "Phone",
                Extension = "123",
            };
            EmploymentContactEntity employmentContact = _mapper.Map<EmploymentContactEntity>(phoneNumber);
            Assert.AreEqual("6904005678", employmentContact.PhoneNumber);
            Assert.AreEqual("6904005678", employmentContact.OriginalPhoneNumber);
            Assert.AreEqual("123", employmentContact.PhoneExtension);
            Assert.AreEqual(1, employmentContact.StatusCode);
            Assert.AreEqual(0, employmentContact.StateCode);
        }

        [Test]
        public void Phone_Fax_should_map_to_SSG_EmploymentContact_correctly()
        {
            Phone phoneNumber = new Phone()
            {
                PhoneNumber = "6904005678",
                Type = "fax",
                Extension = "123",
            };
            EmploymentContactEntity employmentContact = _mapper.Map<EmploymentContactEntity>(phoneNumber);
            Assert.AreEqual("6904005678", employmentContact.FaxNumber);
            Assert.AreEqual("6904005678", employmentContact.OriginalFaxNumber);
            Assert.AreEqual("123", employmentContact.PhoneExtension);
            Assert.AreEqual(1, employmentContact.StatusCode);
            Assert.AreEqual(0, employmentContact.StateCode);
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
            AddressEntity addr = _mapper.Map<AddressEntity>(address);
            Assert.AreEqual("AddressLine1", addr.AddressLine1);
            Assert.AreEqual("AddressLine2", addr.AddressLine2);
            Assert.AreEqual("AddressLine3", addr.AddressLine3);
            Assert.AreEqual("Manitoba", addr.CountrySubdivisionText);
            Assert.AreEqual("testCity", addr.City);
            Assert.AreEqual("canada", addr.CountryText);
            Assert.AreEqual(LocationType.Residence.Value, addr.Category);
            Assert.AreEqual("p3p3p3", addr.PostalCode);
            Assert.AreEqual(null, addr.Name);
            Assert.AreEqual(1, addr.StatusCode);
            Assert.AreEqual(0, addr.StateCode);
            Assert.AreEqual("Start Date", addr.Date1Label);
            Assert.AreEqual("End Date", addr.Date2Label);
            Assert.AreEqual(new DateTime(2019, 9, 1), addr.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), addr.Date2);
            Assert.AreEqual("description", addr.Description);
            Assert.AreEqual("notes", addr.Notes);
        }

        [Test]
        public void Email_should_map_to_SSG_Email_correctly()
        {
            var e = new Email()
            {
                EmailAddress="test@test.com",

                ReferenceDates = new List<ReferenceDate>(){
                                new ReferenceDate(){ Index=0, Key="Start Date", Value=new DateTime(2019,9,1) },
                                new ReferenceDate(){ Index=1, Key="End Date", Value=new DateTime(2020,9,1) }
                            },
                Description = "description",
                Notes = "notes"
            };
            EmailEntity email = _mapper.Map<EmailEntity>(e);

            Assert.AreEqual("test@test.com", email.Email);
            Assert.AreEqual("Start Date", email.Date1Label);
            Assert.AreEqual("End Date", email.Date2Label);
            Assert.AreEqual(new DateTime(2019, 9, 1), email.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), email.Date2);
        }

        [Test]
        public void Person_without_cautionflag_should_map_to_SSG_SafetyConcern_correctly()
        {
            var p = new Person()
            {
                CautionFlag="",
                CautionNotes="notes",
                CautionReason="violent",
            };
            SafetyConcernEntity safe = _mapper.Map<SafetyConcernEntity>(p);
            Assert.AreEqual(SafetyConcernType.Other.Value, safe.Type);
            Assert.AreEqual(" violent notes", safe.Detail);
            Assert.AreEqual("violent", safe.SupplierTypeCode);
        }

        [Test]
        public void Person_with_cautionflag_should_map_to_SSG_SafetyConcern_correctly()
        {
            var p = new Person()
            {
                CautionFlag = "Threat",
                CautionNotes = "notes",
                CautionReason = "violent",
            };
            SafetyConcernEntity safe = _mapper.Map<SafetyConcernEntity>(p);
            Assert.AreEqual(SafetyConcernType.Threat.Value, safe.Type);
            Assert.AreEqual("Threat violent notes", safe.Detail);
            Assert.AreEqual("Threat", safe.SupplierTypeCode);
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
            Assert.AreEqual("2003-03-03 00:00 | Accepted | acceptedProfile", searchEvent.Name);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_ACCEPTED, searchEvent.EventType);
            Assert.AreEqual("Auto search has been accepted for processing", searchEvent.Message);
        }

        [Test]
        public void PersonSearchAccepted_with_message_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchAccepted accepted = new PersonSearchAccepted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "acceptedProfile"
                },
                Message = "This is for real"
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(accepted);
            Assert.AreEqual("acceptedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_ACCEPTED, searchEvent.EventType);
            Assert.AreEqual("2003-03-03 00:00 | Accepted | acceptedProfile", searchEvent.Name);
            Assert.AreEqual("Auto search has been accepted for processing. This is for real", searchEvent.Message);
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
            Assert.AreEqual("2003-03-03 00:00 | Rejected | rejectedProfile", searchEvent.Name);
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
            Assert.AreEqual("2003-03-03 00:00 | Rejected | rejectedProfile", searchEvent.Name);
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
            Assert.AreEqual("2003-03-03 00:00 | Failed | failedProfile", searchEvent.Name);
            Assert.AreEqual("Auto search processing failed. Reason: failedCause", searchEvent.Message);
        }

        [Test]
        public void PersonSearchSubmitted_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchSubmitted submitted = new PersonSearchSubmitted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "submittedProfile"
                },
                Message = "SubmittedMessage"
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(submitted);
            Assert.AreEqual("submittedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_SUBMITTED, searchEvent.EventType);
            Assert.AreEqual("2003-03-03 00:00 | Submitted | submittedProfile", searchEvent.Name);
            Assert.AreEqual("SubmittedMessage", searchEvent.Message);
        }

        [Test]
        public void PersonSearchSubmitted_with_null_providerProfile_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchSubmitted submitted = new PersonSearchSubmitted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = null,
                Message = "SubmittedMessage"
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(submitted);
            Assert.AreEqual(null, searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_SUBMITTED, searchEvent.EventType);
            Assert.AreEqual("2003-03-03 00:00 | Submitted | ", searchEvent.Name);
            Assert.AreEqual("SubmittedMessage", searchEvent.Message);
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
                Message = "This is some message",
                MatchedPersons = new List<PersonFound>()
                {
                    new PersonFound(){
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
                        },
                        BankInfos = new BankInfo[]
                        {
                            new BankInfo(){}
                        },
                        OtherAssets = new OtherAsset[]
                        {
                            new OtherAsset(){}
                        },
                        CompensationClaims=null,
                        InsuranceClaims=null
                    },
                    new PersonFound(){
                        FirstName = "firstName",
                        LastName = "lastName",
                        DateOfBirth = new DateTime(2019, 3, 5),
                        Identifiers = new PersonalIdentifier[]
                        {
                            new PersonalIdentifier(){ },

                        },
                        Addresses = null,
                        Phones = null,
                        Emails=new Email[]
                        {
                            new Email()
                        },
                        Names = new Name[]
                        {
                            new Name() {}
                        },
                        RelatedPersons = new RelatedPerson[]
                        {
                            new RelatedPerson(){}
                        },
                        Vehicles=new Vehicle[]
                        {
                            new Vehicle(){}
                        },
                        CompensationClaims= new CompensationClaim[]
                        {
                            new CompensationClaim(){ }
                        },
                        InsuranceClaims = new InsuranceClaim[]
                        {
                            new InsuranceClaim(){ }
                        }
                    }
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual("2003-03-03 00:00 | Completed | completedProfile", searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. This is some message. 2 Matched Persons found.\nFor Matched Person 1 : 2 identifier(s) found.  2 addresses found. 2 phone number(s) found. 0 email(s) found. 2 name(s) found. 1 employment(s) found. 0 related person(s) found. 1 bank info(s) found. 0 vehicle(s) found. 1 other asset(s) found. 0 compensation claim(s) found. 0 insurance claim(s) found.\nFor Matched Person 2 : 1 identifier(s) found.  0 addresses found. 0 phone number(s) found. 1 email(s) found. 1 name(s) found. 0 employment(s) found. 1 related person(s) found. 0 bank info(s) found. 1 vehicle(s) found. 0 other asset(s) found. 1 compensation claim(s) found. 1 insurance claim(s) found.\n", searchEvent.Message);
        }

        [Test]
        public void PersonSearchCompleted_with_0_matchperson_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchCompleted completed = new PersonSearchCompleted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                ProviderProfile = new ProviderProfile()
                {
                    Name = "completedProfile"
                },
                MatchedPersons = new List<PersonFound>()
                { }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual("2003-03-03 00:00 | Completed | completedProfile", searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. 0 Matched Persons found.\n", searchEvent.Message);
        }

        [Test]
        public void PersonSearchCompleted_with_null_addresses_should_map_to_SSG_SearchApiEvent_correctly()
        {
            PersonSearchCompleted completed = new PersonSearchCompleted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2003, 3, 3),
                Message = "Message",
                ProviderProfile = new ProviderProfile()
                {
                    Name = "completedProfile"
                },
                MatchedPersons = new List<PersonFound>()
                {
                    new PersonFound(){
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
                }
            };
            SSG_SearchApiEvent searchEvent = _mapper.Map<SSG_SearchApiEvent>(completed);
            Assert.AreEqual("completedProfile", searchEvent.ProviderName);
            Assert.AreEqual(new DateTime(2003, 3, 3), searchEvent.TimeStamp);
            Assert.AreEqual(Keys.EVENT_COMPLETED, searchEvent.EventType);
            Assert.AreEqual("2003-03-03 00:00 | Completed | completedProfile", searchEvent.Name);
            Assert.AreEqual("Auto search processing completed successfully. Message. 1 Matched Persons found.\nFor Matched Person 1 : 2 identifier(s) found.  0 addresses found. 0 phone number(s) found. 0 email(s) found. 0 name(s) found. 0 employment(s) found. 0 related person(s) found. 0 bank info(s) found. 0 vehicle(s) found. 0 other asset(s) found. 0 compensation claim(s) found. 0 insurance claim(s) found.\n", searchEvent.Message);
        }

        [Test]
        public void PersonalIdentifierActual_should_map_to_SSG_Identifier_correctly()
        {
            PersonalIdentifier identifier = new PersonalIdentifier()
            {
                Value = "1111111",
                Type = PersonalIdentifierType.BCDriverLicense,
                IssuedBy = "BC",
                Description = "description",
                Notes = "notes",
                TypeCode = "BCDL",
                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="startDate", Value=new DateTime(2012,1,1) },
                    new ReferenceDate(){Index=1, Key="endDate", Value=new DateTime(2014,1,1) },
                }
            };
            IdentifierEntity sSG_Identifier = _mapper.Map<IdentifierEntity>(identifier);
            Assert.AreEqual("1111111", sSG_Identifier.Identification);
            Assert.AreEqual("description", sSG_Identifier.Description);
            Assert.AreEqual("notes", sSG_Identifier.Notes);
            Assert.AreEqual("BCDL", sSG_Identifier.SupplierTypeCode);
            Assert.AreEqual("BC", sSG_Identifier.IssuedBy);
            Assert.AreEqual(new DateTime(2012, 1, 1), sSG_Identifier.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), sSG_Identifier.Date2);
            Assert.AreEqual("startDate", sSG_Identifier.Date1Label);
            Assert.AreEqual("endDate", sSG_Identifier.Date2Label);
            Assert.AreEqual(IdentificationType.BCDriverLicense.Value, sSG_Identifier.IdentifierType);
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
                Extension = "123",
                Description = "Description",
                Notes = "notes"

            };
            PhoneNumberEntity sSG_PhoneNumber = _mapper.Map<PhoneNumberEntity>(phoneNumber);
            Assert.AreEqual("6904005678", sSG_PhoneNumber.TelePhoneNumber);
            Assert.AreEqual("123", sSG_PhoneNumber.PhoneExtension);
            Assert.AreEqual("home", sSG_PhoneNumber.SupplierTypeCode);
            Assert.AreEqual(TelephoneNumberType.Home.Value, sSG_PhoneNumber.TelephoneNumberType);
            Assert.AreEqual(1, sSG_PhoneNumber.StatusCode);
            Assert.AreEqual(0, sSG_PhoneNumber.StateCode);
            Assert.AreEqual("Description", sSG_PhoneNumber.Description);
            Assert.AreEqual("notes", sSG_PhoneNumber.Notes);
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
            PhoneNumberEntity sSG_PhoneNumber = _mapper.Map<PhoneNumberEntity>(phoneNumber);
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
                Incacerated = "Yes",
                WearGlasses = "Yes",
                HairColour = "Brown",
                Complexion = "light",
                Weight = "200",
                Height = "180",
                EyeColour = "black",
                DistinguishingFeatures = "features"
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
            Assert.AreEqual("black", ssg_person.EyeColor);
            Assert.AreEqual("Brown", ssg_person.HairColor);
            Assert.AreEqual("Yes", ssg_person.WearGlasses);
            Assert.AreEqual("180", ssg_person.Height);
            Assert.AreEqual("200", ssg_person.Weight);
            Assert.AreEqual("features", ssg_person.DistinguishingFeatures);
            Assert.AreEqual("light", ssg_person.Complexion);
            Assert.AreEqual(GenderType.Male.Value, ssg_person.GenderOptionSet);
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
                OtherName = "OtherName",
                Type = "Former",
                Description = "test name",
                Notes = "notes",
                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="startDate", Value=new DateTime(2012,1,1) },
                    new ReferenceDate(){Index=1, Key="endDate", Value=new DateTime(2014,1,1) },
                }
            };
            AliasEntity ssg_name = _mapper.Map<AliasEntity>(name);
            Assert.AreEqual("FirstName", ssg_name.FirstName);
            Assert.AreEqual("LastName", ssg_name.LastName);
            Assert.AreEqual("MiddleName", ssg_name.MiddleName);
            Assert.AreEqual("OtherName", ssg_name.ThirdGivenName);
            Assert.AreEqual(PersonNameCategory.Other.Value, ssg_name.Type);
            Assert.AreEqual("notes test name", ssg_name.Notes);
            Assert.AreEqual("Former", ssg_name.SupplierTypeCode);
            Assert.AreEqual(1, ssg_name.StatusCode);
            Assert.AreEqual(0, ssg_name.StateCode);
            Assert.AreEqual(new DateTime(2012, 1, 1), ssg_name.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), ssg_name.Date2);
            Assert.AreEqual("startDate", ssg_name.Date1Label);
            Assert.AreEqual("endDate", ssg_name.Date2Label);
        }

        [Test]
        public void Name_with_null_type_should_map_to_SSG_Name_correctly()
        {
            var name = new Name()
            {
                FirstName = "FirstName",
                Type = null
            };
            AliasEntity ssg_name = _mapper.Map<AliasEntity>(name);
            Assert.AreEqual("FirstName", ssg_name.FirstName);
            Assert.AreEqual(null, ssg_name.SupplierTypeCode);
            Assert.AreEqual(PersonNameCategory.Other.Value, ssg_name.Type);
        }

        [Test]
        public void Name_Cornet_with_null_type_should_map_to_SSG_Name_correctly()
        {
            var name = new Name()
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Type = null,
                Owner = OwnerType.Applicant,
                DateOfBirth = new DateTimeOffset(new DateTime(2000, 1, 4))
            };
            AliasEntity ssg_name = _mapper.Map<AliasEntity>(name);
            Assert.AreEqual("FirstName", ssg_name.FirstName);
            Assert.AreEqual(null, ssg_name.SupplierTypeCode);
            Assert.AreEqual(PersonNameCategory.Other.Value, ssg_name.Type);
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
                DateOfBirth = new DateTimeOffset(new DateTime(2012, 3, 4)),
                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="relation start date", Value=new DateTimeOffset(new DateTime(2012,1,1)) },
                    new ReferenceDate(){Index=1, Key="relation end date", Value=new DateTimeOffset(new DateTime(2014,1,1) )}
                }
            };
            RelatedPersonEntity ssg_relatedPerson = _mapper.Map<RelatedPersonEntity>(relatedPerson);
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
            Assert.AreEqual(867670000, ssg_relatedPerson.PersonType);
        }

        [Test]
        public void RelatedPerson_LALW_should_map_to_SSG_Identity_MayLiveWith_correctly()
        {
            var relatedPerson = new RelatedPerson()
            {
                Type = "LALW",
            };
            RelatedPersonEntity ssg_relatedPerson = _mapper.Map<RelatedPersonEntity>(relatedPerson);
            Assert.AreEqual(867670008, ssg_relatedPerson.Type);
            Assert.AreEqual(867670000, ssg_relatedPerson.PersonType);
            Assert.AreEqual("LALW", ssg_relatedPerson.SupplierRelationType);
            Assert.AreEqual(1, ssg_relatedPerson.StatusCode);
            Assert.AreEqual(0, ssg_relatedPerson.StateCode);

        }

        [Test]
        public void RelatedPerson_Subscriber_should_map_to_SSG_Identity_AccountHolder_correctly()
        {
            var relatedPerson = new RelatedPerson()
            {
                Type = "Subscriber",
            };
            RelatedPersonEntity ssg_relatedPerson = _mapper.Map<RelatedPersonEntity>(relatedPerson);
            Assert.AreEqual(867670011, ssg_relatedPerson.Type);
            Assert.AreEqual(867670000, ssg_relatedPerson.PersonType);
            Assert.AreEqual("Subscriber", ssg_relatedPerson.SupplierRelationType);
            Assert.AreEqual(1, ssg_relatedPerson.StatusCode);
            Assert.AreEqual(0, ssg_relatedPerson.StateCode);

        }

        [Test]
        public void RelatedPerson_Dependent_should_map_to_SSG_Identity_AccountHolder_correctly()
        {
            var relatedPerson = new RelatedPerson()
            {
                Type = "Dependent",
            };
            RelatedPersonEntity ssg_relatedPerson = _mapper.Map<RelatedPersonEntity>(relatedPerson);
            Assert.AreEqual(867670003, ssg_relatedPerson.Type);
            Assert.AreEqual(867670000, ssg_relatedPerson.PersonType);
            Assert.AreEqual("Dependent", ssg_relatedPerson.SupplierRelationType);
            Assert.AreEqual(1, ssg_relatedPerson.StatusCode);
            Assert.AreEqual(0, ssg_relatedPerson.StateCode);

        }

        [Test]
        public void BankInfo_should_map_to_BankingInformationEntity_correctly()
        {
            var bank = new BankInfo()
            {
                Branch = "Branch",
                TransitNumber = "123456",
                BankName = "Test123",
                AccountNumber = "666666",
                Description = "test description",
                Notes = "notes",

                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="account start date", Value=new DateTimeOffset(new DateTime(2012,1,1)) },
                    new ReferenceDate(){Index=1, Key="account end date", Value=new DateTimeOffset(new DateTime(2014,1,1) )}
                }
            };
            BankingInformationEntity ssg_bank = _mapper.Map<BankingInformationEntity>(bank);
            Assert.AreEqual("666666", ssg_bank.AccountNumber);
            Assert.AreEqual("Test123", ssg_bank.BankName);
            Assert.AreEqual("Branch", ssg_bank.Branch);
            Assert.AreEqual("123456", ssg_bank.TransitNumber);
            Assert.AreEqual("notes test description", ssg_bank.Notes);
            Assert.AreEqual(1, ssg_bank.StatusCode);
            Assert.AreEqual(0, ssg_bank.StateCode);
            Assert.AreEqual(new DateTime(2012, 1, 1), ssg_bank.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), ssg_bank.Date2);
            Assert.AreEqual("account start date", ssg_bank.Date1Label);
            Assert.AreEqual("account end date", ssg_bank.Date2Label);
        }

        [Test]
        public void Vehicle_should_map_to_VehicleEntity_correctly()
        {
            var v = new Vehicle()
            {
                PlateNumber = "123456",
                OwnershipType = "single owner",
                Vin = "Test123VinTest",
                Description = "description",
                Notes = "notes",
                Owners = new List<InvolvedParty>() {
                    new InvolvedParty(){}
                },

                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="start date", Value=new DateTimeOffset(new DateTime(2012,1,1)) },
                    new ReferenceDate(){Index=1, Key="end date", Value=new DateTimeOffset(new DateTime(2014,1,1) )}
                }
            };
            VehicleEntity vehicleEntity = _mapper.Map<VehicleEntity>(v);
            Assert.AreEqual("single owner", vehicleEntity.OwnershipType);
            Assert.AreEqual("123456", vehicleEntity.PlateNumber);
            Assert.AreEqual("Test123VinTest", vehicleEntity.Vin);
            Assert.AreEqual("description", vehicleEntity.Discription);
            Assert.AreEqual("notes", vehicleEntity.Notes);
            Assert.AreEqual(1, vehicleEntity.StatusCode);
            Assert.AreEqual(0, vehicleEntity.StateCode);
            Assert.AreEqual(new DateTime(2012, 1, 1), vehicleEntity.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), vehicleEntity.Date2);
            Assert.AreEqual("start date", vehicleEntity.Date1Label);
            Assert.AreEqual("end date", vehicleEntity.Date2Label);
        }

        [Test]
        public void InvolvedParty_should_map_to_SSG_AssetOwner_correctly()
        {
            var owner = new InvolvedParty()
            {
                Description = "desc",
                Name = new Name()
                {
                    FirstName = "firstname",
                    LastName = "lastname",
                    MiddleName = "middlename",
                    OtherName = "othername"
                },
                Notes = "notes",
                Organization = "orgname",
                Type = "ownerType"
            };

            AssetOwnerEntity assetOwner = _mapper.Map<AssetOwnerEntity>(owner);
            Assert.AreEqual("lastname", assetOwner.LastName);
            Assert.AreEqual("firstname", assetOwner.FirstName);
            Assert.AreEqual("middlename", assetOwner.MiddleName);
            Assert.AreEqual("othername", assetOwner.OtherName);
            Assert.AreEqual("desc", assetOwner.Description);
            Assert.AreEqual("orgname", assetOwner.OrganizationName);
            Assert.AreEqual("notes", assetOwner.Notes);
            Assert.AreEqual("ownerType", assetOwner.Type);
        }

        [Test]
        public void OtherAsset_should_map_to_AssetOtherEntity_correctly()
        {
            var otherAsset = new OtherAsset()
            {
                TypeDescription = "TypeDescription",
                ReferenceDescription = "referenceDescription",
                ReferenceValue = "referenceValue",
                Description = "description",
                Notes = "notes",
                Owners = new List<InvolvedParty>() {
                    new InvolvedParty(){}
                },

                ReferenceDates = new List<ReferenceDate>() {
                    new ReferenceDate(){Index=0, Key="start date", Value=new DateTimeOffset(new DateTime(2012,1,1)) },
                    new ReferenceDate(){Index=1, Key="end date", Value=new DateTimeOffset(new DateTime(2014,1,1) )}
                }
            };
            AssetOtherEntity assetEntity = _mapper.Map<AssetOtherEntity>(otherAsset);
            Assert.AreEqual("TypeDescription", assetEntity.TypeDescription);
            Assert.AreEqual("description", assetEntity.AssetDescription);
            Assert.AreEqual("referenceDescription referenceValue", assetEntity.Description);
            Assert.AreEqual("notes", assetEntity.Notes);
            Assert.AreEqual(1, assetEntity.StatusCode);
            Assert.AreEqual(0, assetEntity.StateCode);
            Assert.AreEqual(new DateTime(2012, 1, 1), assetEntity.Date1);
            Assert.AreEqual(new DateTime(2014, 1, 1), assetEntity.Date2);
            Assert.AreEqual("start date", assetEntity.Date1Label);
            Assert.AreEqual("end date", assetEntity.Date2Label);
        }

        [Test]
        public void CompensationClaim_should_map_to_SSG_Asset_WorkSafeBcClaim_correctly()
        {
            var claim = new CompensationClaim()
            {
                ClaimNumber = "claimNumber",
                ClaimantNumber = "claimant121",
                ClaimStatus = "Processing",
                ClaimType = "Disable compensation",
                Description = "dis",
                Notes = "compensation notes",
                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="compensation Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="compensation Expired Date", Value=new DateTime(2020,9,1) }
                                },
                BankInfo = new BankInfo()
                {
                    Notes = "compensation bank Notes"
                },
                Employer = new Employer()
                {
                    Address = new Address
                    {
                        AddressLine1 = "compensation Employer Address 1",
                        City = "compensation Employer City",
                        StateProvince = "AB",
                        CountryRegion = "Canada",
                        ZipPostalCode = "VR4 123"
                    },
                    ContactPerson = "compensation Employer Surname FirstName",
                    Name = "compensation Employer Sample Company",
                    Phones = new List<Phone>()
                                     {
                                         new Phone {PhoneNumber = "33333333", Extension ="123", Type ="Phone"}
                                     }
                }
            };
            CompensationClaimEntity bcClaim = _mapper.Map<CompensationClaimEntity>(claim);
            Assert.AreEqual("claimNumber", bcClaim.ClaimNumber);
            Assert.AreEqual("claimant121", bcClaim.ClaimantNumber);
            Assert.AreEqual("Processing", bcClaim.ClaimStatus);
            Assert.AreEqual("Disable compensation", bcClaim.ClaimType);
            Assert.AreEqual("dis", bcClaim.Description);
            Assert.AreEqual("compensation notes", bcClaim.Notes);
            Assert.AreEqual(1, bcClaim.StatusCode);
            Assert.AreEqual(0, bcClaim.StateCode);
            Assert.AreEqual(new DateTime(2019, 9, 1), bcClaim.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), bcClaim.Date2);
            Assert.AreEqual("compensation Start Date", bcClaim.Date1Label);
            Assert.AreEqual("compensation Expired Date", bcClaim.Date2Label);
        }

        [Test]
        public void Employer_should_map_to_EmploymentEntity_correctly()
        {
            var employer = new Employer()
            {
                Address = new Address
                {
                    AddressLine1 = "compensation Employer Address 1",
                    City = "compensation Employer City",
                    StateProvince = "AB",
                    CountryRegion = "Canada",
                    ZipPostalCode = "VR4 123"
                },
                
                ContactPerson = "compensation Employer Surname FirstName",
                Name = "compensation Employer Sample Company",
                Phones = new List<Phone>()
                {
                    new Phone {PhoneNumber = "33333333", Extension ="123", Type ="Phone"}
                },
                OwnerName = "owner name"
            };
            EmploymentEntity employmentEntity = _mapper.Map<EmploymentEntity>(employer);
            Assert.AreEqual("compensation Employer Address 1", employmentEntity.AddressLine1);
            Assert.AreEqual(null, employmentEntity.AddressLine2);
            Assert.AreEqual(null, employmentEntity.AddressLine3);
            Assert.AreEqual(null, employmentEntity.Occupation);
            Assert.AreEqual("compensation Employer Sample Company", employmentEntity.BusinessName);
            Assert.AreEqual("owner name", employmentEntity.BusinessOwner);
            Assert.AreEqual("compensation Employer City", employmentEntity.City);
            Assert.AreEqual("compensation Employer Surname FirstName", employmentEntity.ContactPerson);
            Assert.AreEqual("Canada", employmentEntity.CountryText);
            Assert.AreEqual("AB", employmentEntity.CountrySubdivisionText);
            Assert.AreEqual("VR4 123", employmentEntity.PostalCode);
            Assert.AreEqual(null, employmentEntity.Notes);
            Assert.AreEqual(EmploymentRecordType.Employment.Value, employmentEntity.EmploymentType);
            Assert.AreEqual(1, employmentEntity.StatusCode);
            Assert.AreEqual(0, employmentEntity.StateCode);

        }

        [Test]
        public void InsuranceClaim_should_map_to_ICBCClaimEntity_correctly()
        {
            var claim = new InsuranceClaim()
            {
                ClaimNumber = "claimNumber",
                ClaimType = "claimType",
                ClaimStatus = "claimStatus",
                Adjustor = new Name() { FirstName = "adjusterFirstName", LastName = "adjusterLastName", MiddleName = "adjusterMiddleName", OtherName = "adjusterOtherName" },
                AdjustorPhone = new Phone() { PhoneNumber = "adjusterPhoneNumber", Extension = "adjusterPhoneExtension" },
                ClaimCentre = new ClaimCentre()
                {
                    Location = "claimCenterLocation",
                    ContactAddress = new Address()
                    {
                        AddressLine1 = "claimCenterAddressLine1",
                        AddressLine2 = "claimCenterAddressLine2",
                        AddressLine3 = "claimCenterAddressLine3",
                        City = "city",
                        StateProvince = "province",
                        CountryRegion = "claimCenterCountry",
                        ZipPostalCode = "claimCenterPostalCode"
                    },
                    ContactNumber = new List<Phone>()
                    {
                        new Phone(){PhoneNumber="claimCenterContactPhoneNumber1", Extension="claimCenterContactPhoneExt1", Type="Phone"},
                        new Phone(){PhoneNumber="claimCenterContactFaxNumber", Extension="", Type="Fax"}
                    }
                },
                Identifiers = new List<PersonalIdentifier>()
                {
                    new PersonalIdentifier()
                    {
                        Value="InsuranceClaimBCDLNumber",
                        Type=PersonalIdentifierType.BCDriverLicense,
                        ReferenceDates=new List<ReferenceDate>()
                        {
                            new ReferenceDate()
                            {
                                Index=0,
                                Key="ExpiryDate",
                                Value=new DateTimeOffset(new DateTime(2002, 2, 2))
                            }
                        },
                        Description="BCDLStatus",
                        TypeCode="bcdl"
                    },
                    new PersonalIdentifier()
                    {
                        Value="InsuranceClaimPHNNumber",
                        Type=PersonalIdentifierType.PersonalHealthNumber,
                        TypeCode="phn"
                    }
                },
                InsuredParties = new List<InvolvedParty>()
                {
                    new InvolvedParty()
                    {
                        Name=new Name(){ FirstName="InvolvedPartyFirstName", LastName="InvolvedPartLastName", MiddleName="InvolvedPartyMiddleName", OtherName="InvolvedPartyOtherName"},
                        Organization="InvolvedPartyOrgName",
                        Description="InvolvedPartyDescription",
                        Type="InvolvedPartyTypeCode",
                        Notes="InvolvedPartyNotes",
                        TypeDescription="InvolvedPartyTypeDescription"
                    }
                },
                Description = "dis",
                Notes = "insurance notes",
                ReferenceDates = new List<ReferenceDate>(){
                                    new ReferenceDate(){ Index=0, Key="insurance Start Date", Value=new DateTime(2019,9,1) },
                                    new ReferenceDate(){ Index=1, Key="insurance Expired Date", Value=new DateTime(2020,9,1) }
                                },
            };
            ICBCClaimEntity icbcClaim = _mapper.Map<ICBCClaimEntity>(claim);
            Assert.AreEqual("claimNumber", icbcClaim.ClaimNumber);
            Assert.AreEqual("claimType", icbcClaim.ClaimType);
            Assert.AreEqual("claimStatus", icbcClaim.ClaimStatus);
            Assert.AreEqual("adjusterFirstName", icbcClaim.AdjusterFirstName);
            Assert.AreEqual("adjusterLastName", icbcClaim.AdjusterLastName);
            Assert.AreEqual("adjusterMiddleName", icbcClaim.AdjusterMiddleName);
            Assert.AreEqual("adjusterOtherName", icbcClaim.AdjusterOtherName);
            Assert.AreEqual("adjusterPhoneNumber", icbcClaim.AdjusterPhoneNumber);
            Assert.AreEqual("adjusterPhoneNumber", icbcClaim.OriginalAdjusterPhoneNumber);
            Assert.AreEqual("adjusterPhoneExtension", icbcClaim.AdjusterPhoneNumberExt);
            Assert.AreEqual(new DateTimeOffset(new DateTime(2002, 2, 2)).ToString(), icbcClaim.BCDLExpiryDate);
            Assert.AreEqual("InsuranceClaimBCDLNumber", icbcClaim.BCDLNumber);
            Assert.AreEqual("BCDLStatus", icbcClaim.BCDLStatus);
            Assert.AreEqual("city", icbcClaim.City);
            Assert.AreEqual("claimCenterLocation", icbcClaim.ClaimCenterLocationCode);
            Assert.AreEqual("claimCenterCountry", icbcClaim.SupplierCountryCode);
            Assert.AreEqual("province", icbcClaim.SupplierCountrySubdivisionCode);
            Assert.AreEqual("InsuranceClaimPHNNumber", icbcClaim.PHNNumber);
            Assert.AreEqual("claimCenterPostalCode", icbcClaim.PostalCode);
            Assert.AreEqual("insurance notes", icbcClaim.Notes);
            Assert.AreEqual(1, icbcClaim.StatusCode);
            Assert.AreEqual(0, icbcClaim.StateCode);
            Assert.AreEqual(new DateTime(2019, 9, 1), icbcClaim.Date1);
            Assert.AreEqual(new DateTime(2020, 9, 1), icbcClaim.Date2);
            Assert.AreEqual("insurance Start Date", icbcClaim.Date1Label);
            Assert.AreEqual("insurance Expired Date", icbcClaim.Date2Label);
        }


        [Test]
        public void Phone_should_map_to_SSG_SimplePhoneNumber_correctly()
        {
            var phone = new Phone()
            {
                PhoneNumber = "claimCenterContactPhoneNumber1",
                Extension = "claimCenterContactPhoneExt1",
                Type = "Phone"
            };

            SimplePhoneNumberEntity assetPhone = _mapper.Map<SimplePhoneNumberEntity>(phone);
            Assert.AreEqual("claimCenterContactPhoneNumber1", assetPhone.PhoneNumber);
            Assert.AreEqual("claimCenterContactPhoneNumber1", assetPhone.OriginalPhoneNumber);
            Assert.AreEqual("claimCenterContactPhoneExt1", assetPhone.Extension);
            Assert.AreEqual("Phone", assetPhone.Type);
        }

        [Test]
        public void InsuranceClaim_without_BCDL_should_map_to_ICBCClaimEntity_correctly()
        {
            var claim = new InsuranceClaim()
            {
                ClaimCentre = new ClaimCentre()
                {
                    ContactAddress = null,
                    ContactNumber = null
                },
                Identifiers = new List<PersonalIdentifier>()
                {
                    new PersonalIdentifier()
                    {
                        Value="InsuranceClaimPHNNumber",
                        Type=PersonalIdentifierType.PersonalHealthNumber,
                        TypeCode="phn"
                    }
                },
                InsuredParties = null,
                ReferenceDates = null
            };
            ICBCClaimEntity icbcClaim = _mapper.Map<ICBCClaimEntity>(claim);
            Assert.AreEqual(null, icbcClaim.BCDLExpiryDate);
            Assert.AreEqual(null, icbcClaim.BCDLNumber);
            Assert.AreEqual(null, icbcClaim.BCDLStatus);
            Assert.AreEqual(null, icbcClaim.City);
            Assert.AreEqual(null, icbcClaim.ClaimCenterLocationCode);
            Assert.AreEqual(null, icbcClaim.SupplierCountryCode);
            Assert.AreEqual(null, icbcClaim.SupplierCountrySubdivisionCode);
            Assert.AreEqual("InsuranceClaimPHNNumber", icbcClaim.PHNNumber);
            Assert.AreEqual(null, icbcClaim.PostalCode);
            Assert.AreEqual(null, icbcClaim.Notes);
            Assert.AreEqual(1, icbcClaim.StatusCode);
            Assert.AreEqual(0, icbcClaim.StateCode);
            Assert.AreEqual(null, icbcClaim.Date1);
            Assert.AreEqual(null, icbcClaim.Date2);
            Assert.AreEqual(null, icbcClaim.Date1Label);
            Assert.AreEqual(null, icbcClaim.Date2Label);
        }

        [Test]
        public void InsuranceClaim_identifiers_null_should_map_to_ICBCClaimEntity_correctly()
        {
            var claim = new InsuranceClaim()
            {
                ClaimCentre = null,
                Identifiers = null,
                InsuredParties = null,
                ReferenceDates = null,
                Adjustor = null,
                AdjustorPhone = null
            };
            ICBCClaimEntity icbcClaim = _mapper.Map<ICBCClaimEntity>(claim);
            Assert.AreEqual(null, icbcClaim.BCDLExpiryDate);
            Assert.AreEqual(null, icbcClaim.BCDLNumber);
            Assert.AreEqual(null, icbcClaim.BCDLStatus);
            Assert.AreEqual(null, icbcClaim.City);
            Assert.AreEqual(null, icbcClaim.ClaimCenterLocationCode);
            Assert.AreEqual(null, icbcClaim.SupplierCountryCode);
            Assert.AreEqual(null, icbcClaim.SupplierCountrySubdivisionCode);
            Assert.AreEqual(null, icbcClaim.PHNNumber);
            Assert.AreEqual(null, icbcClaim.PostalCode);
            Assert.AreEqual(null, icbcClaim.Notes);
            Assert.AreEqual(1, icbcClaim.StatusCode);
            Assert.AreEqual(0, icbcClaim.StateCode);
            Assert.AreEqual(null, icbcClaim.Date1);
            Assert.AreEqual(null, icbcClaim.Date2);
            Assert.AreEqual(null, icbcClaim.Date1Label);
            Assert.AreEqual(null, icbcClaim.Date2Label);
            Assert.AreEqual(null, icbcClaim.AdjusterFirstName);
            Assert.AreEqual(null, icbcClaim.AdjusterLastName);
            Assert.AreEqual(null, icbcClaim.AdjusterMiddleName);
            Assert.AreEqual(null, icbcClaim.AdjusterOtherName);
            Assert.AreEqual(null, icbcClaim.AdjusterPhoneNumber);
            Assert.AreEqual(null, icbcClaim.AdjusterPhoneNumberExt);
        }

        [Test]
        public void InvolvedParty_should_map_to_SSG_InvolvedParty_correctly()
        {
            var party = new InvolvedParty()
            {
                Name = new Name() { FirstName = "InvolvedPartyFirstName", LastName = "InvolvedPartLastName", MiddleName = "InvolvedPartyMiddleName", OtherName = "InvolvedPartyOtherName" },
                Organization = "InvolvedPartyOrgName",
                Description = "InvolvedPartyDescription",
                Type = "InvolvedPartyTypeCode",
                Notes = "InvolvedPartyNotes",
                TypeDescription = "InvolvedPartyTypeDescription"
            };

            InvolvedPartyEntity ssg_InvolvedParty = _mapper.Map<InvolvedPartyEntity>(party);
            Assert.AreEqual("InvolvedPartyFirstName", ssg_InvolvedParty.FirstName);
            Assert.AreEqual("InvolvedPartLastName", ssg_InvolvedParty.LastName);
            Assert.AreEqual("InvolvedPartyMiddleName", ssg_InvolvedParty.MiddleName);
            Assert.AreEqual("InvolvedPartyOtherName", ssg_InvolvedParty.OtherName);
            Assert.AreEqual("InvolvedPartyNotes", ssg_InvolvedParty.Notes);
            Assert.AreEqual("InvolvedPartyOrgName", ssg_InvolvedParty.OrganizationName);
            Assert.AreEqual("InvolvedPartyTypeDescription", ssg_InvolvedParty.PartyDescription);
            Assert.AreEqual("InvolvedPartyTypeCode", ssg_InvolvedParty.PartyTypeCode);
        }

        [Test]
        public void InvolvedParty_with_null_name_should_map_to_SSG_InvolvedParty_correctly()
        {
            var party = new InvolvedParty()
            {
                Name = null,
                Notes = "InvolvedPartyNotes",
            };

            InvolvedPartyEntity ssg_InvolvedParty = _mapper.Map<InvolvedPartyEntity>(party);
            Assert.AreEqual(null, ssg_InvolvedParty.FirstName);
            Assert.AreEqual(null, ssg_InvolvedParty.LastName);
            Assert.AreEqual(null, ssg_InvolvedParty.MiddleName);
            Assert.AreEqual(null, ssg_InvolvedParty.OtherName);
            Assert.AreEqual("InvolvedPartyNotes", ssg_InvolvedParty.Notes);
            Assert.AreEqual(null, ssg_InvolvedParty.OrganizationName);
            Assert.AreEqual(null, ssg_InvolvedParty.PartyDescription);
            Assert.AreEqual(null, ssg_InvolvedParty.PartyTypeCode);
        }

        [Test]
        public void gender_null_RelatedPerson_should_map_to_SSG_Identity_correctly()
        {
            var relatedPerson = new RelatedPerson()
            {
                FirstName = "FirstName",
                Description = "test description",
                Notes = "notes",
                Gender = null,
            };
            RelatedPersonEntity ssg_relatedPerson = _mapper.Map<RelatedPersonEntity>(relatedPerson);
            Assert.AreEqual(null, ssg_relatedPerson.Gender);
        }


        [Test]
        public void SSG_SearchapiRequestDataProvider_should_map_to_DataProvider_correctly()
        {
            var dp = new SSG_SearchapiRequestDataProvider()
            {
                AdaptorName = "data provider 1"
            };
            DataProvider provider = _mapper.Map<DataProvider>(dp);
            Assert.AreEqual("DATAPROVIDER1", provider.Name);
        }


    }
}

