using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.SearchApiEvent;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;

namespace Fams3Adapter.Dynamics.Test.SearchApiRequest
{
    public class SearchApiRequestServiceGetLinkedSearchRequestIdAsyncTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

        private Guid _testId;
        private Guid _searchRequestId;

        private SearchApiRequestService _sut;

        private const string eventName = "TEST_EVENT";

        [SetUp]
        public void SetUp()
        {

            _testId = Guid.NewGuid();
            _searchRequestId = Guid.NewGuid();

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
                .Key(It.Is<Guid>(x => x == _testId))
                .Select(x => x.SearchRequestId)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest()
                {
                    SearchRequestId = _searchRequestId
                }));

            _sut = new SearchApiRequestService(odataClientMock.Object);

        }


        [Test]
        public async Task with_success_should_return_event()
        {
            var result = await _sut.GetLinkedSearchRequestIdAsync(_testId, CancellationToken.None);
            Assert.AreEqual(_searchRequestId, result);
        }

        [Test]
        public void With_empty_guid_should_throw_ArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.GetLinkedSearchRequestIdAsync(Guid.Empty, CancellationToken.None));
        }


    }
}