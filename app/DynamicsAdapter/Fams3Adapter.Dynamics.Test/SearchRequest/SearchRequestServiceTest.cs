using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.ResultTransaction;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();

            odataClientMock.Setup(x => x.For<SSG_SearchRequestResultTransaction>(null).Set(It.IsAny<SSG_SearchRequestResultTransaction>())
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_SearchRequestResultTransaction()
            {
                SourceIdentifier = new SSG_Identifier() { Identification = "11111" }
            })
            );

            _sut = new SearchRequestService(odataClientMock.Object, _duplicateServiceMock.Object);
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


    }
}
