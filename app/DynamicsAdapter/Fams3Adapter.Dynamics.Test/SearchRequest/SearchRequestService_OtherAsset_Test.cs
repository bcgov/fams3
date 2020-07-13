using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.RelatedPerson;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_OtherAsset_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private Guid _testOtherAssetId;
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region other asset testcases
        [Test]
        public async Task with_non_duplicated_person_CreateOtherAsset_should_return_new_SSG_Asset_Other()
        {
            _testOtherAssetId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Asset_Other>(null).Set(It.Is<AssetOtherEntity>(x => x.Description == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_Other()
                {
                    Description = "normal",
                    AssetOtherId = _testOtherAssetId
                })
                );
            var otherAsset = new SSG_Asset_Other()
            {
                Description = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };

            var result = await _sut.CreateOtherAsset(otherAsset, CancellationToken.None);

            Assert.AreEqual("normal", result.Description);
            Assert.AreEqual(_testOtherAssetId, result.AssetOtherId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_otherAsset_should_return_new_SSG_Asset_Other()
        {
            _testOtherAssetId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Asset_Other>(null).Set(It.Is<AssetOtherEntity>(x => x.Description == "notContain"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_Other()
                {
                    Description = "notContain",
                    AssetOtherId = _testOtherAssetId
                })
                );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<AssetOtherEntity>(m => m.Description == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var otherAsset = new SSG_Asset_Other()
            {
                Description = "notContain",
                Person = new SSG_Person() { IsDuplicated = true }
            };
            var result = await _sut.CreateOtherAsset(otherAsset, CancellationToken.None);

            Assert.AreEqual("notContain", result.Description);
            Assert.AreEqual(_testOtherAssetId, result.AssetOtherId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_otherAsset_should_return_original_otherAsset_guid()
        {
            _testOtherAssetId = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<AssetOtherEntity>(m => m.Description == "contain")))
                .Returns(Task.FromResult(_testOtherAssetId));

            var otherAsset = new AssetOtherEntity()
            {
                Description = "contain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateOtherAsset(otherAsset, CancellationToken.None);

            Assert.AreEqual(_testOtherAssetId, result.AssetOtherId);
        }
        #endregion


    }
}
