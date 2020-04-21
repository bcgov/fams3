using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
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

            int readyForSearchVAlue = SearchApiRequestStatusReason.ReadyForSearch.Value;
            int inProgressValue = SearchApiRequestStatusReason.InProgress.Value;
            int completeValue = SearchApiRequestStatusReason.Complete.Value;

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
                .Select(x => x.SearchApiRequestId)
                .Filter(It.IsAny<Expression<Func<SSG_SearchApiRequest, bool>>>())
                .FindEntriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(new List<SSG_SearchApiRequest>()
                {
                     new SSG_SearchApiRequest()
                     {
                         SearchApiRequestId= _testId
                     }
                }));

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
                .Key(_testId)
                .Expand(x=>x.Identifiers)
                .Expand(x=>x.DataProviders)
                .Expand(x=>x.SearchRequest)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest
                {                      
                    SearchApiRequestId = _testId,
                    PersonGivenName = "personGivenName1",
                    Identifiers = new List<Identifier.SSG_Identifier>(){
                        new Identifier.SSG_Identifier()
                        {
                            Identification="identification1",
                            StatusCode = 1
                        },
                        new Identifier.SSG_Identifier()
                        {
                            Identification="identification2",
                            StatusCode = 1
                        }
                    }.ToArray(),
                    DataProviders = new List<DataProvider.SSG_SearchapiRequestDataProvider>()
                    {
                        new DataProvider.SSG_SearchapiRequestDataProvider(){Name="ICBC", SuppliedByValue=111111}
                    }.ToArray(),
                    SearchRequest = new SSG_SearchRequest() { FileId="fileId"}
                }));

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
                .Key(_testId)
                .Set(new Dictionary<string, object>() { { Keys.DYNAMICS_STATUS_CODE_FIELD, inProgressValue } })
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest()
                    {
                        SearchApiRequestId = Guid.NewGuid(),
                        PersonGivenName = "personGivenName",
                        StatusCode = SearchApiRequestStatusReason.InProgress.Value
                    }));

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
             .Key(_testId)
             .Set(new Dictionary<string, object>() { { Keys.DYNAMICS_STATUS_CODE_FIELD, completeValue } })
             .UpdateEntryAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest()
             {
                 SearchApiRequestId = Guid.NewGuid(),
                 PersonGivenName = "personGivenName",
                 StatusCode = SearchApiRequestStatusReason.Complete.Value
             }));

            _sut = new SearchApiRequestService(odataClientMock.Object);

        }


        [Test]
        public void with_success_should_return_a_collection_of_search_request()
        {
            var result = _sut.GetAllReadyForSearchAsync(CancellationToken.None).Result;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("personGivenName1", result.FirstOrDefault().PersonGivenName);
            Assert.AreEqual(2, result.FirstOrDefault().Identifiers.Count());
            Assert.AreEqual(1, result.FirstOrDefault().DataProviders.Count());
        }

        [Test]
        public void With_empty_guid_should_throw_ArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.MarkInProgress(Guid.Empty, CancellationToken.None));
        }

        [Test]
        public void With_empty_guid_should_throw_mark_completeArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.MarkComplete(Guid.Empty, CancellationToken.None));
        }


        [Test]
        public async Task With_guid_should_mark_entry_in_progress()
        {
            var result = await _sut.MarkInProgress(_testId, CancellationToken.None);

            Assert.AreEqual(SearchApiRequestStatusReason.InProgress.Value, result.StatusCode);
        }

        [Test]
        public async Task With_guid_should_mark_entry_complete()
        {
            var result = await _sut.MarkComplete(_testId, CancellationToken.None);

            Assert.AreEqual(SearchApiRequestStatusReason.Complete.Value, result.StatusCode);
        }


    }
}