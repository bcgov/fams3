using AgencyAdapter.Sample.Models;
using AgencyAdapter.Sample.SearchRequest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AgencAdapter.Sample.Test
{
    public class SearchRequestValidatorTest

    {
        private SearchRequestValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new SearchRequestValidator();
        }

        [Test]
        public void When_search_is_valid_should_be_valid()
        {
            var command = new SearchRequest()
            {
               RequestorAgencyCode = "code",
               AgentEmail = "myemail@email.com",
               InformationRequestedList =  new List<string>  () { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void when_agency_code_is_null_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = null,
                AgentEmail = "myemail@email.com",
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.RequestorAgencyCode), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_agency_code_is_not_passed_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
               
                AgentEmail = "myemail@email.com",
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.RequestorAgencyCode), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_agency_code_is_empty_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = string.Empty,
                AgentEmail = "myemail@email.com",
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.RequestorAgencyCode), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void when_agent_email_is_null_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = null,
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.AgentEmail), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_agent_email_is_not_passed_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
            
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.AgentEmail), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_agent_email_is_empty_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = string.Empty,
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual(nameof(command.AgentEmail), result.Errors.FirstOrDefault().PropertyName);
        }


        [Test]
        public void when_info_requested_is_null_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = "email@myemail.com",
                InformationRequestedList = null,
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.InformationRequestedList), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_info_requested_is_not_passed_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = "email@myemail.com",
              
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.InformationRequestedList), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_info_requested_is_empty_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = "email@myemail.com",
                InformationRequestedList = new List<string>(),
                RequestAction = "NEW"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.InformationRequestedList), result.Errors.FirstOrDefault().PropertyName);
        }


        [Test]
        public void when_request_action_requested_is_null_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = "email@myemail.com",
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = null
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.RequestAction), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_request_action_is_not_passed_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = "email@myemail.com",
                InformationRequestedList = new List<string>() { "test", "ramt" },
              
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.RequestAction), result.Errors.FirstOrDefault().PropertyName);
        }
        [Test]
        public void when_reques_action_is_empty_should_return_validation_error()
        {
            var command = new SearchRequest()
            {
                RequestorAgencyCode = "code",
                AgentEmail = "email@myemail.com",
                InformationRequestedList = new List<string>() { "test", "ramt" },
                RequestAction = string.Empty
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.RequestAction), result.Errors.FirstOrDefault().PropertyName);
        }
    }
}
