using System;
using System.Linq;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest;
using SearchApi.Core.Contracts;

namespace SearchAdapter.ICBC.Test.SearchRequest
{
    public class PersonSearchValidatorTest
    {

        private PersonSearchValidator _sut;

        public class ExecuteSearchCommandTest : ExecuteSearch
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new PersonSearchValidator();
        }

        [Test]
        public void When_search_is_valie_should_not_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = "fistName",
                LastName = "lastName"
            };

            var result = _sut.Validate(command);

            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void When_first_name_is_null_should_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = null,
                LastName = "lastName"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.FirstName), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void When_first_name_is_empty_should_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = "",
                LastName = "lastName"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.FirstName), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void When_first_name_is_white_space_should_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = "  ",
                LastName = "lastName"
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.FirstName), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void When_last_name_is_null_should_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = "fisrtName",
                LastName = null
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.LastName), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void When_last_name_is_empty_should_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = "firstNAme",
                LastName = ""
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.LastName), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void When_last_name_is_white_space_should_have_validation_error()
        {
            var command = new ExecuteSearchCommandTest()
            {
                FirstName = "firstName",
                LastName = "  "
            };

            var result = _sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(nameof(command.LastName), result.Errors.FirstOrDefault().PropertyName);
        }
    }
}