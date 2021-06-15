using Fams3Adapter.Dynamics.Address;
using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
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
    public class SearchRequestService_Address_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testAddressId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();

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

            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        #region address testcases
        [Test]
        public async Task with_non_duplicated_person_CreateAddress_should_return_new_SSG_address()
        {
            _odataClientMock.Setup(x => x.For<SSG_Address>(null).Set(It.Is<AddressEntity>(x => x.AddressLine1 == "address full text"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Address()
                {
                    AddressLine1 = "line1",
                    AddressId = _testAddressId
                })
                );
            var address = new AddressEntity()
            {
                AddressLine1 = "address full text",
                CountryText = "canada",
                CountrySubdivisionText = "British Columbia",
                Person = new SSG_Person() { PersonId = _testId }
            };

            var result = await _sut.CreateAddress(address, CancellationToken.None);

            Assert.AreEqual("line1", result.AddressLine1);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_address_should_return_new_SSG_address()
        {
            _odataClientMock.Setup(x => x.For<SSG_Address>(null).Set(It.Is<AddressEntity>(x => x.AddressLine1 == "line1"))
                 .InsertEntryAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.FromResult(new SSG_Address()
                 {
                     AddressLine1 = "line",
                     AddressId = _testAddressId
                 })
                 );
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<AddressEntity>(m => m.AddressLine1 == "line1")))
                .Returns(Task.FromResult(Guid.Empty));

            var address = new AddressEntity()
            {
                AddressLine1 = "line1",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    SSG_Addresses = new List<SSG_Address>()
                    {
                        new SSG_Address(){ AddressId=_testAddressId}
                    }.ToArray(),
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateAddress(address, CancellationToken.None);

            Assert.AreEqual("line", result.AddressLine1);
            Assert.AreEqual(_testAddressId, result.AddressId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_address_should_return_original_address_guid()
        {
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<AddressEntity>(m => m.AddressLine1 == "duplicated")))
                .Returns(Task.FromResult(_testAddressId));

            var address = new AddressEntity()
            {
                AddressLine1 = "duplicated",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    SSG_Addresses = new List<SSG_Address>()
                    {
                        new SSG_Address(){AddressId=_testAddressId}
                    }.ToArray(),
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateAddress(address, CancellationToken.None);

            Assert.AreEqual(_testAddressId, result.AddressId);
        }
        #endregion


    }
}
