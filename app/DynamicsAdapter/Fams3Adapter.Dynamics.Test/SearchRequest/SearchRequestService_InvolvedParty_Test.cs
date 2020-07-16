using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.InsuranceClaim;
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
    public class SearchRequestService_InvolvedParty_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testInvolvedPartyId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region involved party testcases
        [Test]
        public async Task with_non_duplicated_insurance_CreateInvolvedParty_should_return_new_SSG_InvolvedParty()
        {
            _odataClientMock.Setup(x => x.For<SSG_InvolvedParty>(null).Set(It.Is<InvolvedPartyEntity>(x => x.FirstName == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_InvolvedParty()
                {
                    InvolvedPartyId = _testInvolvedPartyId
                })
                );
            var party = new InvolvedPartyEntity()
            {
                FirstName = "normal",
                SSG_Asset_ICBCClaim = new SSG_Asset_ICBCClaim() { IsDuplicated = false }
            };
            var result = await _sut.CreateInvolvedParty(party, CancellationToken.None);

            Assert.AreEqual(_testInvolvedPartyId, result.InvolvedPartyId);
        }

        [Test]
        public async Task with_duplicated_insurance_not_contains_same_party_CreateInvolvedParty_should_return_new_SSG_InvolvedParty()
        {
            _odataClientMock.Setup(x => x.For<SSG_InvolvedParty>(null).Set(It.Is<InvolvedPartyEntity>(x => x.FirstName == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_InvolvedParty()
                 {
                     InvolvedPartyId = _testInvolvedPartyId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Asset_ICBCClaim>(m => m.ICBCClaimId == _testId), It.Is<InvolvedPartyEntity>(m => m.FirstName == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var party = new InvolvedPartyEntity()
            {
                FirstName = "notContain",
                SSG_Asset_ICBCClaim = new SSG_Asset_ICBCClaim() { IsDuplicated = true }
            };

            var result = await _sut.CreateInvolvedParty(party, CancellationToken.None);

            Assert.AreEqual(_testInvolvedPartyId, result.InvolvedPartyId);
        }

        [Test]
        public async Task with_duplicated_insurance_contains_same_party_CreateInvolvedParty_should_return_original_involvedparty_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Asset_ICBCClaim>(m => m.ICBCClaimId == _testId), It.Is<InvolvedPartyEntity>(m => m.FirstName == "contains")))
                .Returns(Task.FromResult(originalGuid));

            var party = new InvolvedPartyEntity()
            {
                FirstName = "contains",
                SSG_Asset_ICBCClaim = new SSG_Asset_ICBCClaim()
                {
                    ICBCClaimId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateInvolvedParty(party, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.InvolvedPartyId);
        }
        #endregion


    }
}
