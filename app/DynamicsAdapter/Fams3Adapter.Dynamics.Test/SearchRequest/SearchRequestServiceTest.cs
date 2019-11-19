using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestServiceTest
    {
        private Mock<IODataClient> odataClientMock = new Mock<IODataClient>();
        private readonly Mock<ILogger<SearchRequestService>> _loggerMock = new Mock<ILogger<SearchRequestService>>();

        private Guid testId = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");

        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            odataClientMock.Setup(x => x.For<SSG_Identifier>(null).Set(It.Is<Dictionary<string, Object>>(x => x["SSG_Identification"].ToString() == "identificationtest"))
            .InsertEntryAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(new SSG_Identifier()
            {
                SSG_Identification = "test"
            })
            );

            odataClientMock.Setup(x => x.For<SSG_Identifier>(null).Set(It.Is<Dictionary<string, Object>>(x => x["SSG_Identification"].ToString() == "exception"))
            .InsertEntryAsync(It.IsAny<CancellationToken>())).Throws(new Exception());

            _sut = new SearchRequestService(odataClientMock.Object, _loggerMock.Object);
        }


        [Test]
        public async Task with_correct_searchRequestid_upload_identifier_should_success()
        {
            var identifier = new SSG_Identifier()
            {
                SSG_Identification = "identificationtest",
                ssg_identificationeffectivedate = DateTime.Now,
                StateCode = 0,
                StatusCode = 1,
                SSG_SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId }
            };

            var result = await _sut.UploadIdentifier(identifier, CancellationToken.None);

            Assert.AreEqual("test", result.SSG_Identification);
        }

        [Test]
        public void with_wrong_identifier_should_throws_exception()
        {
            SSG_Identifier identifier = new SSG_Identifier();
            identifier.SSG_Identification = "exception";
            identifier.ssg_identificationeffectivedate = DateTime.Now;
            identifier.StateCode = 0;
            identifier.StatusCode = 1;
            identifier.SSG_SearchRequest = new SSG_SearchRequest() { SearchRequestId = testId };
            Assert.ThrowsAsync<Exception>(async () => await _sut.UploadIdentifier(identifier, CancellationToken.None));
        }
    }
}
