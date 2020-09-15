using BcGov.Fams3.Redis;
using BcGov.Fams3.SearchApi.Contracts.Person;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SearchApi.Web.Configuration;
using SearchApi.Web.Controllers;
using SearchApi.Web.DeepSearch;
using SearchApi.Web.DeepSearch.Schema;
using SearchApi.Web.Test.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchApi.Web.Test.DeepSearch
{
    public class DeepSearchServiceTest
    {

        private DeepSearchService _sut;
        private Mock<ILogger<DeepSearchService>> _loggerMock;
      
        private Mock<ICacheService> _cacheServiceMock;
        private Mock<IOptions<DeepSearchOptions>> _deepSearchOptionsMock;
        WaveMetaData wave;
        string dataPartner = "ICBC";
        string SearchRequestKey = "FirstTimeWave";
        string NotSearchRequestKey = "AnotherWave";
        PersonSearchRequest person;
        [SetUp]
        public void SetUp()
        {
            _loggerMock = LoggerUtils.LoggerMock<DeepSearchService>();
            _deepSearchOptionsMock = new Mock<IOptions<DeepSearchOptions>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _cacheServiceMock.Setup(x => x.Get($"deepsearch-{SearchRequestKey}-{dataPartner}"))
                .Returns(Task.FromResult(""));
            _deepSearchOptionsMock.Setup(x => x.Value).Returns(new DeepSearchOptions
            {
                MaxWaveCount = 5
            });
            wave = new WaveMetaData
            {
                AllParameter = new List<Person>(),
                CurrentWave = 2,
                DataPartner = dataPartner,
                NewParameter = new List<Person>(),
                SearchRequestKey = SearchRequestKey
            };
            _cacheServiceMock.Setup(x => x.Get($"deepsearch-{NotSearchRequestKey}-{dataPartner}"))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(wave)));
            person = new PersonSearchRequest(
                "firstname", 
                "lastname", 
                DateTime.Now, 
                new List<PersonalIdentifier>(), 
                new List<Address>(), 
                new List<Phone>(), 
                new List<Name>(), 
                new List<RelatedPerson>(), 
                new List<Employment>(), 
                new List<DataProvider>(), 
                SearchRequestKey);

        }

        [Test]
        public async Task should_save_in_cache_if_first_wave()
        {
           
            _sut = new DeepSearchService(_cacheServiceMock.Object, _loggerMock.Object, _deepSearchOptionsMock.Object);
            await _sut.SaveRequest(person, dataPartner);
            _loggerMock.VerifyLog(LogLevel.Debug, $"{ person.SearchRequestKey} does not have active wave");
            _loggerMock.VerifyLog(LogLevel.Debug, $"{ person.SearchRequestKey} saved");


        }
        [Test]
        public async Task if_existing_should_update_wave_count()
        {
            _sut = new DeepSearchService(_cacheServiceMock.Object, _loggerMock.Object, _deepSearchOptionsMock.Object);
            person.SearchRequestKey = NotSearchRequestKey;

           
            await _sut.SaveRequest(person, dataPartner);
            _loggerMock.VerifyLog(LogLevel.Debug, $"{person.SearchRequestKey} has an active wave");

            _loggerMock.VerifyLog(LogLevel.Debug, $"{person.SearchRequestKey} Current Metadata Wave : {wave.CurrentWave}");
            _loggerMock.VerifyLog(LogLevel.Debug, $"{person.SearchRequestKey} New wave {wave.CurrentWave++} saved");


        }

    }
}
