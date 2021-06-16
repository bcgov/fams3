using Fams3Adapter.Dynamics.Duplicate;
using Fams3Adapter.Dynamics.Name;
using Fams3Adapter.Dynamics.Notes;
using Fams3Adapter.Dynamics.Person;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fams3Adapter.Dynamics.Test.SearchRequest
{
    public class SearchRequestService_Notes_Test
    {
        private Mock<IODataClient> _odataClientMock;
        private Mock<IDuplicateDetectionService> _duplicateServiceMock;
        private Mock<ILogger<SearchRequestService>> _loggerMock;
        private readonly Guid _testNotesId = Guid.Parse("6AE89FE6-9909-EA11-1111-00505683FBF4");
        private SearchRequestService _sut;

        [SetUp]
        public void SetUp()
        {
            _odataClientMock = new Mock<IODataClient>();
            _duplicateServiceMock = new Mock<IDuplicateDetectionService>();
            _loggerMock = new Mock<ILogger<SearchRequestService>>();
            _sut = new SearchRequestService(_odataClientMock.Object, _duplicateServiceMock.Object, _loggerMock.Object);
        }

        #region notes testcases
        [Test]
        public async Task with_existed_fileId_createNotes_should_return_SSG_Notes()
        {
            _odataClientMock.Setup(x => x.For<SSG_Notese>(null).Set(It.Is<NotesEntity>(x => x.Description == "normal"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new SSG_Notese()
                {
                    NotesId = _testNotesId
                })
                );
            var notesEntity = new NotesEntity()
            {
                StatusCode = 1,
                Description = "normal",
            };
            var result = await _sut.CreateNotes(notesEntity, CancellationToken.None);

            Assert.AreEqual(_testNotesId, result.NotesId);
        }

        [Test]
        public void when_exceptionOccure_createNotes_should_throws_exception()
        {
            _odataClientMock.Setup(x => x.For<SSG_Notese>(null).Set(It.Is<NotesEntity>(x => x.Description == "exception"))
                .InsertEntryAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception("fakeException"));
            var notesEntity = new NotesEntity()
            {
                StatusCode = 1,
                Description = "exception",
            };
            Assert.ThrowsAsync<Exception>(async () => await _sut.CreateNotes(notesEntity, CancellationToken.None));
        }
        #endregion


    }
}
