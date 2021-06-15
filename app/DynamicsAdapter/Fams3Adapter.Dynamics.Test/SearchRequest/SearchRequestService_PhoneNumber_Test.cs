using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.PhoneNumber;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_PhoneNumber_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testPhoneId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object,_loggerMock.Object);
        }

        #region phonenumber testcases
        [Test]
        public async Task with_non_duplicated_person_createPhoneNumber_should_return_new_SSG_phoneNumber()
        {
            _odataClientMock.Setup(x => x.For<SSG_PhoneNumber>(null).Set(It.Is<PhoneNumberEntity>(x => x.TelePhoneNumber == "4007678231"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_PhoneNumber()
                {
                    TelePhoneNumber = "4007678231",
                    PhoneNumberId = _testPhoneId
                })
                );
            var phone = new PhoneNumberEntity()
            {
                Date1 = DateTime.Now,
                Date1Label = "Effective Date",
                Date2 = new DateTime(2001, 1, 1),
                Date2Label = "Expiry Date",
                TelePhoneNumber = "4007678231",
                StateCode = 0,
                StatusCode = 1,
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.CreatePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual("4007678231", result.TelePhoneNumber);
            Assert.AreEqual(_testPhoneId, result.PhoneNumberId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_phonenumber_should_return_new_SSG_phoneNumber()
        {
            _odataClientMock.Setup(x => x.For<SSG_PhoneNumber>(null).Set(It.Is<PhoneNumberEntity>(x => x.TelePhoneNumber == "4000000000"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_PhoneNumber()
                 {
                     TelePhoneNumber = "4000000000",
                     PhoneNumberId = _testPhoneId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<PhoneNumberEntity>(m => m.TelePhoneNumber == "4000000000")))
                .Returns(Task.FromResult(Guid.Empty));

            var phone = new PhoneNumberEntity()
            {
                TelePhoneNumber = "4000000000",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    SSG_PhoneNumbers = new List<SSG_PhoneNumber>()
                    {
                        new SSG_PhoneNumber(){PhoneNumberId=_testPhoneId}
                    }.ToArray(),
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreatePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual("4000000000", result.TelePhoneNumber);
            Assert.AreEqual(_testPhoneId, result.PhoneNumberId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_phonenumber_should_return_original_phonenumber_guid()
        {
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<PhoneNumberEntity>(m => m.TelePhoneNumber == "3000000000")))
                .Returns(Task.FromResult(_testPhoneId));

            var phone = new PhoneNumberEntity()
            {
                TelePhoneNumber = "3000000000",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    SSG_PhoneNumbers = new List<SSG_PhoneNumber>()
                    {
                        new SSG_PhoneNumber(){PhoneNumberId=_testPhoneId}
                    }.ToArray(),
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreatePhoneNumber(phone, CancellationToken.None);

            Assert.AreEqual(_testPhoneId, result.PhoneNumberId);
        }
        #endregion


    }
}
