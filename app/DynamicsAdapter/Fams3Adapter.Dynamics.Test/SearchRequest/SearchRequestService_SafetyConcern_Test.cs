using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SafetyConcern;
using Fams3Adapter.Dynamics.SearchRequest;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_SafetyConcern_Test
    {
        private Mock<IODataClient> _odataClientMock;

        private readonly Guid _testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
        private readonly Guid _testAliasId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _sut = new SearchRequestService(_odataClientMock.Object, null);
        }

        #region safetyconcern testcases
        [Test]
        public async Task with_safetyconcern_entity_CreateSafetyConcern_should_return_new_SSG_SafetyConcern()
        {
            _odataClientMock.Setup(x => x.For<SSG_SafetyConcernDetail>(null).Set(It.Is<SafetyConcernEntity>(x => x.Detail == "safe"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SafetyConcernDetail()
                {
                    SafetyConcernDetailId = _testAliasId
                })
                );
            var safe = new SafetyConcernEntity()
            {
                Detail = "safe",
                Person = new SSG_Person() {}
            };
            var result = await _sut.CreateSafetyConcern(safe, CancellationToken.None);

            Assert.AreEqual(_testAliasId, result.SafetyConcernDetailId);
        }

        [Test]
        public async Task update_SafetyConcern_should_success()
        {
            Guid testId = Guid.NewGuid();
            _odataClientMock.Setup(x => x.For<SSG_SafetyConcernDetail>(null).Key(It.Is<Guid>(m => m == testId)).Set(It.IsAny<Dictionary<string, object>>())
                .UpdateEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_SafetyConcernDetail()
                {
                    SafetyConcernDetailId = testId,
                })
                );
            IDictionary<string, object> updatedFields = new Dictionary<string, object> { { "ssg_name", "new" } };

            var result = await _sut.UpdateSafetyConcern(testId, updatedFields, CancellationToken.None);

            Assert.AreEqual(testId, result.SafetyConcernDetailId);

        }

        #endregion


    }
}
