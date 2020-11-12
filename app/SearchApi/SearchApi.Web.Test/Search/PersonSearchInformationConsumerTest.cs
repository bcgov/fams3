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
using BcGov.Fams3.SearchApi.Contracts.Person;

namespace SearchApi.Web.Test.Search
{
    public class PersonSearchInformationConsumerTest
    {
        private InMemoryTestHarness _harness;
        private Mock<ILogger<PersonSearchInformationConsumer>> _loggerMock;
        private Mock<ISearchApiNotifier<PersonSearchAdapterEvent>> _searchApiNotifierMock;

        private string _requestKey;
      

        [OneTimeSetUp]
        public async Task A_consumer_is_being_tested()
        {
            _loggerMock = LoggerUtils.LoggerMock<PersonSearchInformationConsumer>();
            _searchApiNotifierMock = new Mock<ISearchApiNotifier<PersonSearchAdapterEvent>>();
            _harness = new InMemoryTestHarness();
            _requestKey = "111111_000000";

            var fakePersonSearchStatus = new FakePersonSearchInformation
            {
                SearchRequestKey = _requestKey,
                TimeStamp = DateTime.Now,
                Message = "code : fake person search information recieved",
                ProviderProfile = new FakeProviderProfile { Name ="JCA", SearchSpeedType=SearchSpeedType.Slow }
            };


            _harness.Consumer(() => new PersonSearchInformationConsumer(_searchApiNotifierMock.Object, _loggerMock.Object));

            await _harness.Start();

            await _harness.BusControl.Publish<PersonSearchInformation>(fakePersonSearchStatus) ;

        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _harness.Stop();
        }

        [Test]
        public void Should_send_the_message_to_the_consumer()
        {
            Assert.IsTrue(_harness.Consumed.Select<PersonSearchInformation>().Any());
            _searchApiNotifierMock.Verify(x => x.NotifyEventAsync(It.Is<string>(x => x == _requestKey), It.IsAny<PersonSearchInformation>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
