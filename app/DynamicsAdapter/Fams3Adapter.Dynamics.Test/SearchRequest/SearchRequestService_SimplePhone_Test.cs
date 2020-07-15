using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.InsuranceClaim;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_SimplePhone_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testSimplePhoneId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
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
        public async Task with_non_duplicated_insurance_CreateSimplePhoneNumber_should_return_new_SSG_InvolvedParty()
        {
            _odataClientMock.Setup(x => x.For<SSG_SimplePhoneNumber>(null).Set(It.Is<SimplePhoneNumberEntity>(x => x.PhoneNumber == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SimplePhoneNumber()
                {
                    SimplePhoneNumberId = _testSimplePhoneId
                })
                );
            var phone = new SimplePhoneNumberEntity()
            {
                PhoneNumber = "normal",
                SSG_Asset_ICBCClaim = new SSG_Asset_ICBCClaim() { IsDuplicated = false }
            };
            var result = await _sut.CreateSimplePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual(_testSimplePhoneId, result.SimplePhoneNumberId);
        }

        [Test]
        public async Task with_duplicated_insurance_not_contains_same_simplePhone_CreateSimplePhoneNumber_should_return_new_SSG_InvolvedParty()
        {
            _odataClientMock.Setup(x => x.For<SSG_SimplePhoneNumber>(null).Set(It.Is<SimplePhoneNumberEntity>(x => x.PhoneNumber == "notContain"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SimplePhoneNumber()
                {
                    SimplePhoneNumberId = _testSimplePhoneId
                })
                );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Asset_ICBCClaim>(m => m.ICBCClaimId == _testId), It.Is<SimplePhoneNumberEntity>(m => m.PhoneNumber == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var phone = new SimplePhoneNumberEntity()
            {
                PhoneNumber = "notContain",
                SSG_Asset_ICBCClaim = new SSG_Asset_ICBCClaim() { IsDuplicated = true }
            };

            var result = await _sut.CreateSimplePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual(_testSimplePhoneId, result.SimplePhoneNumberId);
        }

        [Test]
        public async Task with_duplicated_insurance_contains_same_simplePhone_CreateSimplePhoneNumber_should_return_original_involvedparty_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Asset_ICBCClaim>(m => m.ICBCClaimId == _testId), It.Is<SimplePhoneNumberEntity>(m => m.PhoneNumber == "contains")))
                .Returns(Task.FromResult(originalGuid));

            var phone = new SimplePhoneNumberEntity()
            {
                PhoneNumber = "contains",
                SSG_Asset_ICBCClaim = new SSG_Asset_ICBCClaim()
                {
                    ICBCClaimId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateSimplePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.SimplePhoneNumberId);
        }
        #endregion


    }
}
