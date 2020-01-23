using System;
using System.Collections.Generic;
using System.Threading;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpenTracing;
using SearchApi.Web.Controllers;

namespace SearchApi.Web.Test.People
{
    public class PeopleControllerTest
    {

        private PeopleController _sut;

        private readonly Mock<ITracer> _tracerMock = new Mock<ITracer>();

        private readonly Mock<ISpan> _spanMock = new Mock<ISpan>();

        private readonly Mock<ILogger<PeopleController>> _loggerMock = new Mock<ILogger<PeopleController>>();

        private Mock<IBusControl> _busControlMock;

            [SetUp]
        public void Init()
        {
            _busControlMock = new Mock<IBusControl>();
            _spanMock.Setup(x => x.SetTag(It.IsAny<string>(), It.IsAny<string>())).Returns(_spanMock.Object);
            _tracerMock.Setup(x => x.ActiveSpan).Returns(_spanMock.Object);
            _sut = new PeopleController(_busControlMock.Object, _loggerMock.Object, _tracerMock.Object);
        }


        [Test]
        public void With_valid_payload_should_return_created()
        {
            var result =
                (AcceptedResult) this._sut.Search(null, new PersonSearchRequest("firstName", "lastName", null, new List<PersonalIdentifier>(), new List<Address>(), new List<Phone>(), new List<Name>())).Result;
            Assert.IsInstanceOf<PersonSearchResponse>(result.Value);
            Assert.IsNotNull(((PersonSearchResponse)result.Value).Id);
            _spanMock.Verify(x => x.SetTag("searchRequestId", $"{((PersonSearchResponse)result.Value).Id}"), Times.Once);
            _busControlMock.Verify(x => x.Publish(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void With_valid_payload_and_id_should_return_created()
        {
            var expectedId = Guid.NewGuid();

            var result =
                (AcceptedResult)this._sut.Search($"{expectedId}", new PersonSearchRequest("firstName", "lastName", null, new List<PersonalIdentifier>(), new List<Address>(), new List<Phone>(), new List<Name>())).Result;
            Assert.IsInstanceOf<PersonSearchResponse>(result.Value);
            Assert.AreEqual( expectedId, ((PersonSearchResponse)result.Value).Id);
            _spanMock.Verify(x => x.SetTag("searchRequestId", $"{((PersonSearchResponse)result.Value).Id}"), Times.Once);
            _busControlMock.Verify(x => x.Publish(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}