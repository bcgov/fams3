using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Contracts;
using SearchApi.Web.Notifications;
using SearchApi.Web.Search;
using SearchApi.Web.Test.Utils;

namespace SearchApi.Web.Test.Search
{
    public class MatchFoundConsumerTest
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<MatchFoundConsumer> _sut;

        private Mock<ILogger<MatchFoundConsumer>> _loggerMock;
        private Mock<ISearchApiNotifier> _searchApiNotifierMock;

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _loggerMock = LoggerUtils.LoggerMock<MatchFoundConsumer>();
            _searchApiNotifierMock = new Mock<ISearchApiNotifier>();
            _harness = new InMemoryTestHarness();
            _sut = _harness.Consumer(() => new MatchFoundConsumer(_searchApiNotifierMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<MatchFound>(new
            {
                FirstName = "firstName",
                LastName = "lastName",
                DateOfBirth = new DateTime(2001, 1, 1)
            });

        }


        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Published.Select<MatchFound>().Any());
        }

        [Test]
        public void Should_receive_the_message_type_a()
        {
            Assert.IsTrue(_harness.Consumed.Select<MatchFound>().Any());
        }



    }
}