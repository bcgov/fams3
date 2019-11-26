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
    public class SearchApiRequestServiceAddEventTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

        private Guid _testId;

        private SearchApiRequestService _sut;

        private const string eventName = "TEST_EVENT";

        [SetUp]
        public void SetUp()
        {

            _testId = Guid.NewGuid();

            int readyForSearchVAlue = SearchApiRequestStatusReason.ReadyForSearch.Value;
            int inProgressValue = SearchApiRequestStatusReason.InProgress.Value;

            odataClientMock.Setup(x => x
                    .For<SSG_SearchApiEvent>(null)
                    .Set(It.Is<SSG_SearchApiEvent>(x => x.Name == eventName))
                    .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchApiEvent>(new SSG_SearchApiEvent()
                {
                    Name = eventName,
                    Id = _testId
                }));

            _sut = new SearchApiRequestService(odataClientMock.Object);

        }


        [Test]
        public async Task with_success_should_return_event()
        {

            var searchApiEvent = new SSG_SearchApiEvent()
            {
                Name = eventName
            };

            var result = await _sut.AddEventAsync(_testId, searchApiEvent, CancellationToken.None);
            Assert.AreEqual(_testId, result.Id);
            Assert.AreEqual(eventName, result.Name);
        }

        [Test]
        public void With_empty_guid_should_throw_ArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.AddEventAsync(Guid.Empty, null, CancellationToken.None));
        }


    }
}