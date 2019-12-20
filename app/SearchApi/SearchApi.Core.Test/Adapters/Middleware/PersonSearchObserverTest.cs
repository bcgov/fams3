using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;
using BcGov.Fams3.SearchApi.Core.Adapters.Middleware;

namespace SearchApi.Core.Test.Adapters.Middleware
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
            public Person Person { get; set; }
        }

        public class FakePerson : Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public IEnumerable<PersonalIdentifier> Identifiers { get; }
            public IEnumerable<Address> Addresses { get; set; }

            public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
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
                Person = new FakePerson()
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
                .Any(x => x.Context.Message.Cause == "Test Exception"));
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