using Fams3Adapter.Dynamics.DataProvider;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.DataPartners
{
    public class DataPartnerServiceTest
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<ILogger<DataProviderService>> _loggerMock = new Mock<ILogger<DataProviderService>>();
        private DataProviderService _sut;
        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _loggerMock  = new Mock<ILogger<DataProviderService>>();

            _odataClientMock.Setup( x => x.For<SSG_DataProvider>(null).FindEntriesAsync(It.IsAny<CancellationToken>())).Returns (
                Task.FromResult<IEnumerable<SSG_DataProvider>>(

                  new List<SSG_DataProvider>()
                  {
                      new SSG_DataProvider()
                      {
                        AdaptorName = "ICBC",
                        NumberOfDaysToRetry =5,
                        SearchSpeed = "Fast",
                        TimeBetweenRetries = 60,
                        NumberOfRetries = 3

                      }
                  }
               ));

            _odataClientMock.Setup(x => x.For<SSG_SearchapiRequestDataProvider>(null)
            .Key(It.IsAny<Guid>())
            .Set(It.IsAny<SSG_SearchapiRequestDataProvider>())
            .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult( new SSG_SearchapiRequestDataProvider {
                    AdaptorName = "ICBC",
                    NumberOfDaysToRetry = 5,
                    TimeBetweenRetries = 60,
                    NumberOfRetries = 3
                }));

            _sut = new DataProviderService(_loggerMock.Object, _odataClientMock.Object);
        }

        [Test]
        public async Task should_get_a_list_of_providers()
        {
            var result = await _sut.GetAllDataProviders(CancellationToken.None);
            _odataClientMock.Verify(x => x.For<SSG_DataProvider>(null).FindEntriesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("ICBC", result.FirstOrDefault().AdaptorName);
        }

        [Test]
        public async Task update_serach_api_request_correctly()
        {
            var result = await _sut.UpdateSearchRequestApiProvider(new SSG_SearchapiRequestDataProvider { } ,CancellationToken.None);
            _odataClientMock.Verify(x => x.For<SSG_SearchapiRequestDataProvider>(null)
            .Key (It.IsAny<Guid>())
            .Set(It.IsAny<SSG_SearchapiRequestDataProvider>())
            .UpdateEntryAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.IsInstanceOf<SSG_SearchapiRequestDataProvider>(result);
            Assert.AreEqual(5, result.NumberOfDaysToRetry);
            Assert.AreEqual("ICBC", result.AdaptorName);
        }

    }
}
