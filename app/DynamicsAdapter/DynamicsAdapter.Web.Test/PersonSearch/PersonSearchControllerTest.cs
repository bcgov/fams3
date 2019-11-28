using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.PersonSearch;
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

        [SetUp]
        public void Init()
        {

            _testGuid = Guid.NewGuid();
            _exceptionGuid = Guid.NewGuid();
            _loggerMock = new Mock<ILogger<PersonSearchController>>();
            _searchApiRequestServiceMock = new Mock<ISearchApiRequestService>();
            _searchRequestServiceMock = new Mock<ISearchRequestService>();

            var validRequestId = Guid.NewGuid();
            var invalidRequestId = Guid.NewGuid();


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

            _sut = new PersonSearchController(_searchRequestServiceMock.Object, _searchApiRequestServiceMock.Object, _loggerMock.Object);

        }

        [Test]
        public async Task With_valid_match_found_data_should_return_ok()
        {
            var result = await _sut.MatchFound(_testGuid, new MatchFound()
            {
                PersonIds = new List<PersonId>()
                {
                    new PersonId()
                    {
                        Issuer = "test",
                        Number = "test",
                        Kind = PersonIDKind.DriverLicense
                    }
                }
            });
            Assert.IsInstanceOf(typeof(OkResult), result);

        }

        [Test]
        public async Task With_exception_match_found_data_should_return_badrequest()
        {
            var result = await _sut.MatchFound(_exceptionGuid, new MatchFound()
            {
                PersonIds = new List<PersonId>()
                {
                    new PersonId()
                    {
                        Issuer = "test",
                        Number = "test",
                        Kind = PersonIDKind.DriverLicense
                    }
                }
            });
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task With_valid_event_it_should_return_ok()
        {
            var result = await _sut.Accepted(_testGuid, new PersonSearchController.PersonAcceptedEvent()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ProviderProfile = new PersonSearchController.ProviderProfile()
                {
                    Name = "TEST PROVIDER"
                }
            });


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

    }
}
