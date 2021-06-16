using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Vehicle;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_Insurance_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testInsuranceId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        #region insurance testcases
        [Test]
        public async Task with_non_duplicated_person_CreateInsuranceClaim_should_return_new_SSG_ICBCClaim()
        {
            _odataClientMock.Setup(x => x.For<SSG_Asset_ICBCClaim>(null).Set(It.Is<ICBCClaimEntity>(x => x.ClaimNumber == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_ICBCClaim()
                {
                    ICBCClaimId = _testInsuranceId
                })
                );
            var claim = new ICBCClaimEntity()
            {
                ClaimNumber = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.CreateInsuranceClaim(claim, CancellationToken.None);

            Assert.AreEqual(_testInsuranceId, result.ICBCClaimId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_insurance_should_return_new_SSG_ICBCClaim()
        {
            _odataClientMock.Setup(x => x.For<SSG_Asset_ICBCClaim>(null).Set(It.Is<ICBCClaimEntity>(x => x.ClaimNumber == "notContain"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_ICBCClaim()
                {
                    ICBCClaimId = _testInsuranceId
                })
                );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<ICBCClaimEntity>(m => m.ClaimNumber == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var claim = new ICBCClaimEntity()
            {
                ClaimNumber = "notContain",
                Person = new SSG_Person() { IsDuplicated = true, PersonId=_testId }
            };
            var result = await _sut.CreateInsuranceClaim(claim, CancellationToken.None);

            Assert.AreEqual(_testInsuranceId, result.ICBCClaimId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_insurance_should_return_original_Insurance_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<ICBCClaimEntity>(m => m.ClaimNumber == "contains")))
                .Returns(Task.FromResult(_testInsuranceId));

            _odataClientMock.Setup(x => x.For<SSG_Asset_ICBCClaim>(null)
                  .Key(It.Is<Guid>(m => m == _testInsuranceId))
                  .Expand(x => x.SSG_InvolvedParties)
                  .Expand(x => x.SSG_SimplePhoneNumbers)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Asset_ICBCClaim()
                  {
                      ICBCClaimId = originalGuid,
                      ClaimNumber = "original"
                  }));

            var claim = new ICBCClaimEntity()
            {
                ClaimNumber = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateInsuranceClaim(claim, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.ICBCClaimId);
            Assert.AreEqual("original", result.ClaimNumber);
        }
        #endregion


    }
}
