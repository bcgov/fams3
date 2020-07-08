using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.Duplicate;
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
    public class SearchRequestService_AssetOwner_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testAssetOwnerId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region assetowner testcases
        [Test]
        public async Task with_non_duplicated_vehicle_CreateAssetOwner_should_return_new_SSG_AssetOwner()
        {
            _odataClientMock.Setup(x => x.For<SSG_AssetOwner>(null).Set(It.Is<AssetOwnerEntity>(x => x.FirstName == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_AssetOwner()
                {
                    AssetOwnerId = _testAssetOwnerId
                })
                );
            var o = new AssetOwnerEntity()
            {
                FirstName = "normal",
                Vehicle = new SSG_Asset_Vehicle() { IsDuplicated = false }
            };
            var result = await _sut.CreateAssetOwner(o, CancellationToken.None);

            Assert.AreEqual(_testAssetOwnerId, result.AssetOwnerId);
        }

        [Test]
        public async Task with_duplicated_vehicle_not_contains_same_assetowner_should_return_new_SSG_AssetOwner()
        {
            _odataClientMock.Setup(x => x.For<SSG_AssetOwner>(null).Set(It.Is<AssetOwnerEntity>(x => x.FirstName == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_AssetOwner()
                 {
                     AssetOwnerId = _testAssetOwnerId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Asset_Vehicle>(m => m.VehicleId == _testId), It.Is<AssetOwnerEntity>(m => m.FirstName == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var o = new AssetOwnerEntity()
            {
                FirstName = "notContain",
                Vehicle = new SSG_Asset_Vehicle() { IsDuplicated = true }
            };

            var result = await _sut.CreateAssetOwner(o, CancellationToken.None);

            Assert.AreEqual(_testAssetOwnerId, result.AssetOwnerId);
        }

        [Test]
        public async Task with_duplicated_vehicle_contains_same_assetOwner_should_return_original_assetowner_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Asset_Vehicle>(m => m.VehicleId == _testId), It.Is<AssetOwnerEntity>(m => m.FirstName == "contains")))
                .Returns(Task.FromResult(originalGuid));

            var o = new AssetOwnerEntity()
            {
                FirstName = "contains",
                Vehicle = new SSG_Asset_Vehicle()
                {
                    VehicleId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateAssetOwner(o, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.AssetOwnerId);
        }
        #endregion


    }
}
