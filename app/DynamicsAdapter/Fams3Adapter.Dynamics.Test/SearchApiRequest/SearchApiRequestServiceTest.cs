using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;

namespace Fams3Adapter.Dynamics.Test.SearchApiRequest
{
    public class SearchApiRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

        private Guid _testId;

        private SearchApiRequestService _sut;

        [SetUp]
        public void SetUp()
        {

            _testId = Guid.NewGuid();

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
                    .Filter(x => x.StatusCode == 1)
                    .FindEntriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(new List<SSG_SearchApiRequest>()
                {
                    new SSG_SearchApiRequest()
                    {
                        SearchApiRequestId = Guid.NewGuid(),
                        PersonGivenName = "personGivenName"
                    }
                }));

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
                .Key(_testId)
                .Set(new Dictionary<string, object>() { { Keys.DYNAMICS_STATUS_CODE_FIELD, SearchApiRequestStatusReason.InProgress.GetHashCode() } })
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest()
                    {
                        SearchApiRequestId = Guid.NewGuid(),
                        PersonGivenName = "personGivenName",
                        StatusCode = SearchApiRequestStatusReason.InProgress.GetHashCode()
                    }));

            _sut = new SearchApiRequestService(odataClientMock.Object);

        }


        [Test]
        public void with_success_should_return_a_collection_of_search_request()
        {
            var result = _sut.GetAllReadyForSearchAsync(CancellationToken.None).Result;

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("personGivenName", result.FirstOrDefault().PersonGivenName);

        }

        [Test]
        public void With_empty_guid_should_throw_ArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.MarkInProgress(Guid.Empty, CancellationToken.None));
        }

        [Test]
        public async Task With_guid_should_mark_entry_in_progress()
        {
            var result = await _sut.MarkInProgress(_testId, CancellationToken.None);

            Assert.AreEqual(SearchApiRequestStatusReason.InProgress.GetHashCode(), result.StatusCode);
        }



    }
}