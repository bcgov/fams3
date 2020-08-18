using AutoMapper;
using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class AgencyRequestService_Update_Test
    {
        private AgencyRequestService _sut;
        private Mock<ILogger<AgencyRequestService>> _loggerMock;
        private Mock<ISearchRequestService> _searchRequestServiceMock;
        private Mock<IMapper> _mapper;

        private SearchRequestOrdered _searchRequstOrdered;
        private Guid _validRequestId;
        private IdentifierEntity _fakePersoneIdentifier;
        private AddressEntity _fakePersonAddress;
        private PhoneNumberEntity _fakePersonPhoneNumber;
        private AliasEntity _fakeName;
        private RelatedPersonEntity _fakeRelatedPerson;
        private EmploymentEntity _fakeEmployment;
        private EmploymentContactEntity _fakeEmploymentContact;
        private PersonEntity _ssg_fakePerson;
        private SearchRequestEntity _fakeSearchRequest;
        private Person _searchRequestPerson;

        [SetUp]
        public void Init()
        {
            _validRequestId = Guid.NewGuid();
            _loggerMock = new Mock<ILogger<AgencyRequestService>>();
            _searchRequestServiceMock = new Mock<ISearchRequestService>();
            _mapper = new Mock<IMapper>();

            _searchRequestPerson = new Person()
            {
                DateOfBirth = DateTime.Now,
                FirstName = "TEST1",
                LastName = "TEST2",
                Identifiers = new List<PersonalIdentifier>()
                {
                    new PersonalIdentifier()
                    {
                        Value = "test",
                        Type = PersonalIdentifierType.BCDriverLicense,
                        Owner = OwnerType.PersonSought
                    },
                    new PersonalIdentifier()
                    {
                        Value = "test2",
                        Type = PersonalIdentifierType.SocialInsuranceNumber,
                        Owner = OwnerType.PersonSought
                    }
                },
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AddressLine1 = "AddressLine1",
                        AddressLine2 = "AddressLine2",
                        City = "testCity",
                        Owner=OwnerType.PersonSought
                    }
                },
                Phones = new List<Phone>()
                {
                    new Phone()
                    {
                        PhoneNumber = "4005678900"
                    }
                },
                Names = new List<Name>()
                {
                    new Name()
                    {
                        FirstName = "firstName"
                    }
                },
                RelatedPersons = new List<RelatedPerson>()
                {
                    new RelatedPerson() { FirstName = "firstName" }
                },

                Employments = new List<Employment>()
                {
                    new Employment()
                    {
                        Occupation = "Occupation",
                        Employer = new Employer()
                        {
                            Phones = new List<Phone>() {
                                new Phone() { PhoneNumber = "1111111", Type = "Phone" }
                            }
                        }
                    }
                }

            };

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2010, 1, 1),
                SearchRequestKey = "key",
                Person = _searchRequestPerson

            };


            _fakePersoneIdentifier = new IdentifierEntity
            {
                Identification = "1234567",
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = _validRequestId
                }
            };
            _fakePersonAddress = new AddressEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = _validRequestId
                },
                AddressLine1 = "addressLine1"
            };
            _fakeEmployment = new EmploymentEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = _validRequestId
                }
            };

            _fakeEmploymentContact = new EmploymentContactEntity
            {
                PhoneNumber = "11111111"
            };

            _fakePersonPhoneNumber = new PhoneNumberEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = _validRequestId
                }
            };

            _fakeRelatedPerson = new RelatedPersonEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = _validRequestId
                },
                LastName = "relatedChild",
                PersonType = RelatedPersonPersonType.Relation.Value
            };

            _fakeName = new AliasEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = _validRequestId
                }
            };

            _fakeSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP",
                RequestPriority = RequestPriorityType.Rush.Value,
                ApplicantAddressLine1 = "new Address line 1",
                ApplicantAddressLine2 = "",
                PersonSoughtDateOfBirth = new DateTime(1998, 1, 1),
                LocationRequested = true,
                PHNRequested = true,
                DateOfDeathRequested = false
            };

            _ssg_fakePerson = new PersonEntity
            {
                FirstName = "firstName",
                LastName = "lastName"
            };

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(_ssg_fakePerson);

            _mapper.Setup(m => m.Map<EmploymentEntity>(It.IsAny<Employment>()))
              .Returns(_fakeEmployment);

            _mapper.Setup(m => m.Map<EmploymentContactEntity>(It.IsAny<Phone>()))
                .Returns(_fakeEmploymentContact);

            _mapper.Setup(m => m.Map<RelatedPersonEntity>(It.IsAny<RelatedPerson>()))
                   .Returns(_fakeRelatedPerson);

            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                    .Returns(_fakeSearchRequest);

            _searchRequestServiceMock.Setup(x => x.CreateSearchRequest(It.Is<SearchRequestEntity>(x => x.AgencyCode == "FMEP"), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = _validRequestId,
                    AgencyCode = "SUCCEED"
                }));

            _searchRequestServiceMock.Setup(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Identifier>(new SSG_Identifier()
                {
                }));

            _searchRequestServiceMock.Setup(x => x.CreateAddress(It.Is<AddressEntity>(x => x.SearchRequest.SearchRequestId == _validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Address>(new SSG_Address()
                {
                    AddressLine1 = "test full line"
                }));

            _searchRequestServiceMock.Setup(x => x.CreatePhoneNumber(It.Is<PhoneNumberEntity>(x => x.SearchRequest.SearchRequestId == _validRequestId), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_PhoneNumber>(new SSG_PhoneNumber()
              {
                  TelePhoneNumber = "4007678231"
              }));

            _searchRequestServiceMock.Setup(x => x.CreateName(It.Is<AliasEntity>(x => x.SearchRequest.SearchRequestId == _validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Aliase>(new SSG_Aliase()
                {
                    FirstName = "firstName"
                }));

            _searchRequestServiceMock.Setup(x => x.SavePerson(It.Is<PersonEntity>(x => x.SearchRequest.SearchRequestId == _validRequestId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
            {
                FirstName = "First"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateEmployment(It.Is<EmploymentEntity>(x => x.SearchRequest.SearchRequestId == _validRequestId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
            {
                EmploymentId = Guid.NewGuid(),
                Occupation = "Occupation"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_EmploymentContact>(new SSG_EmploymentContact()
            {
                PhoneNumber = "4007678231"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateRelatedPerson(It.Is<RelatedPersonEntity>(x => x.SearchRequest.SearchRequestId == _validRequestId), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_Identity>(new SSG_Identity()
              {
                  FirstName = "firstName"
              }));

            _sut = new AgencyRequestService(_searchRequestServiceMock.Object, _loggerMock.Object, _mapper.Object);

        }

        [Test]
        public async Task searchRequest_changed_ProcessUpdateSearchRequest_should_run_updateSearchRequest_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    RequestPriority = RequestPriorityType.Regular.Value,
                    ApplicantAddressLine1 = "old Address line1",
                    ApplicantAddressLine2 = "old Address line2",
                    PersonSoughtDateOfBirth = new DateTime(1999, 1, 1),
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    LocationRequested = true,
                    PHNRequested = false,
                    DateOfDeathRequested = true,
                    Notes = "note1",
                    SSG_Notes = new List<SSG_Notese>().ToArray(),
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName"
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP",
                RequestPriority = RequestPriorityType.Rush.Value,
                ApplicantAddressLine1 = "new Address line 1",
                ApplicantAddressLine2 = "",
                PersonSoughtDateOfBirth = new DateTime(1998, 1, 1),
                LocationRequested = true,
                PHNRequested = true,
                DateOfDeathRequested = false,
                Notes = "note1"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _searchRequestServiceMock.Setup(x => x.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                 {
                     SearchRequestId = guid
                 }));
            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",

                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.Is<SSG_SearchRequest>(
                m => (m.ApplicantAddressLine1 == "new Address line 1"
                && m.ApplicantAddressLine2 == "old Address line2"
                && m.RequestPriority == RequestPriorityType.Rush.Value
                && m.PersonSoughtDateOfBirth == new DateTime(1998, 1, 1)
                && m.LocationRequested
                && m.PHNRequested
                && m.DateOfDeathRequested == false
                && m.Notes == "note1")
                ), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<SSG_Notese>()
                , It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task personSought_changed_ProcessUpdateSearchRequest_should_run_updateSearchRequest_updatePerson_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtDateOfBirth = new DateTime(1999, 1, 1),
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName"
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP",
                PersonSoughtDateOfBirth = new DateTime(1999, 1, 1),
                PersonSoughtFirstName = "changedfirstName",
                PersonSoughtLastName = "lastName",
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _searchRequestServiceMock.Setup(x => x.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                 {
                     SearchRequestId = guid
                 }));

            _searchRequestServiceMock.Setup(x => x.UpdatePerson(It.IsAny<SSG_Person>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                 {
                     PersonId = personGuid
                 }));

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "changedfirstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "changedfirstName",
                    LastName = "lastName",

                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.Is<SSG_SearchRequest>(
                m => m.PersonSoughtFirstName == "changedfirstName"
                && m.PersonSoughtLastName == "lastName"),
                It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.Is<SSG_Person>(m => m.FirstName == "changedfirstName")
                        , It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<SSG_Notese>()
                , It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Notes_changed_ProcessUpdateSearchRequest_should_run_addNotes_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid noteGuid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    Notes = "note1",
                    SSG_Notes = new List<SSG_Notese>().ToArray(),
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName"
                }));

            _searchRequestServiceMock.Setup(x => x.CreateNotes(It.IsAny<NotesEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Notese>(new SSG_Notese()
                {
                    NotesId = noteGuid
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                Notes = "note2"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",

                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task RelatedPerson_changed_ProcessUpdateSearchRequest_should_run_updateRelatedPerson_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    Notes = "note1",
                    SSG_Notes = new List<SSG_Notese>().ToArray(),
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Identities = new List<SSG_Identity>() {
                        new SSG_Identity() {
                            FirstName = "child",
                            LastName = "lastName",
                            InformationSource = InformationSourceType.Request.Value,
                            PersonType = RelatedPersonPersonType.Relation.Value
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    RelatedPersons = new List<RelatedPerson>() { 
                        new RelatedPerson(){FirstName="newFirstName", LastName="lastName"}}
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
            , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task RelatedPerson_added_ProcessUpdateSearchRequest_should_run_addRelatedPerson_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    Notes = "note1",
                    SSG_Notes = new List<SSG_Notese>().ToArray(),
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Identities = new List<SSG_Identity>() {
                        new SSG_Identity() {
                            FirstName = "child",
                            LastName = "lastName",
                            InformationSource = InformationSourceType.ICBC.Value,
                            PersonType = RelatedPersonPersonType.Relation.Value
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };

            _searchRequestServiceMock.Setup(x => x.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Identity>(new SSG_Identity()
                {
                    RelatedPersonId = Guid.NewGuid()
                }));
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _mapper.Setup(m => m.Map<RelatedPersonEntity>(It.IsAny<RelatedPerson>()))
                .Returns(new RelatedPersonEntity
                {
                    FirstName = "newFirstName",
                    PersonType = RelatedPersonPersonType.Relation.Value
                });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    RelatedPersons = new List<RelatedPerson>() { 
                        new RelatedPerson(){FirstName="newFirstName", LastName="lastName"}}
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateRelatedPerson(It.Is<RelatedPersonEntity>(m=>m.FirstName=="newFirstName")
            , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Applicant_changed_ProcessUpdateSearchRequest_should_run_UpdateRelatedPerson_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    ApplicantFirstName = "applicantFirstName",
                    ApplicantLastName = "applicantLastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Identities = new List<SSG_Identity>() {
                        new SSG_Identity() {
                            FirstName = "applicantFirstName",
                            LastName = "applicantLastName",
                            InformationSource = InformationSourceType.Request.Value,
                            PersonType = RelatedPersonPersonType.Applicant.Value
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                ApplicantFirstName="changedFirstName",
                ApplicantLastName="changedApplicant"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Names = new List<Name>()
                    {
                        new Name(){FirstName="changedFirstName", LastName="changedApplicant", Owner=OwnerType.Applicant}
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
            , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Employment_changed_ProcessUpdateSearchRequest_should_run_updateEmployment_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Employments = new List<SSG_Employment>()
                    { 
                        new SSG_Employment()
                        {
                            BusinessName="existingBusinessName",
                            InformationSource = InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetEmployment(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
                {
                    SSG_EmploymentContacts = null
                }));


            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<EmploymentEntity>(It.IsAny<Employment>()))
                .Returns(new EmploymentEntity() { BusinessName="Changed"});

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Employments = new List<Employment>()
                    {
                        new Employment(){
                            Employer=new Employer(){ 
                                Name="newBizName",
                                Phones= new List<Phone>(){
                                    new Phone(){PhoneNumber="12345"}
                                }
                             },
                        }
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<SSG_Employment>()
                , It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Employment_added_ProcessUpdateSearchRequest_should_run_addEmployment_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",                   
                }));

            _searchRequestServiceMock.Setup(x => x.GetEmployment(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
                {
                    SSG_EmploymentContacts = null
                }));


            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<EmploymentEntity>(It.IsAny<Employment>()))
                .Returns(new EmploymentEntity() { BusinessName = "Changed" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Employments = new List<Employment>()
                    {
                        new Employment(){
                            Employer=new Employer(){
                                Name="newBizName",
                                Phones= new List<Phone>(){
                                    new Phone(){PhoneNumber="12345"}
                                }
                             },
                        }
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmployment(It.IsAny<EmploymentEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Addresses_changed_ProcessUpdateSearchRequest_should_run_addAddress_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Addresses = new List<SSG_Address>() {
                        new SSG_Address()
                        {
                            AddressLine1 = "addressLine1", AddressLine2="line2"
                        }
                    }.ToArray()
                })); 

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {};
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<AddressEntity>(It.IsAny<Employment>()))
                .Returns(new AddressEntity() { AddressLine1="addressLine1"});

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Addresses = new List<Address>()
                    {
                        new Address(){
                            AddressLine1="addressLine1",
                            Owner=OwnerType.PersonSought
                        }
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<SSG_Employment>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateAddress(It.IsAny<AddressEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Phones_changed_ProcessUpdateSearchRequest_should_run_addPhones_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_PhoneNumbers = new List<SSG_PhoneNumber>() {
                        new SSG_PhoneNumber()
                        {
                            TelePhoneNumber = "111111"
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            { };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PhoneNumberEntity>(It.IsAny<Phone>()))
                .Returns(new PhoneNumberEntity() { TelePhoneNumber = "111111" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Phones = new List<Phone>()
                    {
                        new Phone(){
                            PhoneNumber="2222222",
                            Owner=OwnerType.PersonSought
                        }
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<SSG_Employment>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateAddress(It.IsAny<AddressEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Identifiers_changed_ProcessUpdateSearchRequest_should_run_updateIdentifiers_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="222222",
                            IdentifierType = IdentificationType.BCDriverLicense.Value,
                            InformationSource = InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                .Returns(new IdentifierEntity() { Identification = "111111", IdentifierType= IdentificationType.BCDriverLicense.Value });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Identifiers = new List<PersonalIdentifier>()
                    {
                        new PersonalIdentifier(){
                            Owner=OwnerType.PersonSought,
                            Type=PersonalIdentifierType.BCDriverLicense,
                            Value="1111"
                        }
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<SSG_Employment>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateIdentifier(It.IsAny<SSG_Identifier>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Identifiers_added_ProcessUpdateSearchRequest_should_run_addIdentifiers_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid, FirstName = "firstName", LastName="lastName", InformationSource= InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="222222",
                            IdentifierType = IdentificationType.BirthCertificate.Value,
                            InformationSource = InformationSourceType.Request.Value
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                .Returns(new IdentifierEntity() { Identification = "111111", IdentifierType = IdentificationType.BCDriverLicense.Value });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Identifiers = new List<PersonalIdentifier>()
                    {
                        new PersonalIdentifier(){
                            Owner=OwnerType.PersonSought,
                            Type=PersonalIdentifierType.BCDriverLicense,
                            Value="1111"
                        }
                    }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<SSG_SearchRequest>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<SSG_Person>()
                        , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<SSG_Identity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<SSG_Employment>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateIdentifier(It.IsAny<IdentifierEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public void exception_searchRequestOrdered_ProcessUpdateSearchRequest_should_throw_exception()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("fakeException"));
            Assert.ThrowsAsync<Exception>(async () => await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered));
        }

        [Test]
        public async Task wrong_fileId_searchRequestOrdered_ProcessUpdateSearchRequest_should_return_null()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(null));

            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreEqual(null, ssgSearchRequest);
        }
    }
}
