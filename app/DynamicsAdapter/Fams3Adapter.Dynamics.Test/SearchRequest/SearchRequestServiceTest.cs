using Fams3Adapter.Dynamics.APICall;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();

            odataClientMock.Setup(x => x.For<SSG_SearchRequestResultTransaction>(null).Set(It.IsAny<SSG_SearchRequestResultTransaction>())
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_SearchRequestResultTransaction()
            {
                SourceIdentifier = new SSG_Identifier() { Identification = "11111" }
            })
            );

            _sut = new SearchRequestService(odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task upload_ResultTransaction_should_success()
        {
            var trans = new SSG_SearchRequestResultTransaction() { };

            var result = await _sut.CreateTransaction(trans, CancellationToken.None);

            Assert.AreEqual("11111", result.SourceIdentifier.Identification);
        }

        [Test]
        public void Invalid_searchRequestId_SubmitToQueue_should_throw_exception()
        {
            Guid invalidGuid = Guid.NewGuid();
            odataClientMock.Setup(
                x => x.For<SSG_SearchRequest>(null).Key(It.Is<Guid>(m => m == invalidGuid))
                      .Action(It.IsAny<string>())
                      .ExecuteAsSingleAsync())
            .Throws(new Exception("invalid search request id"));
            Assert.ThrowsAsync<Exception>(
                async () => await _sut.SubmitToQueue(invalidGuid)
                );
        }

        [Test]
        public async Task SubmitToQueue_should_success()
        {
            odataClientMock.Setup(
                x => x.For<SSG_SearchRequest>(null).Key(It.IsAny<Guid>())
                      .Action(It.IsAny<string>())
                      .ExecuteAsSingleAsync())
            .Returns(Task.FromResult(new SSG_SearchRequest()
            {
                SearchRequestId = Guid.NewGuid()
            })
            );
            var result = await _sut.SubmitToQueue(Guid.NewGuid());
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task UpdateApiCall_should_success()
        {
            odataClientMock.Setup(
                x => x.For<SSG_APICall>(null).Key(It.IsAny<Guid>())
                      .Set(It.IsAny<IDictionary<string,object>>())
                      .UpdateEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_APICall(){ }));
            var result = await _sut.UpdateApiCall(Guid.NewGuid(), true, "", CancellationToken.None);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Invalid_UpdateApiCall_should_throw_exception()
        {
            Guid invalidGuid = Guid.NewGuid();
            odataClientMock.Setup(
                x => x.For<SSG_APICall>(null).Key(It.Is<Guid>(m=>m==invalidGuid))
                      .Set(It.IsAny<IDictionary<string, object>>())
                      .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                      .Throws(new Exception("invalid apiCall id"));
            Assert.ThrowsAsync<Exception>(
                async () => await _sut.UpdateApiCall(invalidGuid, true, "", CancellationToken.None)
                );
        }

        [Test]
        public async Task GetAutoCloseSearchRequest_should_success()
        {
            Guid invalidGuid = Guid.NewGuid();
            odataClientMock.Setup(
                x => x.For<SSG_SearchRequest>(null).Filter(It.IsAny<Expression<Func<SSG_SearchRequest, bool>>>())
                      .FindEntriesAsync(It.IsAny<CancellationToken>()))
                      .Returns(Task.FromResult<IEnumerable<SSG_SearchRequest>>(new List<SSG_SearchRequest>()
                {
                     new SSG_SearchRequest()
                     {
                         FileId = "1111"
                     }
                }));
            var result = await _sut.GetAutoCloseSearchRequestAsync(CancellationToken.None);
            Assert.AreEqual(1, result.Count());
        }
    }
}
