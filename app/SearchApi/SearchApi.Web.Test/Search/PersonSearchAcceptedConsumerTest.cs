using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchApi.Core.Test.Fake;
using SearchApi.Web.Notifications;
using SearchApi.Web.Search;
using SearchApi.Web.Test.Utils;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;

namespace SearchApi.Web.Test.Search
{
    public class PersonSearchAcceptedConsumerTest
    {
        private InMemoryTestHarness _harness;
        private ConsumerTestHarness<PersonSearchAcceptedConsumer> _sut;

        private Mock<ILogger<PersonSearchAcceptedConsumer>> _loggerMock;
        private Mock<ISearchApiNotifier<PersonSearchAdapterEvent>> _searchApiNotifierMock;

        private string _requestKey;
      

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _loggerMock = LoggerUtils.LoggerMock<PersonSearchAcceptedConsumer>();
            _searchApiNotifierMock = new Mock<ISearchApiNotifier<PersonSearchAdapterEvent>>();
            _harness = new InMemoryTestHarness();
            _requestKey = "111111_222222";

            var fakePersonSearchStatus = new FakePersonSearchAccepted
            {
                SearchRequestKey = _requestKey,
                TimeStamp = DateTime.Now
            };


            _sut = _harness.Consumer(() => new PersonSearchAcceptedConsumer(_searchApiNotifierMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<PersonSearchAccepted>(fakePersonSearchStatus) ;

        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_initial_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Consumed.Select<PersonSearchAccepted>().Any());
            _searchApiNotifierMock.Verify(x => x.NotifyEventAsync(It.Is<string>(x => x == _requestKey), It.IsAny<PersonSearchAccepted>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
