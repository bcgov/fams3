using Moq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using DynamicsAdapter.Web.SearchRequest;
using Quartz;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class SearchRequestJobTest
    {
     
        private readonly Mock<ILogger<SearchRequestJob>> _loggerMock = new Mock<ILogger<SearchRequestJob>>();
        private readonly Mock<IJobExecutionContext> _jobExecutionContextMock = new Mock<IJobExecutionContext>();

        private SearchRequestJob _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SearchRequestJob(_loggerMock.Object);
        }

        [Test]
        public void It_should_execute_the_job()
        {
            _sut.Execute(_jobExecutionContextMock.Object);

        }


    }
}