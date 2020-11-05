using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DynamicsAdapter.Web.PersonSearch;
using DynamicsAdapter.Web.PersonSearch.Models;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.DataProvider;
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
        private string _searchRequestKey;
        private string _searchRequestKeySearchNotComplete;
        private string _exceptionSearchRequestKey;

        private PersonSearchController _sut;
        private Mock<ILogger<PersonSearchController>> _loggerMock ;
        private Mock<ISearchResultService> _searchResultServiceMock;
        private Mock<ISearchApiRequestService> _searchApiRequestServiceMock;
        private Mock<IDataPartnerService> _dataPartnerServiceMock; 
        private Mock<ISearchRequestRegister> _registerMock;
        private PersonSearchCompleted _fakePersonCompletedEvent;
        private PersonSearchAccepted _fakePersonAcceptedEvent;
        private PersonSearchFailed _fakePersonFailedEvent;
        private PersonSearchRejected _fakePersonRejectEvent;
        private PersonSearchSubmitted _fakePersonSubmittedEvent;
        private PersonSearchFinalized _fakePersonFinalizedEvent;
        private SSG_SearchApiEvent _fakeSearchApiEvent;
        private IdentifierEntity _fakePersoneIdentifier;
        private AddressEntity _fakePersonAddress;
        private PhoneNumberEntity _fakePersonPhoneNumber;
        private AliasEntity _fakeName;
        private SSG_Person _fakePerson;
        private SSG_Identifier _fakeSourceIdentifier;
        private SSG_SearchApiRequest _fakeSearchApiRequest;

        private Mock<IMapper> _mapper;

        [SetUp]
        public void Init()
        {
            _testGuid = Guid.NewGuid();
            _searchRequestKey = "fileId_SequenceNumber";
            _searchRequestKeySearchNotComplete = "fileId_SequenceNumber_NotComplete";
            _exceptionSearchRequestKey = "exception_seqNum";
            _exceptionGuid = Guid.NewGuid();
            _loggerMock = new Mock<ILogger<PersonSearchController>>();
            _searchApiRequestServiceMock = new Mock<ISearchApiRequestService>();
            _searchResultServiceMock = new Mock<ISearchResultService>();
            _dataPartnerServiceMock = new Mock<IDataPartnerService>();
            _mapper = new Mock<IMapper>();
            _registerMock = new Mock<ISearchRequestRegister>();

            _fakeSearchApiRequest = new SSG_SearchApiRequest()
            {
                SearchApiRequestId = _testGuid
            };

            var validRequestId = Guid.NewGuid();
            var invalidRequestId = Guid.NewGuid();

            _fakeSearchApiEvent = new SSG_SearchApiEvent { };

            _fakePersoneIdentifier = new IdentifierEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };


            _fakePersonAddress = new AddressEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakePersonPhoneNumber = new PhoneNumberEntity
            {
                SearchRequest = new SSG_SearchRequest
                {
                    SearchRequestId = validRequestId
                }
            };

            _fakeName = new AliasEntity
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

            _fakeSourceIdentifier = new SSG_Identifier()
            {
                IdentifierId = Guid.NewGuid()
            };

            _fakePersonAcceptedEvent = new PersonSearchAccepted()
            {
                SearchRequestId = Guid.NewGuid(),
                SearchRequestKey = _searchRequestKey,
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                }
            };

            _fakePersonCompletedEvent = new PersonSearchCompleted()
            {
                SearchRequestId = Guid.NewGuid(),
                SearchRequestKey= _searchRequestKey,
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                MatchedPersons = new List<PersonFound>()
                {
                    new PersonFound(){
                        DateOfBirth = DateTime.Now,
                        FirstName = "TEST1",
                        LastName = "TEST2",
                        Identifiers = new List<PersonalIdentifier>() { },
                        Addresses = new List<Address>(){ },
                        Phones = new List<Phone>(){ },
                        Names = new List<Name>(){ },
                        SourcePersonalIdentifier = new PersonalIdentifier()
                            {
                                Value = "1234567"
                            }
                    }
                }
            };

            _fakePersonFailedEvent = new PersonSearchFailed()
            {
                SearchRequestId = Guid.NewGuid(),
                SearchRequestKey = _searchRequestKey,
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                Cause = "Unable to proceed"
            };


            _fakePersonRejectEvent = new PersonSearchRejected()
            {
                SearchRequestId = Guid.NewGuid(),
                SearchRequestKey = _searchRequestKey,
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                Reasons = new List<ValidationResult> { }
            };

            _fakePersonFinalizedEvent = new PersonSearchFinalized()
            {
                SearchRequestId = Guid.NewGuid(),
                SearchRequestKey = _searchRequestKey,
                TimeStamp = DateTime.Now,
                Message = "test message"
            };

            _fakePersonSubmittedEvent = new PersonSearchSubmitted()
            {
                SearchRequestId = Guid.NewGuid(),
                SearchRequestKey = _searchRequestKey,
                TimeStamp = DateTime.Now,
                ProviderProfile = new ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                },
                Message = "the search api request has been submitted to the Data provider."
            };

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchAccepted>()))
                                .Returns(_fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchRejected>()))
                               .Returns(_fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchFailed>()))
                               .Returns(_fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchCompleted>()))
                               .Returns(_fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<SSG_SearchApiEvent>(It.IsAny<PersonSearchSubmitted>()))
                                .Returns(_fakeSearchApiEvent);

            _mapper.Setup(m => m.Map<IdentifierEntity>(It.IsAny<PersonalIdentifier>()))
                               .Returns(_fakePersoneIdentifier);

            _mapper.Setup(m => m.Map<PhoneNumberEntity>(It.IsAny<Phone>()))
                             .Returns(_fakePersonPhoneNumber);

            _mapper.Setup(m => m.Map<AddressEntity>(It.IsAny<Address>()))
                              .Returns(_fakePersonAddress);

            _mapper.Setup(m => m.Map<AliasEntity>(It.IsAny<Name>()))
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
                  SequenceNumber="1234567"
               }));

            _dataPartnerServiceMock
                .Setup(x => x.GetSearchApiRequestDataProvider(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SearchapiRequestDataProvider { AdaptorName = "ICBC", SearchAPIRequestId = _testGuid, NumberOfFailures = 0, TimeBetweenRetries = 10 }));


            _dataPartnerServiceMock
                .Setup(x => x.UpdateSearchRequestApiProvider(It.IsAny<SSG_SearchapiRequestDataProvider>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SearchapiRequestDataProvider { AdaptorName = "ICBC", SearchAPIRequestId = _testGuid, NumberOfFailures = 0, TimeBetweenRetries = 10 }));

            _searchResultServiceMock.Setup(x => x.ProcessPersonFound(It.Is<Person>(x => x.FirstName == "TEST1"),It.IsAny<ProviderProfile>(), It.IsAny<SSG_SearchRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<SSG_Identifier>()))
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

            _registerMock.Setup(x => x.GetSearchApiRequest(It.Is<string>(m => m == _searchRequestKey)))
                .Returns(Task.FromResult(_fakeSearchApiRequest));
            _registerMock.Setup(x => x.GetSearchApiRequest(It.Is<string>(m => m == _searchRequestKeySearchNotComplete)))
                .Returns(Task.FromResult(_fakeSearchApiRequest));

            _registerMock.Setup(x => x.DataPartnerSearchIsComplete(It.Is<string>(m => m == _searchRequestKey)))
            .Returns(Task.FromResult(true));

            _registerMock.Setup(x => x.DataPartnerSearchIsComplete(It.Is<string>(m => m == _searchRequestKeySearchNotComplete)))
           .Returns(Task.FromResult(false));

            _registerMock.Setup(x => x.GetMatchedSourceIdentifier(It.IsAny<PersonalIdentifier>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(_fakeSourceIdentifier));

            _registerMock.Setup(x => x.RemoveSearchApiRequest(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            
            _sut = new PersonSearchController(_searchResultServiceMock.Object, _searchApiRequestServiceMock.Object, _dataPartnerServiceMock.Object, _loggerMock.Object, _mapper.Object, _registerMock.Object);

        }

        [Test]
        public async Task With_valid_completed_event_it_should_return_ok()
        {
            
            var result = await _sut.Completed(_searchRequestKey, _fakePersonCompletedEvent);

            _searchResultServiceMock.Verify(x => x.ProcessPersonFound(It.Is<Person>(x => x.FirstName == "TEST1"), It.IsAny<ProviderProfile>(), It.IsAny<SSG_SearchRequest>(),It.IsAny<Guid>(), It.IsAny<CancellationToken>(),It.IsAny<SSG_Identifier>()), Times.Once);
            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            _dataPartnerServiceMock
               .Verify(x => x.GetSearchApiRequestDataProvider(It.Is<Guid>(x => x == _testGuid), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _dataPartnerServiceMock
              .Verify(x => x.UpdateSearchRequestApiProvider(It.IsAny<SSG_SearchapiRequestDataProvider>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task With_valid_finalized_event_it_should_return_ok()
        {
            var result = await _sut.Finalized(_searchRequestKey, _fakePersonFinalizedEvent);
           _searchApiRequestServiceMock
                .Verify(x => x.MarkComplete(It.Is<Guid>(x => x == _testGuid), It.IsAny<CancellationToken>()), Times.Once);
           _registerMock.Verify(x => x.RemoveSearchApiRequest(It.IsAny<string>()), Times.Once);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task With_valid_finalized_event_with_inomcplete_search_jca_it_should_not_mark_complete_and_not_delete_key()
        {
            var result = await _sut.Finalized(_searchRequestKeySearchNotComplete, _fakePersonFinalizedEvent);
            _searchApiRequestServiceMock
                 .Verify(x => x.MarkComplete(It.Is<Guid>(x => x == _testGuid), It.IsAny<CancellationToken>()), Times.Never());
            _registerMock.Verify(x => x.RemoveSearchApiRequest(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task With_exception_completed_event_should_return_badrequest()
        {
            var result = await _sut.Completed(_exceptionSearchRequestKey, _fakePersonCompletedEvent);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task With_null_completed_event_should_return_bad_request_()
        {
            var result = await _sut.Completed(_exceptionSearchRequestKey, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }
        [Test]
        public async Task With_valid_accepted_event_it_should_return_ok()
        {
            var result = await _sut.Accepted(_searchRequestKey, _fakePersonAcceptedEvent);
            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_event_it_should_return_badrequest()
        {
            var result = await _sut.Accepted(_exceptionSearchRequestKey, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }


        [Test]
        public async Task With_valid_failed_event_it_should_return_ok()
        {
            var result = await _sut.Failed(_searchRequestKey, _fakePersonFailedEvent);


            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            _dataPartnerServiceMock
            .Verify(x => x.GetSearchApiRequestDataProvider(It.Is<Guid>(x => x == _testGuid), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _dataPartnerServiceMock
              .Verify(x => x.UpdateSearchRequestApiProvider(It.IsAny<SSG_SearchapiRequestDataProvider>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_failed_event_it_should_return_badrequest()
        {
            var result = await _sut.Failed(_exceptionSearchRequestKey, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task With_valid_rejected_event_it_should_return_ok()
        {
            var result = await _sut.Rejected(_searchRequestKey, _fakePersonRejectEvent);


            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_rejected_event_it_should_return_badrequest()
        {
            var result = await _sut.Rejected(_exceptionSearchRequestKey, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task With_valid_submitted_event_it_should_return_ok()
        {
            var result = await _sut.Submitted(_searchRequestKey, _fakePersonSubmittedEvent);

            _searchApiRequestServiceMock
                .Verify(x => x.AddEventAsync(It.Is<Guid>(x => x == _testGuid), It.IsAny<SSG_SearchApiEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }


        [Test]
        public async Task With_exception_submitted_event_it_should_return_badrequest()
        {
            var result = await _sut.Submitted(_exceptionSearchRequestKey, null);
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }
    }
}
