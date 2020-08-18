using Fams3Adapter.Dynamics.Address;
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
    public class SearchRequestService_Employment_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testEmploymentId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
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
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object);
        }

        #region employment testcases
        [Test]
        public async Task with_non_duplicated_person_CreateEmployment_should_return_new_SSG_Employment()
        {
            _odataClientMock.Setup(x => x.For<SSG_Employment>(null).Set(It.Is<EmploymentEntity>(x => x.AddressLine1 == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Employment()
                {
                    EmploymentId = _testEmploymentId
                })
                );
            var employment = new EmploymentEntity()
            {
                AddressLine1 = "normal",
                Person = new SSG_Person() { IsDuplicated = false }
            };
            var result = await _sut.CreateEmployment(employment, CancellationToken.None);

            Assert.AreEqual(_testEmploymentId, result.EmploymentId);
        }

        [Test]
        public async Task with_duplicated_person_not_contains_same_employment_should_return_new_SSG_Employment()
        {
            _odataClientMock.Setup(x => x.For<SSG_Employment>(null).Set(It.Is<EmploymentEntity>(x => x.AddressLine1 == "notContain"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Employment()
                {
                    EmploymentId = _testEmploymentId
                })
                );

            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<EmploymentEntity>(m => m.AddressLine1 == "notContain")))
                .Returns(Task.FromResult(Guid.Empty));

            var employment = new EmploymentEntity()
            {
                AddressLine1 = "notContain",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateEmployment(employment, CancellationToken.None);

            Assert.AreEqual(_testEmploymentId, result.EmploymentId);
        }

        [Test]
        public async Task with_duplicated_person_contains_same_employment_should_return_original_employment_guid()
        {
            Guid originalGuid = Guid.NewGuid();
            _duplicateServiceMock.Setup(x => x.Exists(It.Is<SSG_Person>(m => m.PersonId == _testId), It.Is<EmploymentEntity>(m => m.AddressLine1 == "contains")))
                .Returns(Task.FromResult(_testEmploymentId));

            _odataClientMock.Setup(x => x.For<SSG_Employment>(null)
                  .Key(It.Is<Guid>(m => m == _testEmploymentId))
                  .Expand(x => x.SSG_EmploymentContacts)
                  .FindEntryAsync(It.IsAny<CancellationToken>()))
                  .Returns(Task.FromResult(new SSG_Employment()
                  {
                      EmploymentId = originalGuid,
                      AddressLine1 = "original"
                  }));

            var employment = new EmploymentEntity()
            {
                AddressLine1 = "contains",
                Person = new SSG_Person()
                {
                    PersonId = _testId,
                    IsDuplicated = true
                }
            };
            var result = await _sut.CreateEmployment(employment, CancellationToken.None);

            Assert.AreEqual(originalGuid, result.EmploymentId);
            Assert.AreEqual("original", result.AddressLine1);
        }

        [Test]
        public async Task GetEmployment_should_return_1_level_expanded_data()
        {
            Guid employId = Guid.NewGuid();

            _odataClientMock.Setup(x => x.For<SSG_Employment>(null)
                .Key(It.Is<Guid>(m => m == employId))
                .Expand(x => x.SSG_EmploymentContacts)
                .FindEntryAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.FromResult<SSG_Employment>(new SSG_Employment()
               {
                   EmploymentId = employId,
                   SSG_EmploymentContacts = new List<SSG_EmploymentContact>() { new SSG_EmploymentContact() { } }.ToArray()
               }));
            var result = await _sut.GetEmployment(employId, CancellationToken.None);
            Assert.AreEqual(1, result.SSG_EmploymentContacts.Length);
        }

        [Test]
        public async Task update_correct_Employment_should_success()
        {
            Guid testId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_Employment>(null).Key(It.Is<Guid>(m => m == testId)).Set(It.IsAny<EmploymentEntity>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Employment()
                {
                    EmploymentId = testId,
                    BusinessName = "new"
                })
                );

            var employ = new SSG_Employment()
            {
                EmploymentId = testId,
                BusinessName = "old"
            };
            var result = await _sut.UpdateEmployment(employ, CancellationToken.None);

            Assert.AreEqual("new", result.BusinessName);

        }
        #endregion


    }
}
