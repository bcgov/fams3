using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Health;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.Test.FakeMessages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;


namespace DynamicsAdapter.Web.Test.Health
{
    public class StatusReasonHealthCheckTest
    {
        private StatusReasonHealthCheck _sut;
        private readonly Mock<IStatusReasonService> _statusReasonServiceMock = new Mock<IStatusReasonService>();

        [Test]
        public async Task with_success_should_return_a_collection_of_search_request()
        {

            _statusReasonServiceMock.Setup(x => x.GetListAsync(CancellationToken.None))
                .Returns(Task.FromResult(FakeHttpMessageResponse.GetFakeInvalidReason()));
            _sut = new StatusReasonHealthCheck(_statusReasonServiceMock.Object);

            var result = await _sut.CheckHealthAsync(new HealthCheckContext() ,CancellationToken.None);
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
        }


        [Test]
        public async Task with_same_statuses_should_return_healthy()
        {

            _statusReasonServiceMock.Setup(x => x.GetListAsync(CancellationToken.None))
                .Returns(Task.FromResult(FakeHttpMessageResponse.GetFakeValidReason()));
            _sut = new StatusReasonHealthCheck(_statusReasonServiceMock.Object);

            var result = await _sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);
            Assert.AreEqual(HealthStatus.Healthy, result.Status);
        }


        [Test]
        public async Task with_empty_statuses_should_return_unhealthy()
        {

            _statusReasonServiceMock.Setup(x => x.GetListAsync(CancellationToken.None))
                .Returns(Task.FromResult(FakeHttpMessageResponse.GetFakeNullResult()));
            _sut = new StatusReasonHealthCheck(_statusReasonServiceMock.Object);

            var result = await _sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);
            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
        }
    }
}
