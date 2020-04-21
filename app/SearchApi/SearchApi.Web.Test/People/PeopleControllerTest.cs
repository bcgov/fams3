using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.Redis;
using BcGov.Fams3.Redis.Model;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpenTracing;
using SearchApi.Web.Controllers;
using SearchApi.Web.Messaging;

namespace SearchApi.Web.Test.People
{
    public class PeopleControllerTest
    {

        private PeopleController _sut;

        private Mock<ITracer> _tracerMock;

        private Mock<ISpan> _spanMock;

        private Mock<ILogger<PeopleController>> _loggerMock;

        private Mock<IDispatcher> _dispatcherMock;
        private Mock<ICacheService> _cacheMock;

        private Guid expectedId = Guid.NewGuid();

        [SetUp]
        public void Init()
        {

            _dispatcherMock = new Mock<IDispatcher>();

            _tracerMock = new Mock<ITracer>();

            _spanMock = new Mock<ISpan>();

            _loggerMock = new Mock<ILogger<PeopleController>>();

            _cacheMock = new Mock<ICacheService>();

            _spanMock.Setup(x => x.SetTag(It.IsAny<string>(), It.IsAny<string>())).Returns(_spanMock.Object);
            _tracerMock.Setup(x => x.ActiveSpan).Returns(_spanMock.Object);

            _dispatcherMock.Setup(x => x.Dispatch(It.IsAny<PersonSearchRequest>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            _sut = new PeopleController(_loggerMock.Object, _tracerMock.Object, _dispatcherMock.Object,_cacheMock.Object);
        }


        [Test]
        public void With_valid_payload_should_return_created()
        {
            var result =
                (AcceptedResult)this._sut.Search(null, new PersonSearchRequest("firstName", "lastName", null, new List<PersonalIdentifier>(), new List<Address>(), new List<Phone>(), new List<Name>(), new List<RelatedPerson>(), new List<Employment>(), new List<DataProvider>(),"fileId")).Result;

            Assert.IsInstanceOf<PersonSearchResponse>(result.Value);
            Assert.IsNotNull(((PersonSearchResponse)result.Value).Id);
            _spanMock.Verify(x => x.SetTag("searchRequestId", $"{((PersonSearchResponse)result.Value).Id}"), Times.Once);
            _cacheMock.Verify(x => x.SaveRequest(It.Is<SearchRequest>(m => m.SearchRequestId != expectedId)), Times.Once);
            _dispatcherMock.Verify(x => x.Dispatch(It.IsAny<PersonSearchRequest>(), It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void With_valid_payload_and_id_should_return_created()
        {
            var result =
                (AcceptedResult)this._sut.Search($"{expectedId}", new PersonSearchRequest("firstName", "lastName", null, new List<PersonalIdentifier>(), new List<Address>(), new List<Phone>(), new List<Name>(), new List<RelatedPerson>(), new List<Employment>(), new List<DataProvider>() , "fileId")).Result;

            Assert.IsInstanceOf<PersonSearchResponse>(result.Value);
            Assert.AreEqual(expectedId, ((PersonSearchResponse)result.Value).Id);
            _spanMock.Verify(x => x.SetTag("searchRequestId", $"{((PersonSearchResponse)result.Value).Id}"), Times.Once);
            _cacheMock.Verify(x => x.SaveRequest(It.Is<SearchRequest>(m => m.SearchRequestId == expectedId)), Times.Once);
            _dispatcherMock.Verify(x => x.Dispatch(It.IsAny<PersonSearchRequest>(), It.IsAny<Guid>()), Times.Once);
        }

    }
}