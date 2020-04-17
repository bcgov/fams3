using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
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
        private Mock<ISearchResultService> _searchResultServiceMock;
        private Mock<ISearchApiRequestService> _searchApiRequestServiceMock;
        private PersonSearchCompleted _fakePersonCompletedEvent;
        private PersonSearchAccepted fakePersonAcceptedEvent;
        private PersonSearchFailed fakePersonFailedEvent;
        private PersonSearchRejected fakePersonRejectEvent;
        private SSG_SearchApiEvent fakeSearchApiEvent;
        private SSG_Identifier _fakePersoneIdentifier;
        private SSG_Address _fakePersonAddress;
        private SSG_PhoneNumber _fakePersonPhoneNumber;
        private SSG_Aliase _fakeName;
        private SSG_Person _fakePerson;

        private Mock<IMapper> _mapper;

        [SetUp]
        public void Init()
        {
            _testGuid = Guid.NewGuid();
            _exceptionGuid = Guid.NewGuid();
            _loggerMock = new Mock<ILogger<PersonSearchController>>();
            _searchApiRequestServiceMock = new Mock<ISearchApiRequestService>();
            _searchResultServiceMock = new Mock<ISearchResultService>();
            _mapper = new Mock<IMapper>();
            var validRequestId = Guid.NewGuid();
            var invalidRequestId = Guid.NewGuid();

            fakeSearchApiEvent = new SSG_SearchApiEvent { };

            _fakePersoneIdentifier = new SSG_Identifier {
                SearchRequest = new SSG_SearchRequest
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

            _fakePersonPhoneNumber = new SSG_PhoneNumber
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakeName = new SSG_Aliase
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakePerson = new SSG_Person
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
                MatchedPersons = new List<Person>()
                {
                    new Person(){
                        DateOfBirth = DateTime.Now,
                        FirstName = "TEST1",
                        LastName = "TEST2",
                        Identifiers = new List<PersonalIdentifier>() { },
                        Addresses = new List<Address>(){ },
                        Phones = new List<Phone>(){ },
                        Names = new List<Name>(){ }
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

            _mapper.Setup(m => m.Map<SSG_PhoneNumber>(It.IsAny<Phone>()))
                             .Returns(_fakePersonPhoneNumber);

            _mapper.Setup(m => m.Map<SSG_Address>(It.IsAny<Address>()))
                              .Returns(_fakePersonAddress);

            _mapper.Setup(m => m.Map<SSG_Aliase>(It.IsAny<Name>()))
                  .Returns(_fakeName);

            _mapper.Setup(m => m.Map<SSG_Person>(It.IsAny<Person>()))
               .Returns(_fakePerson);


            _searchApiRequestServiceMock.Setup(x => x.GetLinkedSearchRequestIdAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Guid>(_testGuid));

            _searchApiRequestServiceMock.Setup(x => x.GetLinkedSearchRequestIdAsync(It.Is<Guid>(x => x == _exceptionGuid), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<Guid>(invalidRequestId));


            _searchApiRequestServiceMock.Setup(x => x.MarkComplete(It.Is<Guid>(x => x == _testGuid), It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest()
               {
                  SearchApiRequestId = _testGuid,
                   Name = "Random Event"
               }));

            _searchResultServiceMock.Setup(x => x.ProcessPersonFound(It.Is<Person>(x => x.FirstName == "TEST1"),It.IsAny<ProviderProfile>(), It.IsAny<SSG_SearchRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<bool>(true));

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

            _sut = new PersonSearchController(_searchResultServiceMock.Object, _searchApiRequestServiceMock.Object, _loggerMock.Object,_mapper.Object);

        }

        [Test]
        public async Task With_valid_completed_event_it_should_return_ok()
        {

            var result = await _sut.Completed(_testGuid, _fakePersonCompletedEvent);

            _searchResultServiceMock.Verify(x => x.ProcessPersonFound(It.Is<Person>(x => x.FirstName == "TEST1"), It.IsAny<ProviderProfile>(), It.IsAny<SSG_SearchRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task With_valid_finalized_event_it_should_return_ok()
        {
            var result = await _sut.Finalized(_testGuid);
           _searchApiRequestServiceMock
                .Verify(x => x.MarkComplete(It.Is<Guid>(x => x == _testGuid), It.IsAny<CancellationToken>()), Times.Once);
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
