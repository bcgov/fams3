using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Quartz;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class SearchRequestJobTest
    {
     
        private readonly Mock<ILogger<SearchRequestJob>> _loggerMock = new Mock<ILogger<SearchRequestJob>>();
        private readonly Mock<IJobExecutionContext> _jobExecutionContextMock = new Mock<IJobExecutionContext>();
        private readonly Mock<ISearchApiClient> _searchApiClientMock = new Mock<ISearchApiClient>();
        private readonly Mock<ISearchApiRequestService> _searchRequestService = new Mock<ISearchApiRequestService>();


        private SearchRequestJob _sut;

        [SetUp]
        public void Setup()
        {

            _searchRequestService.Setup(x => x.GetAllReadyForSearchAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_SearchApiRequest>>(new List<SSG_SearchApiRequest>()
                {
                    new SSG_SearchApiRequest()
                    {
                        SearchApiRequestId = Guid.NewGuid(),
                        PersonGivenName = "personGivenName"
                    }
                }));

            PersonSearchRequest personSearchRequest = new PersonSearchRequest();
            _searchApiClientMock.Setup(x => x.SearchAsync(It.IsAny<PersonSearchRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(
                new PersonSearchResponse()
                {
                    Id = Guid.NewGuid()
                }));

            _sut = new SearchRequestJob(_searchApiClientMock.Object, _searchRequestService.Object,_loggerMock.Object);
        }

        [Test]
        public async Task It_should_execute_the_job()
        {
            await _sut.Execute(_jobExecutionContextMock.Object);
            _searchApiClientMock.Verify(x => x.SearchAsync(It.IsAny<PersonSearchRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);

        }


    }
}