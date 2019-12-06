using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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

namespace DynamicsAdapter.Web.Test.PersonSearch
{
    public class PersonSearchControllerTest
    {

        private Guid _testGuid;
        private Guid _exceptionGuid;

        private PersonSearchController _sut;
        private Mock<ILogger<PersonSearchController>> _loggerMock ;
        private Mock<ISearchRequestService> _searchRequestServiceMock;
        private Mock<ISearchApiRequestService> _searchApiRequestServiceMock;
        private PersonSearchCompleted _fakePersonCompletedEvent;
        private PersonSearchAccepted fakePersonAcceptedEvent;
        private PersonSearchFailed fakePersonFailedEvent;
        private PersonSearchRejected fakePersonRejectEvent;
        private SSG_SearchApiEvent fakeSearchApiEvent;
        private SSG_Identifier _fakePersoneIdentifier;
        private SSG_Address _fakePersonAddress;

        private Mock<IMapper> _mapper;

        [SetUp]
        public void Init()
        {

            _testGuid = Guid.NewGuid();
            _exceptionGuid = Guid.NewGuid();
            _loggerMock = new Mock<ILogger<PersonSearchController>>();
            _searchApiRequestServiceMock = new Mock<ISearchApiRequestService>();
            _searchRequestServiceMock = new Mock<ISearchRequestService>();
            _mapper = new Mock<IMapper>();
            var validRequestId = Guid.NewGuid();
            var invalidRequestId = Guid.NewGuid();

            fakeSearchApiEvent = new SSG_SearchApiEvent { };

            _fakePersoneIdentifier = new SSG_Identifier {
                SSG_SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakePersonAddress = new SSG_Address
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            fakePersonAcceptedEvent = new PersonSearchAccepted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                }
            };

            _fakePersonCompletedEvent = new PersonSearchCompleted()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                MatchedPerson = new Person()
                {
                    DateOfBirth = DateTime.Now,
                    FirstName = "TEST1",
                    LastName = "TEST2",
                    Identifiers = new List<PersonalIdentifier>()
                        {
                            new PersonalIdentifier()
                            {
                               SerialNumber  = "test",
                               IssuedBy = "test",
                               Type = PersonalIdentifierType.DriverLicense
                            }
                        },
                    Addresses = new List<Address>()
                        {
                            new Address()
                            {
                                AddressLine1 = "AddressLine1",
                                AddressLine2 = "AddressLine2",
                                Province = "Manitoba",
                                City = "testCity",
                                Type = "residence",
                                Country="canada",
                                PostalCode = "p3p3p3",
                                SuppliedBy = "Employer"
                            }
                        }
                }
            };

            fakePersonFailedEvent = new PersonSearchFailed()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                Cause = "Unable to proceed"
            };

            fakePersonRejectEvent = new PersonSearchRejected()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                Reasons = new List<ValidationResult> { }
            };

            
            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchAccepted>()))
                                .Returns(fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchRejected>()))
                               .Returns(fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchFailed>()))
                               .Returns(fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchCompleted>()))
                               .Returns(fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_Identifier>(It.IsAny<PersonalIdentifier>()))
                               .Returns(_fakePersoneIdentifier);

            _mapper.Setup(m => m.Map<SSG_Address>(It.IsAny<Address>()))
                   .Returns(_fakePersonAddress);

            _searchApiRequestServiceMock.Setup(x => x.GetLinkedSearchRequestIdAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Guid>(_testGuid));

            _searchApiRequestServiceMock.Setup(x => x.GetLinkedSearchRequestIdAsync(It.Is<Guid>(x => x == _exceptionGuid), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Guid>(invalidRequestId));


            _searchRequestServiceMock.Setup(x => x.UploadIdentifier(It.Is<SSG_Identifier>(x => x.SSG_SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Identifier>(new SSG_Identifier()
                {
                    Identification = "test identification"
                }));

            _searchRequestServiceMock.Setup(x => x.UploadIdentifier(It.Is<SSG_Identifier>(x => x.SSG_SearchRequest.SearchRequestId == invalidRequestId), It.IsAny<CancellationToken>()))
                .Throws(new Exception("random exception"));

            _searchRequestServiceMock.Setup(x => x.UploadAddress(It.Is<SSG_Address>(x => x.SearchRequest.SearchRequestId == validRequestId), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_Address>(new SSG_Address()
                {
                    FullText = "test full line"
                }));

            _searchApiRequestServiceMock.Setup(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid),
                    It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchApiEvent>(new SSG_SearchApiEvent()
                {
                    Id = _testGuid,
                    Name = "Random Event"
                }));

            _searchApiRequestServiceMock.Setup(x => x.AddEventAsync(It.Is<Guid>(x => x == _exceptionGuid),
                    It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception("random exception"));

            _sut = new PersonSearchController(_searchRequestServiceMock.Object, _searchApiRequestServiceMock.Object, _loggerMock.Object,_mapper.Object);

        }

        [Test]
        public async Task With_valid_completed_event_it_should_return_ok()
        {
            var result = await _sut.Completed(_testGuid, _fakePersonCompletedEvent);
            _searchRequestServiceMock
                .Verify(x => x.UploadIdentifier(It.IsAny<SSG_Identifier>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchRequestServiceMock
                .Verify(x => x.UploadAddress(It.IsAny<SSG_Address>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task With_exception_completed_event_should_return_badrequest()
        {
            var result = await _sut.Completed(_exceptionGuid, _fakePersonCompletedEvent);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task With_valid_accepted_event_it_should_return_ok()
        {
            var result = await _sut.Accepted(_testGuid, fakePersonAcceptedEvent);


            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_event_it_should_return_badrequest()
        {
            var result = await _sut.Accepted(_exceptionGuid, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }


        [Test]
        public async Task With_valid_failed_event_it_should_return_ok()
        {
            var result = await _sut.Failed(_testGuid, fakePersonFailedEvent);


            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_failed_event_it_should_return_badrequest()
        {
            var result = await _sut.Failed(_exceptionGuid, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task With_valid_rejected_event_it_should_return_ok()
        {
            var result = await _sut.Rejected(_testGuid, fakePersonRejectEvent
                );


            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_rejected_event_it_should_return_badrequest()
        {
            var result = await _sut.Rejected(_exceptionGuid, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

    }
}
