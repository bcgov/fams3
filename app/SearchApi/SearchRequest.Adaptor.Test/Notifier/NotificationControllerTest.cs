using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SearchRequest.Adaptor.Notifier;
using SearchRequest.Adaptor.Notifier.Models;
using SearchRequest.Adaptor.Publisher.Models;
using SearchRequestAdaptor.Publisher;
using System;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Test.Notifier
{
    public class NotificationControllerTest
    {

        private NotificationController _sut;
        private Mock<ILogger<NotificationController>> _loggerMock;
        private Mock<ISearchRequestEventPublisher> _searchRquestEventPublisherMock;
        private Notification FakeNotification;

        [SetUp]
        public void Init()
        {
            FakeNotification = new Notification
            {
                Acvitity = "RequestAssignedToFSO",
                ActivityDate = DateTime.Now,
                FileId = "1231231",
                Agency = "FMEP",
                AgencyFileId = "1231231"
            };

            _loggerMock = new Mock<ILogger<NotificationController>>();
            _searchRquestEventPublisherMock = new Mock<ISearchRequestEventPublisher>();

            _searchRquestEventPublisherMock.Setup(x => x.PublishSearchRequestNotification(It.IsAny<SearchRequestNotificationEvent>()))
                .Returns(Task.CompletedTask);

            _sut = new NotificationController(_searchRquestEventPublisherMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task With_valid_payload_should_return_ok()
        {
          await _sut.Notify(FakeNotification);
            _searchRquestEventPublisherMock.Verify(x => x.PublishSearchRequestNotification(It.IsAny<SearchRequestNotificationEvent>()), Times.Once());
        }

        [Test]
        public async Task With_null_payload_should_return_bad_Request()
        {
           var result = await _sut.Notify(null);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task With_invalid_payload_should_return_bad_Request()
        {
            _sut.ModelState.AddModelError("Notification.Activity", "Required");
            var result = await _sut.Notify(FakeNotification);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
