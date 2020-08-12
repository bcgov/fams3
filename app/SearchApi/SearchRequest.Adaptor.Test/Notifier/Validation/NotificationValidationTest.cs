using Microsoft.VisualBasic;
using NUnit.Framework;
using SearchRequest.Adaptor.Notifier.Models;
using SearchRequest.Adaptor.Notifier.Models.Validation;
using System;

namespace SearchRequest.Adaptor.Test.Notifier.Validation
{
    public class NotificationValidatorTest
    {
        private NotificationValidator _sut;

        private Notification notification;

        [SetUp]
        public void SetUp()
        {
            _sut = new NotificationValidator();
        }

        [Test]
        public void when_notification_request_is_valid_should_be_valid()
        {
            notification = new Notification
            {
                ActivityDate = DateTime.Now,
                Acvitity = "RequestAssignedToFSO",
                AgencyFileId = "12312312",
                Agency = "FMEP",
                FileId = "121212"
            };
          
            var result = _sut.Validate(notification);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void when_notification_activity_is_incorrect_should_be_invalid()
        {
            notification = new Notification
            {
                ActivityDate = DateTime.Now,
                Acvitity = "RequestAsToFSO",
                AgencyFileId = "12312312",
                Agency = "FMEP",
                FileId = "121212"
            };

            var result = _sut.Validate(notification);

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void when_notification_request_has_no_activity_should_be_invalid()
        {
            notification = new Notification
            {
                ActivityDate = DateTime.Now,
                AgencyFileId = "12312312",
                Agency = "FMEP",
                FileId = "121212"
            };

            var result = _sut.Validate(notification);

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void when_notification_request_has_no_activity_date_should_be_invalid()
        {
            notification = new Notification
            {
               
                Acvitity = "RequestAssignedToFSO",
                AgencyFileId = "12312312",
                Agency = "FMEP",
                FileId = "121212"
            };

            var result = _sut.Validate(notification);

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void when_notification_request_has_no_agencyfileid_should_be_invalid()
        {
            notification = new Notification
            {
                ActivityDate = DateTime.Now,
                Acvitity = "RequestAssignedToFSO",
                Agency = "FMEP",
                FileId = "121212"
            };

            var result = _sut.Validate(notification);

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void when_notification_request_has_no_agency_should_be_invalid()
        {
            notification = new Notification
            {
                ActivityDate = DateTime.Now,
                Acvitity = "RequestAssignedToFSO",
                AgencyFileId = "12312312",
                FileId = "121212"
            };

            var result = _sut.Validate(notification);

            Assert.IsFalse(result.IsValid);
        }


        [Test]
        public void when_notification_request_has_no_fileid_should_be_invalid()
        {
            notification = new Notification
            {
                ActivityDate = DateTime.Now,
                Acvitity = "RequestAssignedToFSO",
                AgencyFileId = "12312312"
            };

            var result = _sut.Validate(notification);

            Assert.IsFalse(result.IsValid);
        }
    }
}
