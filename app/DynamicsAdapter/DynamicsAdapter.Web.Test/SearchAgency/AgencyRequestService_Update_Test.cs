using AutoMapper;
using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Exceptions;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Agency;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SafetyConcern;
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
        private Guid _validRequestGuid;
        private Guid _personGuid;

        [SetUp]
        public void Init()
        {
            _validRequestGuid = Guid.NewGuid();
            _personGuid = Guid.NewGuid();
            _loggerMock = new Mock<ILogger<AgencyRequestService>>();
            _searchRequestServiceMock = new Mock<ISearchRequestService>();
            _mapper = new Mock<IMapper>();

            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
              {
                  SearchRequestId = _validRequestGuid,
                  PersonSoughtFirstName = "firstName",
                  PersonSoughtLastName = "lastName",
                  CreatedByApi = true,
                  AgencyCode = "FMEP",
                  Agency = new SSG_Agency { AgencyCode = "FMEP" },
                  SendNotificationOnCreation = true,
                  SSG_Persons = new List<SSG_Person>()
                  {
                                new SSG_Person()
                                {
                                    PersonId=_personGuid,
                                    FirstName = "firstName",
                                    LastName="lastName",
                                    InformationSource= InformationSourceType.Request.Value,
                                    IsCreatedByAgency=true
                                }
                  }.ToArray()
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
                    Agency = new SSG_Agency { AgencyCode = "FMEP" },
                    PHNRequested = false,
                    CreatedByApi = true,
                    DateOfDeathRequested = true,
                    Notes = "note1",
                    SSG_Notes = new List<SSG_Notese>().ToArray(),
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid,
                            FirstName = "firstName",
                            LastName="lastName",
                            InformationSource= InformationSourceType.Request.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true
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

            _searchRequestServiceMock.Setup(x => x.UpdateSearchRequest(It.IsAny<Guid>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                 {
                     SearchRequestId = guid
                 }));
            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Agency = new Agency { Code = "FMEP" }

                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>()
                        , It.IsAny<IDictionary<string, object>>(), It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<SSG_Notese>()
                , It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task personSought_changed_ProcessUpdateSearchRequest_should_run_updateSearchRequest_updatePerson_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
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

            _searchRequestServiceMock.Setup(x => x.UpdateSearchRequest(It.IsAny<Guid>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                 {
                     SearchRequestId = _validRequestGuid
                 }));

            _searchRequestServiceMock.Setup(x => x.UpdatePerson(It.IsAny<Guid>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                 {
                     PersonId = _personGuid
                 }));

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "changedfirstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "changedfirstName",
                    LastName = "lastName",
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(
                It.IsAny<Guid>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                It.IsAny<IDictionary<string, object>>(),
                It.IsAny<PersonEntity>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<SSG_Notese>()
                , It.IsAny<CancellationToken>()), Times.Never);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Notes_changed_ProcessUpdateSearchRequest_should_run_addNotes_correctly()
        {
            Guid noteGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true
                }));

            _searchRequestServiceMock.Setup(x => x.CreateNotes(It.IsAny<NotesEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Notese>(new SSG_Notese()
                {
                    NotesId = noteGuid
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP",
                Notes = "note2"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task RelatedPerson_changed_ProcessUpdateSearchRequest_should_run_createRelatedPerson_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    SSG_Identities = new List<SSG_Identity>() {
                        new SSG_Identity() {
                            FirstName = "child",
                            LastName = "lastName",
                            InformationSource = InformationSourceType.Request.Value,
                            PersonType = RelatedPersonPersonType.Relation.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<SSG_Identity>(new SSG_Identity()
                    {
                        RelatedPersonId = Guid.NewGuid()
                    }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity() { AgencyCode = "FMEP" };

            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _mapper.Setup(m => m.Map<RelatedPersonEntity>(It.IsAny<RelatedPerson>()))
                   .Returns(new RelatedPersonEntity
                   {
                       LastName = "relatedChild",
                       PersonType = RelatedPersonPersonType.Relation.Value
                   });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Agency = new Agency { Code = "FMEP" },
                    RelatedPersons = new List<RelatedPerson>() {
                        new RelatedPerson(){FirstName="newFirstName", LastName="lastName"}}
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>()
            , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task RelatedPerson_added_ProcessUpdateSearchRequest_should_run_addRelatedPerson_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    SSG_Identities = new List<SSG_Identity>() {
                        new SSG_Identity() {
                            FirstName = "child",
                            LastName = "lastName",
                            InformationSource = InformationSourceType.ICBC.Value,
                            PersonType = RelatedPersonPersonType.Relation.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP"
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

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    RelatedPersons = new List<RelatedPerson>() {
                        new RelatedPerson(){FirstName="newFirstName", LastName="lastName"}},
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateRelatedPerson(It.Is<RelatedPersonEntity>(m => m.FirstName == "newFirstName" && m.UpdateDetails== "Create New Related Person")
            , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
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
                    CreatedByApi = true,
                    SendNotificationOnCreation = true,
                    Agency= new SSG_Agency { AgencyCode="FMEP" },
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid,
                            FirstName = "firstName",
                            LastName="lastName",
                            InformationSource= InformationSourceType.Request.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    SSG_Identities = new List<SSG_Identity>() {
                        new SSG_Identity() {
                            FirstName = "applicantFirstName",
                            LastName = "applicantLastName",
                            InformationSource = InformationSourceType.Request.Value,
                            PersonType = RelatedPersonPersonType.Applicant.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                ApplicantFirstName = "changedFirstName",
                ApplicantLastName = "changedApplicant"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                    },
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(
                        It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Employment_changed_ProcessUpdateSearchRequest_should_run_addEmployment_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId = guid,
                    PersonSoughtFirstName = "firstName",
                    PersonSoughtLastName = "lastName",
                    CreatedByApi = true,
                    AgencyCode = "FMEP",
                    Agency= new SSG_Agency {AgencyCode="FMEP"},
                    SendNotificationOnCreation = true,
                    SSG_Persons = new List<SSG_Person>()
                    {
                        new SSG_Person()
                        {
                            PersonId=personGuid,
                            FirstName = "firstName",
                            LastName="lastName",
                            InformationSource= InformationSourceType.Request.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    SSG_Employments = new List<SSG_Employment>()
                    {
                        new SSG_Employment()
                        {
                            BusinessName="existingBusinessName",
                            InformationSource = InformationSourceType.Request.Value,
                            IsCreatedByAgency=true
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
                .Returns(new EmploymentEntity() { BusinessName = "Changed" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            _mapper.Setup(m => m.Map<EmploymentContactEntity>(It.IsAny<Phone>()))
             .Returns(new EmploymentContactEntity { PhoneNumber = "11111111" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                    },
                    Agency = new Agency { Code = "FMEP" }
                }

            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmployment(It.IsAny<EmploymentEntity>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(guid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Employment_added_ProcessUpdateSearchRequest_should_run_addEmployment_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true
                }));

            _searchRequestServiceMock.Setup(x => x.GetEmployment(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
                {
                    SSG_EmploymentContacts = null
                }));


            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<EmploymentEntity>(It.IsAny<Employment>()))
                .Returns(new EmploymentEntity() { BusinessName = "Changed" });

            _mapper.Setup(m => m.Map<EmploymentContactEntity>(It.IsAny<Phone>()))
                .Returns(new EmploymentContactEntity { PhoneNumber = "11111111" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                    },
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmployment(It.IsAny<EmploymentEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Addresses_changed_ProcessUpdateSearchRequest_should_run_addAddress_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Addresses = new List<SSG_Address>() {
                        new SSG_Address()
                        {
                            AddressLine1 = "addressLine1", AddressLine2="line2", IsCreatedByAgency=true
                        }
                    }.ToArray(),
                    IsCreatedByAgency = true
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity { AgencyCode = "FMEP" };

            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<AddressEntity>(It.IsAny<Address>()))
                .Returns(new AddressEntity() { AddressLine1 = "addressLine1" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                    },
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateAddress(It.IsAny<AddressEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Phones_changed_ProcessUpdateSearchRequest_should_run_addPhones_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    SSG_PhoneNumbers = new List<SSG_PhoneNumber>() {
                        new SSG_PhoneNumber()
                        {
                            TelePhoneNumber = "111111"
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            { AgencyCode = "FMEP" };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PhoneNumberEntity>(It.IsAny<Phone>()))
                .Returns(new PhoneNumberEntity() { TelePhoneNumber = "111111" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                    },
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateAddress(It.IsAny<AddressEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Identifiers_changed_ProcessUpdateSearchRequest_should_run_createIdentifiers_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    SSG_Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="222222",
                            IdentifierType = IdentificationType.BCDriverLicense.Value,
                            InformationSource = InformationSourceType.Request.Value,
                            IsCreatedByAgency=true
                        }
                    }.ToArray(),
                    IsCreatedByAgency = true
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                .Returns(new IdentifierEntity() { Identification = "111111", IdentifierType = IdentificationType.BCDriverLicense.Value });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    FirstName = "firstName",
                    LastName = "lastName",
                    Agency = new Agency { Code = "FMEP" },
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
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateIdentifier(It.IsAny<IdentifierEntity>(),
                 It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Identifiers_added_ProcessUpdateSearchRequest_should_run_addIdentifiers_correctly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
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
                    }.ToArray(),
                    IsCreatedByAgency = true
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
                AgencyCode = "FMEP"
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                .Returns(new IdentifierEntity() { Identification = "111111", IdentifierType = IdentificationType.BCDriverLicense.Value });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                    },
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>()
                , It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateIdentifier(It.IsAny<IdentifierEntity>()
                , It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Names_changed_ProcessUpdateSearchRequest_should_run_addAliasCorrectly()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = _personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    SSG_Aliases = new List<SSG_Aliase>() {
                        new SSG_Aliase()
                        {
                            FirstName = "firstName", IsCreatedByAgency=true
                        }
                    }.ToArray()
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            { AgencyCode = "FMEP" };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<AliasEntity>(It.IsAny<Name>()))
                .Returns(new AliasEntity() { FirstName = "newfirstName" });

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
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
                        new Name(){
                            FirstName="newfirstName",
                            Owner=OwnerType.PersonSought
                        }
                    },
                    Agency = new Agency { Code = "FMEP" }
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSearchRequest(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.UpdatePerson(It.IsAny<Guid>(),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<PersonEntity>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateRelatedPerson(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.UpdateEmployment(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateName(It.IsAny<AliasEntity>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.AreEqual(_validRequestGuid, ssgSearchRequest.SearchRequestId);
        }

        [Test]
        public async Task Person_Caution_added_ProcessUpdateSearchRequest_should_run_CreateSafetyConcern_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                }));

            SearchRequestEntity newSearchRequest = new SearchRequestEntity()
            {
            };
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                .Returns(newSearchRequest);

            _mapper.Setup(m => m.Map<PersonEntity>(It.IsAny<Person>()))
               .Returns(new PersonEntity() { FirstName = "firstName", LastName = "lastName"});

            _mapper.Setup(m => m.Map<SafetyConcernEntity>(It.IsAny<Person>()))
                .Returns(new SafetyConcernEntity {Detail= "flag" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    Agency=new Agency { Code="FMEP"},
                    CautionFlag="flag"                 
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.CreateSafetyConcern(It.IsAny<SafetyConcernEntity>(),It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        public async Task Person_Caution_changed_ProcessUpdateSearchRequest_should_run_UpdateSafetyConcern_correctly()
        {
            Guid guid = Guid.NewGuid();
            Guid personGuid = Guid.NewGuid();

            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                {
                    PersonId = personGuid,
                    LastName = "lastName",
                    FirstName = "firstName",
                    IsCreatedByAgency = true,
                    sSG_SafetyConcernDetails= new List<SSG_SafetyConcernDetail>() {
                        new SSG_SafetyConcernDetail()
                        {
                            Detail = "originalDetail", IsCreatedByAgency=true
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

            _mapper.Setup(m => m.Map<SafetyConcernEntity>(It.IsAny<Person>()))
                .Returns(new SafetyConcernEntity { Detail = "flag" });

            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.UPDATE,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                Person = new Person()
                {
                    Agency = new Agency { Code = "FMEP" },
                    CautionFlag = "flag"
                }
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.UpdateSafetyConcern(It.IsAny<Guid>(),
                 It.IsAny<IDictionary<string, object>>(),
                 It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void exception_searchRequestOrdered_ProcessUpdateSearchRequest_should_throw_exception()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("fakeException"));
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered();
            Assert.ThrowsAsync<Exception>(async () => await _sut.ProcessUpdateSearchRequest(searchRequestOrdered));
        }

        [Test]
        public void closed_searchRequest_ProcessUpdateSearchRequest_should_throw_exception()
        {
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                    {
                        SearchRequestId = _validRequestGuid,
                        StatusCode = 2
                    }));

            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered();
            Assert.ThrowsAsync<AgencyRequestException>(async () => await _sut.ProcessUpdateSearchRequest(searchRequestOrdered));
        }

        [Test]
        public void cancelled_searchRequest_ProcessUpdateSearchRequest_should_throw_exception()
        {
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                    {
                        SearchRequestId = _validRequestGuid,
                        StatusCode = 867670009
                    }));

            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered();
            Assert.ThrowsAsync<AgencyRequestException>(async () => await _sut.ProcessUpdateSearchRequest(searchRequestOrdered));
        }

        [Test]
        public async Task wrong_fileId_searchRequestOrdered_ProcessUpdateSearchRequest_should_return_null()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(null));
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered();
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequestOrdered);
            _searchRequestServiceMock.Verify(m => m.CreateNotes(It.IsAny<NotesEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreEqual(null, ssgSearchRequest);
        }

        [Test]
        public void wrong_agencyCode_searchRequestOrdered_ProcessUpdateSearchRequest_should_return_throwException()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                    {
                        PersonId = _personGuid,
                        IsCreatedByAgency = true
                    }));
            Guid guid = Guid.NewGuid();
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered { Person = new Person { Agency = new Agency { Code = "TEST" } } };
            Assert.ThrowsAsync<AgencyRequestException>(async()=>await _sut.ProcessUpdateSearchRequest(searchRequestOrdered));
        }

        [Test]
        public async Task null_searchRequestOrdered_ProcessUpdateSearchRequest_should_return_null()
        {
            _searchRequestServiceMock.Setup(x => x.GetPerson(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
                    {
                        PersonId = _personGuid,
                        IsCreatedByAgency = true
                    }));
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
               .Returns((SearchRequestEntity)(null));

            Guid guid = Guid.NewGuid();
            SearchRequestOrdered searchRequestOrdered = new SearchRequestOrdered { Person = new Person { Agency = new Agency { Code = "FMEP" } } };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessUpdateSearchRequest(searchRequestOrdered);
            Assert.AreEqual(null, ssgSearchRequest);
        }
    }
}
