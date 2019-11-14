using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Health;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.Services.Dynamics.Model;
using DynamicsAdapter.Web.Test.FakeMessages;
using Moq;
using NUnit.Framework;


namespace DynamicsAdapter.Web.Test.Health
{
    public class StatusReasonHealthCheckTest
    {
        private StatusReasonHealthCheck _sut;
        private readonly Mock<StatusReasonService> _statusReasonServiceMock = new Mock<StatusReasonService>();

        [SetUp]
        public void SetUp()
        {

            _statusReasonServiceMock.Setup(x => x.GetListAsync(new CancellationToken()))
                .Returns(Task.FromResult(FakeHttpMessageResponse.GetFakeReason()));

            _sut = new StatusReasonHealthCheck(_statusReasonServiceMock.Object);

        }
    }
}
