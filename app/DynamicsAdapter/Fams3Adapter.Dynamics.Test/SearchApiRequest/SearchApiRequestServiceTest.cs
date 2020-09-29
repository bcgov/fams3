using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Fams3Adapter.Dynamics.DataProvider;
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
        private Guid _testSRId;

        private SearchApiRequestService _sut;
        private string adaptorName = "ICBC";
        private int retries = 10;

        List<SSG_DataProvider> providersList;
        [SetUp]
        public void SetUp()
        {
            _testSRId = Guid.NewGuid();
            _testId = Guid.NewGuid();
            providersList = new List<SSG_DataProvider>()
                  {
                      new SSG_DataProvider()
                      {
                        AdaptorName = adaptorName,
                        NumberOfDaysToRetry =retries,
                        SearchSpeed = "Fast",
                        TimeBetweenRetries = 60,
                        NumberOfRetries = 3

                      }
                  };
            int readyForSearchVAlue = SearchApiRequestStatusReason.ReadyForSearch.Value;
            int inProgressValue = SearchApiRequestStatusReason.InProgress.Value;
            int completeValue = SearchApiRequestStatusReason.Complete.Value;

            odataClientMock.Setup(x => x.For<SSG_DataProvider>(null)
            .FindEntriesAsync(It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<IEnumerable<SSG_DataProvider>>(

                  providersList
               ));

            odataClientMock.Setup(x => x.For<SSG_SearchapiRequestDataProvider>(null)
            .Select(It.IsAny<Expression<Func<SSG_SearchapiRequestDataProvider, object>>>())
              .Filter(It.IsAny<Expression<Func<SSG_SearchapiRequestDataProvider, bool>>>())
                .Filter(It.IsAny<Expression<Func<SSG_SearchapiRequestDataProvider, bool>>>())
             .Filter(It.IsAny<Expression<Func<SSG_SearchapiRequestDataProvider, bool>>>())
             .Filter(It.IsAny<Expression<Func<SSG_SearchapiRequestDataProvider, bool>>>())
          .FindEntriesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<SSG_SearchapiRequestDataProvider>>(

                new List<SSG_SearchapiRequestDataProvider>()
                {
                      new SSG_SearchapiRequestDataProvider()
                      {
                        SearchAPIRequestId = _testId,
                        AdaptorName = "ICBC",
                        NumberOfDaysToRetry = 10,
                        
                      }
                }
             ));

            odataClientMock.Setup(x => x.For<SSG_SearchApiRequest>(null)
               
                .Select(It.IsAny<Expression<Func<SSG_SearchApiRequest, object>>>())
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
                            StatusCode = 1,
                            IdentifierId = Guid.NewGuid()
                        },
                        new Identifier.SSG_Identifier()
                        {
                            Identification="identification2",
                            StatusCode = 1,
                            IdentifierId = Guid.NewGuid()
                        }
                    }.ToArray(),
                    DataProviders = new List<SSG_SearchapiRequestDataProvider>()
                    {
                        new SSG_SearchapiRequestDataProvider(){AdaptorName="ICBC"}
                    }.ToArray(),
                    SearchRequest = new SSG_SearchRequest() { SearchRequestId= _testSRId, FileId ="fileId"}
                }));

            odataClientMock.Setup(x => x.For<SSG_SearchRequest>(null)
                .Key(_testSRId)
                .Expand(x => x.SearchReason)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_SearchRequest>(new SSG_SearchRequest
                {
                    FileId = "fileId",
                    SearchReason = new SSG_SearchRequestReason { ReasonCode="reasonCode"}
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
            var result = _sut.GetAllReadyForSearchAsync(CancellationToken.None, providersList.ToArray()).Result;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("personGivenName1", result.FirstOrDefault().PersonGivenName);
            Assert.AreEqual(2, result.FirstOrDefault().Identifiers.Count());
            Assert.AreEqual(1, result.FirstOrDefault().DataProviders.Count());
            Assert.AreEqual(60, result.FirstOrDefault().DataProviders[0].TimeBetweenRetries);
            Assert.AreEqual(3, result.FirstOrDefault().DataProviders[0].NumberOfRetries);
            Assert.AreEqual(false, result.FirstOrDefault().IsFailed);
        }


        [Test]
        public void with_success_should_return_a_collection_of_failed_search_request()
        {
            var result = _sut.GetAllValidFailedSearchRequest(CancellationToken.None, providersList.ToArray()).Result;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("personGivenName1", result.FirstOrDefault().PersonGivenName);
            Assert.AreEqual(2, result.FirstOrDefault().Identifiers.Count());
            Assert.AreEqual(true, result.FirstOrDefault().IsFailed);
            Assert.AreEqual(1, result.FirstOrDefault().DataProviders.Count());
        }

        [Test]
        public void with_success_should_return_a_collection_of_data_provider()
        {
            var result = _sut.GetDataProvidersList(CancellationToken.None).Result;
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ICBC", result.FirstOrDefault().AdaptorName);
            Assert.AreEqual(10, result.FirstOrDefault().NumberOfDaysToRetry);
            Assert.AreEqual("Fast", result.FirstOrDefault().SearchSpeed);
            Assert.AreEqual(60, result.FirstOrDefault().TimeBetweenRetries);
            Assert.AreEqual(3, result.FirstOrDefault().NumberOfRetries);
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