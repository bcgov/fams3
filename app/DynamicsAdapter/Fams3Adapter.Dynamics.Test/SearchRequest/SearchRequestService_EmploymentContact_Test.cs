using Fams3Adapter.Dynamics.AssetOwner;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Employment;
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
    public class SearchRequestService_EmploymentContact_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testEmploymentContactId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region employmnet contact testcases
        [Test]
        public async Task with_non_duplicated_employment_CreateEmploymentContact_should_return_new_SSG_EmploymentContact()
        {
            _odataClientMock.Setup(x => x.For<SSG_EmploymentContact>(null).Set(It.Is<EmploymentContactEntity>(x => x.FaxNumber == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_EmploymentContact()
                {
                    EmploymentContactId = _testEmploymentContactId
                })
                );
            var employmentContact = new EmploymentContactEntity()
            {
                FaxNumber = "normal",
                Employment = new SSG_Employment() { IsDuplicated = false }
            };
            var result = await _sut.CreateEmploymentContact(employmentContact, CancellationToken.None);

            Assert.AreEqual(_testEmploymentContactId, result.EmploymentContactId);
        }

        [Test]
        public async Task with_duplicated_employment_not_contains_same_employmentcontact_should_return_new_SSG_EmploymentContact()
        {
            _odataClientMock.Setup(x => x.For<SSG_EmploymentContact>(null).Set(It.Is<EmploymentContactEntity>(x => x.FaxNumber == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_EmploymentContact()
                 {
                     EmploymentContactId = _testEmploymentContactId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Employment>(m => m.EmploymentId == _testId), It.Is<EmploymentContactEntity>(m => m.FaxNumber == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var employmentContact = new EmploymentContactEntity()
            {
                FaxNumber = "notContain",
                Employment = new SSG_Employment() { IsDuplicated = true }
            };

            var result = await _sut.CreateEmploymentContact(employmentContact, CancellationToken.None);

            Assert.AreEqual(_testEmploymentContactId, result.EmploymentContactId);
        }

        [Test]
        public async Task with_duplicated_employment_contains_same_employmentcontact_should_return_original_employmentcontact_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Employment>(m => m.EmploymentId == _testId), It.Is<EmploymentContactEntity>(m => m.FaxNumber == "contains")))
                .Returns(Task.FromResult(originalGuid));

            var employmentContact = new EmploymentContactEntity()
            {
                FaxNumber = "contains",
                Employment = new SSG_Employment()
                {
                    EmploymentId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateEmploymentContact(employmentContact, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.EmploymentContactId);
        }
        #endregion


    }
}
