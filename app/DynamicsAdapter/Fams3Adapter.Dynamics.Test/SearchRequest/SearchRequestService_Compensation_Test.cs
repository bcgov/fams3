using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.CompensationClaim;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Employment;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_Compensation_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testCompensationId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region compensation IsCompensationDuplicated testcases
        [Test]
        public async Task with_non_duplicated_person_IsCompensationDuplicated_should_return_null()
        {
            var compensation = new CompensationClaimEntity()
            {
                ClaimNumber = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.IsCompensationDuplicated(compensation, CancellationToken.None);

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_claim_IsCompensationDuplicated_should_return_null()
        {
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<CompensationClaimEntity>(m => m.ClaimNumber == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var claim = new CompensationClaimEntity()
            {
                ClaimNumber = "notContain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.IsCompensationDuplicated(claim, CancellationToken.None);

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_claim_different_banking_IsCompensationDuplicated_return_null()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<CompensationClaimEntity>(m => m.ClaimNumber == "contains")))
                .Returns(Task.FromResult(_testCompensationId));

            _odataClientMock.Setup(x => x.For<SSG_Asset_WorkSafeBcClaim>(null)
                  .Key(It.Is<Guid>(m => m == _testCompensationId))
                  .Expand(x => x.BankingInformation)
                  .Expand(x => x.Employment)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Asset_WorkSafeBcClaim()
                  {
                      CompensationClaimId = originalGuid,
                      ClaimNumber = "original"
                  }));

            _duplicateServiceMock.Setup(x => x.Same(It.IsAny<BankingInformationEntity>(), It.IsAny<SSG_Asset_BankingInformation>()))
                    .Returns(Task.FromResult(false));

            var compensationClaim = new CompensationClaimEntity()
            {
                ClaimNumber = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.IsCompensationDuplicated(compensationClaim, CancellationToken.None);

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_claim_same_banking_different_employment_IsCompensationDuplicated_return_null()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<CompensationClaimEntity>(m => m.ClaimNumber == "contains")))
                .Returns(Task.FromResult(_testCompensationId));

            _odataClientMock.Setup(x => x.For<SSG_Asset_WorkSafeBcClaim>(null)
                  .Key(It.Is<Guid>(m => m == _testCompensationId))
                  .Expand(x => x.BankingInformation)
                  .Expand(x => x.Employment)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Asset_WorkSafeBcClaim()
                  {
                      CompensationClaimId = originalGuid,
                      ClaimNumber = "original"
                  }));

            _duplicateServiceMock.Setup(x => x.Same(It.IsAny<BankingInformationEntity>(), It.IsAny<SSG_Asset_BankingInformation>()))
                    .Returns(Task.FromResult(true));

            _duplicateServiceMock.Setup(x => x.Same(It.IsAny<CompensationClaimEntity>(), It.IsAny<SSG_Asset_WorkSafeBcClaim>()))
                    .Returns(Task.FromResult(false));

            var compensationClaim = new CompensationClaimEntity()
            {
                ClaimNumber = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.IsCompensationDuplicated(compensationClaim, CancellationToken.None);

            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_claim_same_banking_same_employment_IsCompensationDuplicated_return_data_with_children()
        {
            Guid originalClaimGuid = Guid.NewGuid();
            Guid originalEmploymentGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<CompensationClaimEntity>(m => m.ClaimNumber == "contains")))
                .Returns(Task.FromResult(_testCompensationId));

            _odataClientMock.Setup(x => x.For<SSG_Asset_WorkSafeBcClaim>(null)
                  .Key(It.Is<Guid>(m => m == _testCompensationId))
                  .Expand(x => x.BankingInformation)
                  .Expand(x => x.Employment)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Asset_WorkSafeBcClaim()
                  {
                      CompensationClaimId = originalClaimGuid,
                      ClaimNumber = "original",
                      Employment = new SSG_Employment()
                      {
                          EmploymentId = originalEmploymentGuid
                      }
                  }));

            _duplicateServiceMock.Setup(x => x.Same(It.IsAny<object>(), It.IsAny<object>()))
                    .Returns(Task.FromResult(true));

            _odataClientMock.Setup(x => x.For<SSG_Employment>(null)
                  .Key(It.Is<Guid>(m => m == originalEmploymentGuid))
                  .Expand(x => x.SSG_EmploymentContacts)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Employment()
                  {
                      EmploymentId = originalEmploymentGuid
                  }));

            var compensationClaim = new CompensationClaimEntity()
            {
                ClaimNumber = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                },
                Employment = new SSG_Employment()
                {
                    EmploymentId = originalEmploymentGuid
                }
            };
            var result = await _sut.IsCompensationDuplicated(compensationClaim, CancellationToken.None);

            Assert.AreEqual(originalClaimGuid, result.CompensationClaimId);
            Assert.AreEqual(originalEmploymentGuid, result.Employment.EmploymentId);
            Assert.AreEqual(true, result.IsDuplicated);
            Assert.AreEqual(true, result.Employment.IsDuplicated);
        }

        [Test]
        public async Task with_claimInfo_CreateCompensationClaim_return_new_SSG_Asset_WorkSafeBcClaim()
        {
            _odataClientMock.Setup(x => x.For<SSG_Asset_WorkSafeBcClaim>(null)
                .Set(It.IsAny<CompensationClaimEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_WorkSafeBcClaim()
                {
                    CompensationClaimId = _testCompensationId
                })
                );

            var compensationClaim = new CompensationClaimEntity()
            {
                ClaimNumber = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateCompensationClaim(compensationClaim, CancellationToken.None);

            Assert.AreEqual(_testCompensationId, result.CompensationClaimId);

        }
        #endregion


    }
}
