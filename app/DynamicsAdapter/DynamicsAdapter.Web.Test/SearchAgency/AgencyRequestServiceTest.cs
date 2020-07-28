using AutoMapper;
using DynamicsAdapter.Web.SearchAgency;
using DynamicsAdapter.Web.SearchAgency.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
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
        private CancellationToken _fakeToken;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<AgencyRequestService>>();
            _searchRequestServiceMock = new Mock<ISearchRequestService>();
            _mapper = new Mock<IMapper>();

            Person searchRequestPerson = new Person()
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
                Person = searchRequestPerson
            };


            _fakeToken = new CancellationToken();

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
            _mapper.Setup(m => m.Map<ntEntity>(It.IsAny<Employment>()))
              .Returns(_fakeEmployment);

            _mapper.Setup(m => m.Map<EmploymentContactEntity>(It.IsAny<Phone>()))
                .Returns(_fakeEmploymentContact);

            _mapper.Setup(m => m.Map<RelatedPersonEntity>(It.IsAny<RelatedPerson>()))
                   .Returns(_fakeRelatedPerson);











            _searchRequestServiceMock.Setup(x => x.CreateIdentifier(It.IsAny<IdentifierEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Identifier>(new SSG_Identifier()
                {
                }));

            _searchRequestServiceMock.Setup(x => x.CreateAddress(It.Is<AddressEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Address>(new SSG_Address()
                {
                    AddressLine1 = "test full line"
                }));

            _searchRequestServiceMock.Setup(x => x.CreatePhoneNumber(It.Is<PhoneNumberEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_PhoneNumber>(new SSG_PhoneNumber()
              {
                  TelePhoneNumber = "4007678231"
              }));

            _searchRequestServiceMock.Setup(x => x.CreateName(It.Is<AliasEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Aliase>(new SSG_Aliase()
                {
                    FirstName = "firstName"
                }));

            _searchRequestServiceMock.Setup(x => x.SavePerson(It.Is<PersonEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<SSG_Person>(new SSG_Person()
            {
                FirstName = "First"
            }));

            _searchRequestServiceMock.Setup(x => x.CreateEmployment(It.Is<EmploymentEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
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

            _searchRequestServiceMock.Setup(x => x.CreateRelatedPerson(It.Is<RelatedPersonEntity>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_Identity>(new SSG_Identity()
              {
                  FirstName = "firstName"
              }));

            _searchRequestServiceMock.Setup(x => x.CreateEmployment(It.Is<EmploymentEntity>(x => x.BusinessName == COMPENSATION_BUISNESS_NAME && x.Date1 == new DateTime(2019, 9, 1)), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
                {
                    EmploymentId = Guid.NewGuid(),

                }));
            _sut = new AgencyRequestService(_searchRequestServiceMock.Object, _loggerMock.Object, _mapper.Object);

        }

    }
}
