using AutoMapper;
using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Exceptions;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class AgencyRequestServiceTest
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
        private SafetyConcernEntity _fakeSafety;
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
                CautionFlag="cautionFlag",
                CautionReason="violence",
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
                        FirstName = "firstName",
                        Owner=OwnerType.PersonSought,
                        Identifiers= new List<PersonalIdentifier>{
                            new PersonalIdentifier{Value="123222", Type= PersonalIdentifierType.BCDriverLicense}
                        },
                        Addresses = new List<Address>{
                            new Address{AddressLine1="line1"}
                        },
                        Phones = new List<Phone>
                        {
                            new Phone{PhoneNumber="12343"}
                        }
                    },
                    new Name()
                    {
                        FirstName = "applicantFirstName",
                        Owner=OwnerType.Applicant
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
                },
                Agency = new Agency { Code="FMEP"}

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
                }
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

            _fakeSafety = new SafetyConcernEntity { Detail = "safety" };

            _ssg_fakePerson = new PersonEntity
            {
            };

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                               .Returns(_fakePersoneIdentifier);

            _mapper.Setup(m => m.Map<PhoneNumberEntity>(It.IsAny<Phone>()))
                             .Returns(_fakePersonPhoneNumber);

            _mapper.Setup(m => m.Map<AddressEntity>(It.IsAny<Address>()))
                              .Returns(_fakePersonAddress);

            _mapper.Setup(m => m.Map<AliasEntity>(It.IsAny<Name>()))
                  .Returns(_fakeName);

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

            _mapper.Setup(m => m.Map<SafetyConcernEntity>(It.IsAny<Person>()))
                    .Returns(_fakeSafety);

            _searchRequestServiceMock.Setup(x => x.CreateSearchRequest(It.Is<SearchRequestEntity>(x => x.AgencyCode == "FMEP"), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    SearchRequestId= _validRequestId,
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

            _searchRequestServiceMock.Setup(x => x.SubmitToQueue(It.IsAny<Guid>()))
              .Returns(Task.FromResult<bool>(true));
            _sut = new AgencyRequestService(_searchRequestServiceMock.Object, _loggerMock.Object, _mapper.Object);

        }

        [Test]
        public async Task normal_searchRequestOrdered_ProcessSearchRequestOrdered_should_succeed()
        {
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessSearchRequestOrdered(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.CreateSearchRequest(It.IsAny<SearchRequestEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
            _searchRequestServiceMock.Verify(m => m.CreateAddress(It.IsAny<AddressEntity>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _searchRequestServiceMock.Verify(m => m.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateEmployment(It.IsAny<EmploymentEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateName(It.IsAny<AliasEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateSafetyConcern(It.IsAny<SafetyConcernEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.SubmitToQueue(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task null_identifiers_addresses_phones_employments_relatedPersons_searchRequestOrdered_ProcessSearchRequestOrdered_should_succeed()
        {
            Person nullPerson = new Person()
            {
                DateOfBirth = DateTime.Now,
                FirstName = "TEST1",
                LastName = "TEST2",
                Identifiers = null,
                Addresses = null,
                Phones = null,
                Names = null,
                RelatedPersons = null,
                Employments = null
            };
            SearchRequestOrdered searchRequstOrdered = new SearchRequestOrdered()
            {
                Action = RequestAction.NEW,
                RequestId = "1111111",
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = new DateTime(2010, 1, 1),
                SearchRequestKey = "key",
                Person = nullPerson
            };
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessSearchRequestOrdered(searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.CreateSearchRequest(It.IsAny<SearchRequestEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.SavePerson(It.IsAny<PersonEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock.Verify(m => m.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateAddress(It.IsAny<AddressEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreatePhoneNumber(It.IsAny<PhoneNumberEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmployment(It.IsAny<EmploymentEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateEmploymentContact(It.IsAny<EmploymentContactEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateRelatedPerson(It.IsAny<RelatedPersonEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateName(It.IsAny<AliasEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _searchRequestServiceMock.Verify(m => m.CreateSafetyConcern(It.IsAny<SafetyConcernEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void exception_searchRequestOrdered_ProcessSearchRequestOrdered_should_throw_exception()
        {
            _mapper.Setup(m => m.Map<SearchRequestEntity>(It.IsAny<SearchRequestOrdered>()))
                    .Throws(new Exception("fakeException"));
            Assert.ThrowsAsync<Exception>(async () => await _sut.ProcessSearchRequestOrdered(_searchRequstOrdered));
        }

        [Test]
        public async Task normal_searchRequestOrdered_ProcessCancelSearchRequest_should_succeed()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
            {
                SearchRequestId = guid,
                Agency = new Fams3Adapter.Dynamics.Agency.SSG_Agency { AgencyCode = "FMEP" }
            }));

            _searchRequestServiceMock.Setup(x => x.CancelSearchRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
                {
                    FileId = "fileId"
                }));
            SSG_SearchRequest ssgSearchRequest = await _sut.ProcessCancelSearchRequest(_searchRequstOrdered);
            _searchRequestServiceMock.Verify(m => m.CancelSearchRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void exception_searchRequestOrdered_ProcessCancelSearchRequest_should_throw_exception()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
            {
                SearchRequestId = guid,
                Agency = new Fams3Adapter.Dynamics.Agency.SSG_Agency { AgencyCode = "FMEP" }
            }));

            _searchRequestServiceMock.Setup(x => x.CancelSearchRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("fakeException"));
            Assert.ThrowsAsync<Exception>(async () => await _sut.ProcessCancelSearchRequest(_searchRequstOrdered));
        }

        [Test]
        public void different_agencyCode_searchRequestOrdered_ProcessCancelSearchRequest_should_throw_exception()
        {
            Guid guid = Guid.NewGuid();
            _searchRequestServiceMock.Setup(x => x.GetSearchRequest(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest()
            {
                SearchRequestId = guid,
                Agency = new Fams3Adapter.Dynamics.Agency.SSG_Agency { AgencyCode = "FMEP2" }
            }));

            Assert.ThrowsAsync<AgencyRequestException>(async () => await _sut.ProcessCancelSearchRequest(_searchRequstOrdered));
        }
    }
}
