using Fams3Adapter.Dynamics.BankInfo;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.OtherAsset;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_BankInfo_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private Guid _testBankInfoId;
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object,_loggerMock.Object);
        }

        #region bank info testcases
        [Test]
        public async Task with_non_duplicated_person_CreateBankInfo_should_return_new_SSG_Asset_BankingInformation()
        {
            _testBankInfoId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Asset_BankingInformation>(null).Set(It.Is<BankingInformationEntity>(x => x.AccountNumber == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_BankingInformation()
                {
                    AccountNumber = "normal",
                    BankingInformationId = _testBankInfoId
                })
                );
            var bankInfo = new SSG_Asset_BankingInformation()
            {
                AccountNumber = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };

            var result = await _sut.CreateBankInfo(bankInfo, CancellationToken.None);

            Assert.AreEqual("normal", result.AccountNumber);
            Assert.AreEqual(_testBankInfoId, result.BankingInformationId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_bankInfo_should_return_new_SSG_Asset_BankingInformation()
        {
            _testBankInfoId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Asset_BankingInformation>(null).Set(It.Is<BankingInformationEntity>(x => x.AccountNumber == "notContain"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Asset_BankingInformation()
                {
                    AccountNumber = "notContain",
                    BankingInformationId = _testBankInfoId
                })
                );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<BankingInformationEntity>(m => m.AccountNumber == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var bankInfo = new SSG_Asset_BankingInformation()
            {
                AccountNumber = "notContain",
                Person = new SSG_Person() { IsDuplicated = true }
            };
            var result = await _sut.CreateBankInfo(bankInfo, CancellationToken.None);

            Assert.AreEqual("notContain", result.AccountNumber);
            Assert.AreEqual(_testBankInfoId, result.BankingInformationId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_bankInfo_should_return_original_bankInfo_guid()
        {
            _testBankInfoId = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<BankingInformationEntity>(m => m.AccountNumber == "contain")))
                .Returns(Task.FromResult(_testBankInfoId));

            var bankInfo = new BankingInformationEntity()
            {
                AccountNumber = "contain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateBankInfo(bankInfo, CancellationToken.None);

            Assert.AreEqual(_testBankInfoId, result.BankingInformationId);
        }
        #endregion


    }
}
