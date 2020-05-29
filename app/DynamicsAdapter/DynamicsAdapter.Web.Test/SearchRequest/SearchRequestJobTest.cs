using AutoMapper;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Quartz;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class SearchRequestJobTest
    {
        private readonly Mock<ISearchApiRequestService> _searchApiRequestServiceMock = new Mock<ISearchApiRequestService>();
        private readonly Mock<ISearchApiClient> _searchApiClientMock = new Mock<ISearchApiClient>();
        private List<SSG_SearchApiRequest> _fakeSearchApiRequests;
        private Guid _validSearchApiRequestId;
        private Guid _validSearchRequestId;
        private Guid _exceptionSearchRequestId;
        private SearchRequestJob _sut;
        private Mock<ILogger<SearchRequestJob>> _loggerMock = new Mock<ILogger<SearchRequestJob>>();
        private Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private Mock<IJobExecutionContext> _jobContext = new Mock<IJobExecutionContext>();

        [SetUp]
        public void Init()
        {
            _validSearchApiRequestId = Guid.NewGuid();
            _validSearchRequestId = Guid.NewGuid();
            _exceptionSearchRequestId = Guid.NewGuid();

            _fakeSearchApiRequests = new List<SSG_SearchApiRequest>()
            {
                new SSG_SearchApiRequest()
                {
                    SearchApiRequestId=_validSearchApiRequestId,
                    SearchRequestId=_validSearchRequestId,
                    SearchRequest = new SSG_SearchRequest(){FileId="111111"}
                }
            };


            _searchApiClientMock.Setup(
                x => x.SearchAsync(
                    It.IsAny<PersonSearchRequest>(),
                    It.Is<string>(m => m == _validSearchApiRequestId.ToString()),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<PersonSearchResponse>(new PersonSearchResponse() { Id = _validSearchRequestId }));

            _searchApiRequestServiceMock.Setup(
                x => x.MarkInProgress(
                    It.Is<Guid>(x => x == _validSearchApiRequestId),
                    It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_SearchApiRequest>(new SSG_SearchApiRequest()
               {
                   SearchApiRequestId = _validSearchRequestId,
                   Name = "Random Event"
               }));

            _mapperMock.Setup(x => x.Map<PersonSearchRequest>(It.IsAny<SSG_SearchApiRequest>()))
                .Returns(new PersonSearchRequest() { FileID = "fileId" });


        }

        [Test]
        public async Task valid_searchapiRequest_execute_run_successfully()
        {

            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(_fakeSearchApiRequests));

            _sut = new SearchRequestJob(
                _searchApiClientMock.Object,
                _searchApiRequestServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);

            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.Is<string>(m => m == _validSearchApiRequestId.ToString()),
                  It.IsAny<CancellationToken>()),
                  Times.Once);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.Is<Guid>(m => m == _validSearchApiRequestId), It.IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(LogLevel.Debug, $"Attempting to post person search for request {_validSearchApiRequestId}", Times.Once());
        }

        [Test]
        public async Task searchapiRequest_not_have_SearchRequestId_execute_run_successfully()
        {
            List<SSG_SearchApiRequest> noSearchRequestIDRequests = new List<SSG_SearchApiRequest>()
            {
                new SSG_SearchApiRequest()
                {
                    SearchApiRequestId=_validSearchApiRequestId,
                    SearchRequestId=Guid.Empty,
                    SearchRequest = new SSG_SearchRequest(){FileId="111111"}
                }
            };
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(noSearchRequestIDRequests));

            _sut = new SearchRequestJob(
                _searchApiClientMock.Object,
                _searchApiRequestServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);

            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.IsAny<string>(),
                  It.IsAny<CancellationToken>()),
                  Times.Never);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task searchAsyn_throw_exception_should_be_caught()
        {
            List<SSG_SearchApiRequest> exceptionSearchRequestIDRequests = new List<SSG_SearchApiRequest>()
            {
                new SSG_SearchApiRequest()
                {
                    SearchApiRequestId=_exceptionSearchRequestId,
                    SearchRequestId=_validSearchRequestId,
                    SearchRequest = new SSG_SearchRequest(){FileId="111111"}
                }
            };
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(exceptionSearchRequestIDRequests));

            _searchApiClientMock.Setup(
                x => x.SearchAsync(
                    It.IsAny<PersonSearchRequest>(),
                    It.Is<string>(m => m == _exceptionSearchRequestId.ToString()),
                    It.IsAny<CancellationToken>()))
                .Throws(new Exception("search Async throws random exception"));


            _sut = new SearchRequestJob(
                _searchApiClientMock.Object,
                _searchApiRequestServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);

            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.IsAny<string>(),
                  It.IsAny<CancellationToken>()),
                  Times.Once);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

            _loggerMock.VerifyLog(LogLevel.Error, $"search Async throws random exception", Times.Once());
        }

        [Test]
        public async Task markInProgress_throw_exception_should_be_caught()
        {
            List<SSG_SearchApiRequest> exceptionSearchRequestIDRequests = new List<SSG_SearchApiRequest>()
            {
                new SSG_SearchApiRequest()
                {
                    SearchApiRequestId=_exceptionSearchRequestId,
                    SearchRequestId=_validSearchRequestId,
                    SearchRequest = new SSG_SearchRequest(){FileId="111111"}
                }
            };
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(exceptionSearchRequestIDRequests));

            _searchApiClientMock.Setup(
                 x => x.SearchAsync(
                     It.IsAny<PersonSearchRequest>(),
                     It.IsAny<string>(),
                     It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult<PersonSearchResponse>(new PersonSearchResponse() { Id = _validSearchRequestId }));
           
            _searchApiRequestServiceMock.Setup(
                    x => x.MarkInProgress(
                        It.Is<Guid>(x => x == _exceptionSearchRequestId),
                        It.IsAny<CancellationToken>()))
                   .Throws(new Exception("mark in progress throws random exception"));


            _sut = new SearchRequestJob(
                _searchApiClientMock.Object,
                _searchApiRequestServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);

            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.IsAny<string>(),
                  It.IsAny<CancellationToken>()),
                  Times.Once);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, $"mark in progress throws random exception", Times.Once());
        }
    }
}
