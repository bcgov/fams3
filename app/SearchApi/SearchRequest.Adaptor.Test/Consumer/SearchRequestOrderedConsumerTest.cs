using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchRequestAdaptor.Consumer;
using SearchRequestAdaptor.Notifier;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Test.Consumer
{
    public class SearchRequestOrderedConsumerTet
    {
        private InMemoryTestHarness _harness;
        private Mock<ILogger<SearchRequestOrderedConsumer>> _loggerMock;
        private Mock<ISearchRequestNotifier<SearchRequestEvent>> _searchRequestNotifierMock;


        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _loggerMock = new Mock<ILogger<SearchRequestOrderedConsumer>>();
            _searchRequestNotifierMock = new Mock<ISearchRequestNotifier<SearchRequestEvent>>();
            _harness = new InMemoryTestHarness();
           
            _harness.Consumer(() => new SearchRequestOrderedConsumer(_searchRequestNotifierMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<SearchRequestOrdered>(new FakeSearchRequestOrdered()
            {
                Action = BcGov.Fams3.SearchApi.Contracts.Person.RequestAction.NEW,
                SearchRequestKey ="key",
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
            Assert.IsTrue(_harness.Consumed.Select<SearchRequestOrdered>().Any());
            _searchRequestNotifierMock.Verify(x => x.NotifySearchRequestEventAsync(It.Is<string>(x => x == "key"), It.IsAny<SearchRequestOrdered>(), It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
