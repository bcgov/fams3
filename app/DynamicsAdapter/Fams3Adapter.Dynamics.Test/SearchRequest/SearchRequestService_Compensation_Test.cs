using Fams3Adapter.Dynamics.Address;
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
using System.Collections.Generic;
using System.Linq.Expressions;
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
        private readonly Guid _testBankInfoId = Guid.Parse("6AE00FE6-9909-EA11-1111-00505683FBF4");
        private readonly Guid _testEmployId = Guid.Parse("77700FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
            _odataClientMock.Setup(x => x.For<SSG_Country>(null)
              .Filter(It.IsAny<Expression<Func<SSG_Country, bool>>>())
              .FindEntryAsync(It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<SSG_Country>(new SSG_Country()
              {
                  CountryId = Guid.NewGuid(),
                  Name = "Canada"
              }));

            _odataClientMock.Setup(x => x.For<SSG_CountrySubdivision>(null)
                .Filter(It.IsAny<Expression<Func<SSG_CountrySubdivision, bool>>>())
                .FindEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<SSG_CountrySubdivision>(new SSG_CountrySubdivision()
                {
                    CountrySubdivisionId = Guid.NewGuid(),
                    Name = "British Columbia"
                }));

            _odataClientMock.Setup(x => x.For<SSG_Asset_WorkSafeBcClaim>(null)
                 .Set(It.IsAny<CompensationClaimEntity>())
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Asset_WorkSafeBcClaim()
                 {
                     CompensationClaimId = _testCompensationId
                 })
                 );

            _odataClientMock.Setup(x => x.For<SSG_Asset_BankingInformation>(null)
                .Set(It.IsAny<BankingInformationEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_BankingInformation()
                {
                    BankingInformationId = _testBankInfoId
                })
                );

            _odataClientMock.Setup(x => x.For<SSG_Employment>(null)
                .Set(It.IsAny<EmploymentEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Employment()
                {
                    EmploymentId = _testEmployId
                })
                );

            _odataClientMock.Setup(x => x.For<SSG_EmploymentContact>(null)
                .Set(It.IsAny<EmploymentContactEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_EmploymentContact()
                {
                    PhoneNumber = "123"
                })
                );
        }

        #region compensation IsCompensationDuplicated testcases
        [Test]
        public async Task with_non_duplicated_person_CreateCompensationClaim_should_create_new_Claim()
        {
            var compensation = new CompensationClaimEntity()
            {
                ClaimNumber = "normal",
                Person = new SSG_Person() { IsDuplicated = false },
                BankInformationEntity = new BankingInformationEntity() { AccountNumber="newbank"},
                EmploymentEntity = new EmploymentEntity() { BusinessName="newEmployer"}
            };
            var result = await _sut.CreateCompensationClaim(compensation, CancellationToken.None);
            
            Assert.AreEqual(_testCompensationId, result.CompensationClaimId);
            _odataClientMock.Verify(x => x.For<SSG_Employment>(It.IsAny<string>())
                            .Set(It.IsAny<EmploymentEntity>())
                            .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
            _odataClientMock.Verify(x => x.For<SSG_Asset_BankingInformation>(It.IsAny<string>())
                .Set(It.IsAny<BankingInformationEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
            _odataClientMock.Verify(x => x.For<SSG_EmploymentContact>(It.IsAny<string>())
                .Set(It.IsAny<EmploymentContactEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_claim_CreateCompensationClaim_should_create_new_Claim()
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
                },
                BankInformationEntity = new BankingInformationEntity() { AccountNumber = "newbank" },
                EmploymentEntity = new EmploymentEntity() {
                    BusinessName = "newEmployer",
                    EmploymentContactEntities = new List<EmploymentContactEntity>()
                    {
                        new EmploymentContactEntity() { PhoneNumber = "123" }
                    }.ToArray()
                }
            };
            var result = await _sut.CreateCompensationClaim(claim, CancellationToken.None);

            Assert.AreEqual(_testCompensationId, result.CompensationClaimId);
            _odataClientMock.Verify(x => x.For<SSG_Employment>(It.IsAny<string>())
                 .Set(It.IsAny<EmploymentEntity>())
                 .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
            _odataClientMock.Verify(x => x.For<SSG_Asset_BankingInformation>(It.IsAny<string>())
                .Set(It.IsAny<BankingInformationEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
            _odataClientMock.Verify(x => x.For<SSG_EmploymentContact>(It.IsAny<string>())
                .Set(It.IsAny<EmploymentContactEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_claim_different_banking_CreateCompensationClaim_should_create_new_Claim()
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
                },
                EmploymentEntity = new EmploymentEntity()
                {
                    BusinessName = "newEmployer",
                    EmploymentContactEntities = new List<EmploymentContactEntity>()
                    {
                        new EmploymentContactEntity() { PhoneNumber = "123" }
                    }.ToArray()
                }
            };
            var result = await _sut.CreateCompensationClaim(compensationClaim, CancellationToken.None);

            Assert.AreEqual(_testCompensationId, result.CompensationClaimId);
            _odataClientMock.Verify(x => x.For<SSG_Employment>(It.IsAny<string>())
                 .Set(It.IsAny<EmploymentEntity>())
                 .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
            _odataClientMock.Verify(x => x.For<SSG_Asset_BankingInformation>(It.IsAny<string>())
                .Set(It.IsAny<BankingInformationEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
            _odataClientMock.Verify(x => x.For<SSG_EmploymentContact>(It.IsAny<string>())
                .Set(It.IsAny<EmploymentContactEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_claim_same_banking_different_employment_CreateCompensationClaim_should_create_new_Claim()
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
            var result = await _sut.CreateCompensationClaim(compensationClaim, CancellationToken.None);

            Assert.AreEqual(_testCompensationId, result.CompensationClaimId);
            _odataClientMock.Verify(x => x.For<SSG_Employment>(It.IsAny<string>())
                 .Set(It.IsAny<EmploymentEntity>())
                 .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
            _odataClientMock.Verify(x => x.For<SSG_Asset_BankingInformation>(It.IsAny<string>())
                .Set(It.IsAny<BankingInformationEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
            _odataClientMock.Verify(x => x.For<SSG_EmploymentContact>(It.IsAny<string>())
                .Set(It.IsAny<EmploymentContactEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_claim_same_banking_same_employment_CreateCompensationClaim_return_existed_claim()
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
            var result = await _sut.CreateCompensationClaim(compensationClaim, CancellationToken.None);

            Assert.AreEqual(originalClaimGuid, result.CompensationClaimId);
            Assert.AreEqual(originalEmploymentGuid, result.Employment.EmploymentId);
            Assert.AreEqual(true, result.IsDuplicated);
            Assert.AreEqual(true, result.Employment.IsDuplicated);
            _odataClientMock.Verify(x => x.For<SSG_Employment>(It.IsAny<string>())
                 .Set(It.IsAny<EmploymentEntity>())
                 .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
            _odataClientMock.Verify(x => x.For<SSG_Asset_BankingInformation>(It.IsAny<string>())
                .Set(It.IsAny<BankingInformationEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);
            _odataClientMock.Verify(x => x.For<SSG_EmploymentContact>(It.IsAny<string>())
                .Set(It.IsAny<EmploymentContactEntity>())
                .InsertEntryAsync(It.IsAny<CancellationToken>()), Times.Never);

        }


        #endregion


    }
}
