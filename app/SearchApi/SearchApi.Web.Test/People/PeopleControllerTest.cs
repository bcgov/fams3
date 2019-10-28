using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenTracing;
using SearchApi.Web.Controllers;

namespace SearchApi.Web.Test.People
{
    public class PeopleControllerTest
    {

        private PeopleController _sut;

        private readonly Mock<ITracer> _tracerMock = new Mock<ITracer>();

        private readonly Mock<ISpan> _spanMock = new Mock<ISpan>();

        [SetUp]
        public void Init()
        {

            _spanMock.Setup(x => x.SetTag(It.IsAny<string>(), It.IsAny<string>())).Returns(_spanMock.Object);
            _tracerMock.Setup(x => x.ActiveSpan).Returns(_spanMock.Object);
            _sut = new PeopleController(null, _tracerMock.Object);
        }


        [Test]
        public void With_valid_payload_should_return_created()
        {
            var result =
                (AcceptedResult) this._sut.Search(new PersonSearchRequest("firstName", "lastName", null)).Result;
            Assert.IsInstanceOf<PersonSearchResponse>(result.Value);
            Assert.IsNotNull(((PersonSearchResponse)result.Value).Id);
            _spanMock.Verify(x => x.SetTag("searchRequestId", $"{((PersonSearchResponse)result.Value).Id}"), Times.Once);

        }

    }
}