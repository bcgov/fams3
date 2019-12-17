using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SearchAdapter.Sample.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Person.Contracts;

namespace SearchAdapter.Sample.Test
{
    public class PersonSearchValidatorTest
    {

        private PersonSearchValidator _sut;

        public class PersonTest : Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public IEnumerable<PersonalIdentifier> Identifiers { get; }
            public IEnumerable<Address> Addresses { get; }

            public IEnumerable<PhoneNumber> PhoneNumbers { get; }
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new PersonSearchValidator();
        }

        [Test]
        public void When_search_is_value_should_be_valid()
        {
            var command = new PersonTest()
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
            var command = new PersonTest()
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
            var command = new PersonTest()
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
            var command = new PersonTest()
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
            var command = new PersonTest()
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
            var command = new PersonTest()
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
            var command = new PersonTest()
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