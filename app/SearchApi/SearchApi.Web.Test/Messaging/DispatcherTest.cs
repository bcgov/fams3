using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.Redis;
using BcGov.Fams3.Redis.Model;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Core.Configuration;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch;
using SearchApi.Web.DeepSearch.Schema;
using SearchApi.Web.Messaging;
using SearchApi.Web.Test.Utils;

namespace SearchApi.Web.Test.Messaging
{
    [TestFixture]
    public class DispatcherTest
    {

        private Dispatcher sut;

        private Mock<ISendEndpointProvider> sendEndpointProviderMock;
        private Mock<ICacheService> _cacheServiceMock;
        private Mock<ILogger<IDispatcher>> _loggerMock;

        private Mock<IOptions<RabbitMqConfiguration>> rabbitMqConfigurationMock;

        private Mock<ISendEndpoint> sendEndpointMock;
        WaveSearchData wave;
        string dataPartner = "ICBC";
        string SearchRequestKey = "FirstTimeWave";
        string NotSearchRequestKey = "AnotherWave";
     

        [SetUp]
        public void SetUp()
        {

            sendEndpointProviderMock = new Mock<ISendEndpointProvider>();
            _cacheServiceMock = new Mock<ICacheService>();
            rabbitMqConfigurationMock = new Mock<IOptions<RabbitMqConfiguration>>();
            _loggerMock = new Mock<ILogger<IDispatcher>>();
            sendEndpointMock = new Mock<ISendEndpoint>();

            RabbitMqConfiguration rabbitMqConfiguration = new RabbitMqConfiguration();
            rabbitMqConfiguration.Host = "localhost";
            rabbitMqConfiguration.Port = 15672;
            rabbitMqConfiguration.Username = "username";
            rabbitMqConfiguration.Password = "password";

            _cacheServiceMock.Setup(x => x.Get($"deepsearch-{NotSearchRequestKey}-{dataPartner}"))
      .Returns(Task.FromResult(JsonConvert.SerializeObject(wave)));
            _cacheServiceMock.Setup(x => x.SaveRequest(It.IsAny<SearchRequest>()))
              .Returns(Task.CompletedTask);

            _cacheServiceMock.Setup(x => x.GetRequest(It.IsAny<string>()))
 .Returns(Task.FromResult(new SearchRequest { DataPartners = new List<DataPartner>() { new DataPartner { Completed = false, Name = "ICBC" } } }));
           

            _cacheServiceMock.Setup(x => x.Get($"deepsearch-{SearchRequestKey}-{dataPartner}"))
            .Returns(Task.FromResult(""));
            wave = new WaveSearchData
            {
                AllParameter = new List<Person>(),
                CurrentWave = 2,
                DataPartner = dataPartner,
                NewParameter = new List<Person>(),
                SearchRequestKey = SearchRequestKey,
                NumberOfRetries = 1,
                TimeBetweenRetries = 3
            };
            _cacheServiceMock.Setup(x => x.Get($"deepsearch-{NotSearchRequestKey}-{dataPartner}"))
            .Returns(Task.FromResult(JsonConvert.SerializeObject(wave)));

            sendEndpointProviderMock
                .Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                .Returns(Task.FromResult(sendEndpointMock.Object));
        

            rabbitMqConfigurationMock.Setup(x => x.Value).Returns(rabbitMqConfiguration);

            sendEndpointMock.Setup(x => x.Send<PersonSearchOrdered>(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(sendEndpointMock));

            sut = new Dispatcher(_loggerMock.Object,sendEndpointProviderMock.Object, rabbitMqConfigurationMock.Object, _cacheServiceMock.Object);

        }

        [Test]
        public async Task with1ProviderShouldSendTo1Queue()
        {

            await sut.Dispatch(new PersonSearchRequest(
                "firstName",
                "lastName",
                DateTime.Now,
                new List<PersonalIdentifier>(),
                new List<Address>(),
                new List<Phone>(),
                new List<Name>(),
                new List<RelatedPerson>(),
                new List<Employment>(),
                new List<DataProvider>
                {
                    new DataProvider()
                    {
                        Name = "TEST",
                        Completed = false,
                        NumberOfRetries = 3,
                        TimeBetweenRetries = 30
                    }
                },
                "SearchRequestKey"), Guid.NewGuid());


            sendEndpointMock.Verify(x => x.Send<PersonSearchOrdered>(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });

        }

        [Test]
        public async Task with5ProviderShouldSendTo5Queue()
        {

            await sut.Dispatch(new PersonSearchRequest(
                "firstName",
                "lastName",
                DateTime.Now,
                new List<PersonalIdentifier>(),
                new List<Address>(),
                new List<Phone>(),
                new List<Name>(),
                new List<RelatedPerson>(),
                new List<Employment>(),
                new List<DataProvider>
                {
                    new DataProvider() {Name = "ICBC", Completed = false, NumberOfRetries = 5, TimeBetweenRetries=10, SearchSpeedType = SearchSpeedType.Fast},
                    new DataProvider() {Name = "ICBC", Completed = false, NumberOfRetries = 3, TimeBetweenRetries=50, SearchSpeedType = SearchSpeedType.Fast},
                    new DataProvider() {Name = "ICBC", Completed= true, NumberOfRetries = 6, TimeBetweenRetries=70, SearchSpeedType = SearchSpeedType.Fast},
                    new DataProvider() {Name = "ICBC", Completed =false, NumberOfRetries = 3, TimeBetweenRetries=45, SearchSpeedType = SearchSpeedType.Slow},
                    new DataProvider() {Name = "ICBC", Completed=false, NumberOfRetries = 5, TimeBetweenRetries=34, SearchSpeedType = SearchSpeedType.Slow}
                },
                NotSearchRequestKey), Guid.NewGuid());

         

            sendEndpointMock.Verify(x => x.Send<PersonSearchOrdered>(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()),
                () => { return Times.Exactly(5); });
            _loggerMock.VerifyLog(LogLevel.Information, $"{NotSearchRequestKey} has an active wave", Times.Exactly(3));

            _loggerMock.VerifyLog(LogLevel.Information, $"{NotSearchRequestKey} Current Metadata Wave : {wave.CurrentWave}", Times.Exactly(3));
            wave.CurrentWave++;
            _loggerMock.VerifyLog(LogLevel.Information, $"{NotSearchRequestKey} New wave {wave.CurrentWave} saved", Times.Exactly(3));

        }
        [Test]
        public async Task with5ProviderShouldSendTo5QueueAndStartWave()
        {

            await sut.Dispatch(new PersonSearchRequest(
                "firstName",
                "lastName",
                DateTime.Now,
                new List<PersonalIdentifier>(),
                new List<Address>(),
                new List<Phone>(),
                new List<Name>(),
                new List<RelatedPerson>(),
                new List<Employment>(),
                new List<DataProvider>
                {
                    new DataProvider() {Name = "TEST1", Completed = false, NumberOfRetries = 5, TimeBetweenRetries=10},
                    new DataProvider() {Name = "TEST2", Completed = false, NumberOfRetries = 3, TimeBetweenRetries=50},
                    new DataProvider() {Name = "TEST3", Completed= true, NumberOfRetries = 6, TimeBetweenRetries=70},
                    new DataProvider() {Name = "TEST4", Completed =false, NumberOfRetries = 3, TimeBetweenRetries=45},
                    new DataProvider() {Name = "TEST5", Completed=false, NumberOfRetries = 5, TimeBetweenRetries=34}
                },
                SearchRequestKey), Guid.NewGuid());



            sendEndpointMock.Verify(x => x.Send<PersonSearchOrdered>(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()),
                () => { return Times.Exactly(5); });
            _loggerMock.VerifyLog(LogLevel.Information, $"{ SearchRequestKey} does not have active wave",Times.Exactly(5));
            _loggerMock.VerifyLog(LogLevel.Information, $"{ SearchRequestKey} saved", Times.Exactly(5));

        }

     
        [Test]
        public async Task with0ProviderShouldSendTo0Queue()
        {

            await sut.Dispatch(new PersonSearchRequest(
                "firstName",
                "lastName",
                DateTime.Now,
                new List<PersonalIdentifier>(),
                new List<Address>(),
                new List<Phone>(),
                new List<Name>(),
                new List<RelatedPerson>(),
                new List<Employment>(),
                new List<DataProvider>(), "SearchRequestKey"), Guid.NewGuid());


            sendEndpointMock.Verify(x => x.Send<PersonSearchOrdered>(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()),
                () => { return Times.Never(); });

        }

        [Test]
        public async Task withNullProviderShouldSendTo0Queue()
        {

            await sut.Dispatch(new PersonSearchRequest(
                "firstName",
                "lastName",
                DateTime.Now,
                new List<PersonalIdentifier>(),
                new List<Address>(),
                new List<Phone>(),
                new List<Name>(),
                new List<RelatedPerson>(),
                new List<Employment>(),
                null, "SearchRequestKey"), Guid.NewGuid());


            sendEndpointMock.Verify(x => x.Send<PersonSearchOrdered>(It.IsAny<PersonSearchOrdered>(), It.IsAny<CancellationToken>()),
                () => { return Times.Never(); });

        }

        [Test]
        public void withNullRequestShouldSendThrowArgumentNullException()
        {

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Dispatch(null, Guid.NewGuid()));

        }



        [Test]
        public void withDefaultRequestIdShouldSendThrowArgumentNullException()
        {

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Dispatch(new PersonSearchRequest(
                "firstName",
                "lastName",
                DateTime.Now,
                new List<PersonalIdentifier>(),
                new List<Address>(),
                new List<Phone>(),
                new List<Name>(),
                new List<RelatedPerson>(),
                new List<Employment>(),
                null, "SearchRequestKey"), new Guid()));

        }


    }
}