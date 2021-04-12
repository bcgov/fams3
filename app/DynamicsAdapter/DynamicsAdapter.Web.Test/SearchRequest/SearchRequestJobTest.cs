using AutoMapper;
using DynamicsAdapter.Web.Configuration;
using DynamicsAdapter.Web.Register;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Quartz;
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
        private readonly Mock<ISearchRequestRegister> _searchApiRequestRegisterMock = new Mock<ISearchRequestRegister>();
        private List<SSG_SearchApiRequest> _fakeSearchApiRequests;
        private Guid _validSearchApiRequestId;
        private Guid _validSearchRequestId;
        private Guid _exceptionSearchRequestId;
        private Guid _searchAsyncExceptionSearchRequestId;
        private Guid _emptySearchApiRequestId;
        private SearchRequestJob _sut;
        private Mock<ILogger<SearchRequestJob>> _loggerMock = new Mock<ILogger<SearchRequestJob>>();
        private Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private Mock<IJobExecutionContext> _jobContext = new Mock<IJobExecutionContext>();
        private Mock<IOptions<SearchApiConfiguration>> _searchApiOptionsMock = new Mock<IOptions<SearchApiConfiguration>>();
        private SearchApiConfiguration _searchApiOptionReal;
      

        [SetUp]
        public void Init()
        {
            _validSearchApiRequestId = Guid.NewGuid();
            _validSearchRequestId = Guid.NewGuid();
            _exceptionSearchRequestId = Guid.NewGuid();
            _emptySearchApiRequestId = Guid.NewGuid();
            _searchAsyncExceptionSearchRequestId = Guid.NewGuid();

            SSG_SearchApiRequest ssgValidSearchApiRequest = new SSG_SearchApiRequest()
            {
                SearchApiRequestId = _validSearchApiRequestId,
                SearchRequestId = _validSearchRequestId,
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" }
            };

         
            _searchApiOptionReal = new SearchApiConfiguration
            {
                AvailableDataPartner = "MSDPR:ICBC,BCHYDRO",
                BaseUrl = "http://base_url"
            };
            _searchApiOptionsMock.Setup(x => x.Value).Returns
                (_searchApiOptionReal);

            SSG_SearchApiRequest ssgExceptionSearchApiRequest = new SSG_SearchApiRequest()
            {
                SearchApiRequestId = _exceptionSearchRequestId,
                SearchRequestId = _validSearchRequestId,
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" }
            };

            SSG_SearchApiRequest ssgSearchAsyncExceptionSearchApiRequest = new SSG_SearchApiRequest()
            {
                SearchApiRequestId = _searchAsyncExceptionSearchRequestId,
                SearchRequestId = _validSearchRequestId,
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" }
            };

            _fakeSearchApiRequests = new List<SSG_SearchApiRequest>()
            {
                ssgValidSearchApiRequest
            };

            _searchApiRequestRegisterMock.Setup(
                x => x.FilterDuplicatedIdentifier(It.Is<SSG_SearchApiRequest>(x => x.SearchApiRequestId == _validSearchApiRequestId)))
                .Returns(ssgValidSearchApiRequest);

            _searchApiRequestRegisterMock.Setup(
                x => x.FilterDuplicatedIdentifier(It.Is<SSG_SearchApiRequest>(x => x.SearchApiRequestId == _exceptionSearchRequestId)))
                .Returns(ssgExceptionSearchApiRequest);


            //_searchApiRequestRegisterMock.Setup(
            //  x => x.SearchForSearchRequestKeys(It.Is<SSG_SearchApiRequest>(x => x.SearchApiRequestId == _validSearchApiRequestId)))
            //  .Returns(Task.FromResult(true));

            _searchApiRequestRegisterMock.Setup(
                x => x.FilterDuplicatedIdentifier(It.Is<SSG_SearchApiRequest>(x => x.SearchApiRequestId == _searchAsyncExceptionSearchRequestId)))
                .Returns(ssgSearchAsyncExceptionSearchApiRequest);

            _searchApiRequestRegisterMock.Setup(
                x => x.RegisterSearchApiRequest(It.IsAny<SSG_SearchApiRequest>()))
                .Returns(Task.FromResult(true));

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
                   SequenceNumber = "1234567"
               }));

            _mapperMock.Setup(x => x.Map<PersonSearchRequest>(It.IsAny<SSG_SearchApiRequest>()))
                .Returns(new PersonSearchRequest() { SearchRequestKey = "fileId" });

            _sut = new SearchRequestJob(
                _searchApiClientMock.Object,
                _searchApiRequestServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _searchApiRequestRegisterMock.Object,
                _searchApiOptionsMock.Object
                );

        }

        [Test]
        public async Task valid_searchapiRequest_execute_run_successfully()
        {

            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>(), It.IsAny<SSG_DataProvider[]>(), _searchApiOptionReal.AvailableDataPartner))
                .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(_fakeSearchApiRequests));


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
                    SearchApiRequestId=_emptySearchApiRequestId,
                    SearchRequestId=Guid.Empty,
                    SearchRequest = new SSG_SearchRequest(){FileId="111111"}
                }
            };
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>(), It.IsAny<SSG_DataProvider[]>(), _searchApiOptionReal.AvailableDataPartner))
            .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(noSearchRequestIDRequests));

            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.Is<string>(m => m == _emptySearchApiRequestId.ToString()),
                  It.IsAny<CancellationToken>()),
                  Times.Never);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.Is<Guid>(m => m == _emptySearchApiRequestId), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task searchAsyn_throw_exception_should_be_caught()
        {
            List<SSG_SearchApiRequest> exceptionSearchRequestIDRequests = new List<SSG_SearchApiRequest>()
            {
                new SSG_SearchApiRequest()
                {
                    SearchApiRequestId=_searchAsyncExceptionSearchRequestId,
                    SearchRequestId=_validSearchRequestId,
                    SearchRequest = new SSG_SearchRequest(){FileId="111111"}
                }
            };
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>(), It.IsAny<SSG_DataProvider[]>(), _searchApiOptionReal.AvailableDataPartner))
            .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(exceptionSearchRequestIDRequests));

            _searchApiClientMock.Setup(
                x => x.SearchAsync(
                    It.IsAny<PersonSearchRequest>(),
                    It.Is<string>(m => m == _searchAsyncExceptionSearchRequestId.ToString()),
                    It.IsAny<CancellationToken>()))
                .Throws(new Exception("search Async throws random exception"));

            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.Is<string>(m => m == _searchAsyncExceptionSearchRequestId.ToString()),
                  It.IsAny<CancellationToken>()),
                  Times.Once);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.Is<Guid>(m => m == _searchAsyncExceptionSearchRequestId), It.IsAny<CancellationToken>()), Times.Never);

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
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>(), It.IsAny<SSG_DataProvider[]>(), _searchApiOptionReal.AvailableDataPartner))
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


            await _sut.Execute(_jobContext.Object);
            _searchApiClientMock
              .Verify(x => x.SearchAsync(
                  It.IsAny<PersonSearchRequest>(),
                  It.Is<string>(m => m == _exceptionSearchRequestId.ToString()),
                  It.IsAny<CancellationToken>()),
                  Times.Once);

            _searchApiRequestServiceMock.Verify(x => x.MarkInProgress(It.Is<Guid>(m => m == _exceptionSearchRequestId), It.IsAny<CancellationToken>()), Times.Once);

            _loggerMock.VerifyLog(LogLevel.Error, $"mark in progress throws random exception", Times.Once());
        }

        [Test]
        public async Task register_throw_exception_should_be_caught()
        {
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>(), It.IsAny<SSG_DataProvider[]>(), _searchApiOptionReal.AvailableDataPartner))
               .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(_fakeSearchApiRequests));

            _searchApiRequestRegisterMock.Setup(
                    x => x.RegisterSearchApiRequest(It.IsAny<SSG_SearchApiRequest>()))
                    .Throws(new RegisterFailedException("register failed"));

            await _sut.Execute(_jobContext.Object);

            _loggerMock.VerifyLog(LogLevel.Error, $"register failed", Times.Once());
        }

        [Test]
        public async Task register_return_false_exception_should_be_caught()
        {
            _searchApiRequestServiceMock.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>(), It.IsAny<SSG_DataProvider[]>(), _searchApiOptionReal.AvailableDataPartner))
               .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(_fakeSearchApiRequests));

            _searchApiRequestRegisterMock.Setup(
                    x => x.RegisterSearchApiRequest(It.IsAny<SSG_SearchApiRequest>()))
                    .Returns(Task.FromResult(false));

            await _sut.Execute(_jobContext.Object);

            _loggerMock.VerifyLog(LogLevel.Error, $"Register SearchApiRequest to cache failed.", Times.Once());
        }
    }


}
