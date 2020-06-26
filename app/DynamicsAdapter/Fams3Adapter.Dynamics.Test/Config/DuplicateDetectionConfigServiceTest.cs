using Fams3Adapter.Dynamics.Config;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.Config
{
    public class DuplicateDetectionConfigServiceTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();

        private DuplicateDetectionConfigService _sut;

        [SetUp]
        public void SetUp()
        {

            odataClientMock.Setup(x => x.For<SSG_DuplicateDetectionConfig>(null)                
                .FindEntriesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<SSG_DuplicateDetectionConfig>>(new List<SSG_DuplicateDetectionConfig>()
                {
                     new SSG_DuplicateDetectionConfig()
                     {
                         EntityName = "Person",
                         DuplicateFields = "ssg_firstName|ssg_lastName|ssg_dateofBirth|ssg_dateOfDeath"
                     }
                }));

            _sut = new DuplicateDetectionConfigService(odataClientMock.Object);

        }


        [Test]
        public async Task GetDuplicateDetectionConfig_should_return_configs()
        {
            IEnumerable<SSG_DuplicateDetectionConfig> configs = await _sut.GetDuplicateDetectionConfig(CancellationToken.None);
            SSG_DuplicateDetectionConfig[] array = configs.ToArray<SSG_DuplicateDetectionConfig>();
            Assert.AreEqual("Person", array[0].EntityName);
            Assert.AreEqual("ssg_firstName|ssg_lastName|ssg_dateofBirth|ssg_dateOfDeath", array[0].DuplicateFields);
        }
    }
}
