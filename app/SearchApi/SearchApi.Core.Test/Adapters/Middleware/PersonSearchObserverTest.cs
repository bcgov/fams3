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
using SearchApi.Core.Person.Contracts;

namespace SearchAdapter.ICBC.Test.Adapters.Middleware
{

    public class PersonSearchObserverTest
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<FakeFailureConsumer> _fakeConsumerTestHarness;
        private Mock<ILogger<PersonSearchObserver>> _personSearchObserver;
        private Mock<IOptions<ProviderProfileOptions>> _providerProfileOptiosnMock;

        public class PersonSearchOrderedTest : PersonSearchOrdered
        {
            public Guid SearchRequestId { get; set; }
            public DateTime TimeStamp { get; set; }
            public ExecuteSearch ExecuteSearch { get; set; }
        }

        public class ExecuteSearchTest : ExecuteSearch
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public IEnumerable<PersonalIdentifier> Identifiers { get; }
        }

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

            await _harness.BusControl.Publish<PersonSearchOrdered>(new PersonSearchOrderedTest()
            {
                SearchRequestId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                ExecuteSearch = new ExecuteSearchTest()
                {
                    FirstName = "",
                    LastName = "lastName",
                    DateOfBirth = new DateTime(2001, 1, 1)
                }
            });
        }

        [Test]
        public void With_failure_should_send_personSearchFailureEvent()
        {
            Assert.IsTrue(_harness.Published.Select<PersonSearchFailed>()
                .Any(x => x.Context.Message.Cause.Message == "Test Exception"));
        }


        public class FakeFailureConsumer : IConsumer<PersonSearchOrdered>
        {
            public Task Consume(ConsumeContext<PersonSearchOrdered> context)
            {
                throw new Exception("Test Exception");
            }
        }

    }
}