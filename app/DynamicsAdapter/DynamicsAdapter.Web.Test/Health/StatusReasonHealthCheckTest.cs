using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Health;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.Services.Dynamics.Model;
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

        [SetUp]
        public void SetUp()
        {

            _statusReasonServiceMock.Setup(x => x.GetListAsync(CancellationToken.None))
                .Returns(Task.FromResult(FakeHttpMessageResponse.GetFakeReason()));

            _sut = new StatusReasonHealthCheck(_statusReasonServiceMock.Object);

        }

        [Test]
        public async Task with_success_should_return_a_collection_of_search_request()
        {
            var result = await _sut.CheckHealthAsync(new HealthCheckContext() ,CancellationToken.None);

            Assert.AreEqual(HealthStatus.Unhealthy, result.Status);


        }
    }
}
