using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using OpenTracing.Mock;
using SearchApi.Core.Adapters.Configuration;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Middleware;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.Test.Adapters.Middleware
{

    public class PersonSearchObserverTest
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<FakeFailureConsumer> _fakeConsumerTestHarness;
        private Mock<ILogger<PersonSearchObserver>> _personSearchObserver;
        private Mock<IOptions<ProviderProfileOptions>> _providerProfileOptiosnMock;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _harness = new InMemoryTestHarness();
            _fakeConsumerTestHarness = _harness.Consumer(() => new FakeFailureConsumer());

            _personSearchObserver = new Mock<ILogger<PersonSearchObserver>>();
            _providerProfileOptiosnMock = new Mock<IOptions<ProviderProfileOptions>>();

            _providerProfileOptiosnMock.Setup(x => x.Value).Returns(new ProviderProfileOptions()
            {
                Name = "Adapter Tester"
            });

            await _harness.Start();

            _harness.Bus.ConnectConsumeMessageObserver(new PersonSearchObserver(_providerProfileOptiosnMock.Object,
                _personSearchObserver.Object));

            await _harness.InputQueueSendEndpoint.Send<ExecuteSearch>(new
            {
                Id = Guid.NewGuid(),
                FirstName = "firstName",
                LastName = "lastName",
                DateOfBirth = new DateTime(2001, 1, 1)
            });
        }

        [Test]
        public void With_failure_should_send_personSearchFailureEvent()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchFailed>()
                .Any(x => x.Context.Message.Cause.Message == "Test Exception"));
        }


        public class FakeFailureConsumer : IConsumer<ExecuteSearch>
        {
            public Task Consume(ConsumeContext<ExecuteSearch> context)
            {
                throw new Exception("Test Exception");
            }
        }

    }
}