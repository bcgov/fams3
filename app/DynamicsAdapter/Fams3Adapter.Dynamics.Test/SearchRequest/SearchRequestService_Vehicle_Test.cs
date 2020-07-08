using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_Vehicle_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testVehicleId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region vehicle testcases
        [Test]
        public async Task with_non_duplicated_person_CreateVehicle_should_return_new_SSG_Vehicle()
        {
            _odataClientMock.Setup(x => x.For<SSG_Asset_Vehicle>(null).Set(It.Is<VehicleEntity>(x => x.Vin == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_Vehicle()
                {
                    VehicleId = _testVehicleId
                })
                );
            var v = new VehicleEntity()
            {
                Vin = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.CreateVehicle(v, CancellationToken.None);

            Assert.AreEqual(_testVehicleId, result.VehicleId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_vehicle_should_return_new_SSG_Asset_Vehicle()
        {
            _odataClientMock.Setup(x => x.For<SSG_Asset_Vehicle>(null).Set(It.Is<VehicleEntity>(x => x.Vin == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Asset_Vehicle()
                 {
                     VehicleId = _testVehicleId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<VehicleEntity>(m => m.Vin == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var v = new VehicleEntity()
            {
                Vin = "notContain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateVehicle(v, CancellationToken.None);

            Assert.AreEqual(_testVehicleId, result.VehicleId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_vehicle_should_return_original_vehicle_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<VehicleEntity>(m => m.Vin == "contains")))
                .Returns(Task.FromResult(_testVehicleId));

            _odataClientMock.Setup(x => x.For<SSG_Asset_Vehicle>(null)
                  .Key(It.Is<Guid>(m => m == _testVehicleId))
                  .Expand(x => x.SSG_AssetOwners)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Asset_Vehicle()
                  {
                      VehicleId = originalGuid,
                      Vin = "original"
                  }));

            var v = new VehicleEntity()
            {
                Vin = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateVehicle(v, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.VehicleId);
            Assert.AreEqual("original", result.Vin);
        }
        #endregion


    }
}
