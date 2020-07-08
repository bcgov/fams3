using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Name;
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
    public class SearchRequestService_Alias_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testAliasId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region alias testcases
        [Test]
        public async Task with_non_duplicated_person_createName_should_return_new_SSG_Aliase()
        {
            _odataClientMock.Setup(x => x.For<SSG_Aliase>(null).Set(It.Is<AliasEntity>(x => x.FirstName == "firstname"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Aliase()
                {
                    AliasId = _testAliasId
                })
                );
            var alias = new AliasEntity()
            {
                Date1 = DateTime.Now,
                Date1Label = "Effective Date",
                Date2 = new DateTime(2001, 1, 1),
                Date2Label = "Expiry Date",
                FirstName = "firstname",
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.CreateName(alias, CancellationToken.None);

            Assert.AreEqual(_testAliasId, result.AliasId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_alias_should_return_new_SSG_Alias()
        {
            _odataClientMock.Setup(x => x.For<SSG_Aliase>(null).Set(It.Is<AliasEntity>(x => x.FirstName == "notContain"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Aliase()
                 {
                     AliasId = _testAliasId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<AliasEntity>(m => m.FirstName == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var alias = new AliasEntity()
            {
                FirstName = "notContain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateName(alias, CancellationToken.None);

            Assert.AreEqual(_testAliasId, result.AliasId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_alias_should_return_original_alias_guid()
        {
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<AliasEntity>(m => m.FirstName == "contains")))
                .Returns(Task.FromResult(_testAliasId));

            var alias = new AliasEntity()
            {
                FirstName = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateName(alias, CancellationToken.None);

            Assert.AreEqual(_testAliasId, result.AliasId);
        }
        #endregion


    }
}
