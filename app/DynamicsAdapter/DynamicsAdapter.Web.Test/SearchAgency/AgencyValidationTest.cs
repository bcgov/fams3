using DynamicsAdapter.Web.SearchAgency.Models;
using DynamicsAdapter.Web.SearchAgency.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class SearchResponseReadyValidatorTest
    {
        private SearchResponseReadyValidator _sut;
        private SearchResponseReady _ready;

        [SetUp]
        public void SetUp()
        {
            _sut = new SearchResponseReadyValidator();
            _ready = new SearchResponseReady()
            {
                Activity = "RequestClosed",
                ActivityDate = DateTime.Now,
                Agency = "agency",
                FileId = "fileId",
                AgencyFileId = "referId",
                FSOName = "fso",
                ResponseGuid = Guid.NewGuid().ToString()
            };
        }

        [Test]
        public void When_SearchResponseReady_is_valid_should_be_valid()
        {
            var result = _sut.Validate(_ready);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void when_agency_code_is_empty_should_return_validation_error()
        {
            _ready.Agency = "";
            var result = _sut.Validate(_ready);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(nameof(_ready.Agency), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void when_agency_code_is_not_passed_should_return_validation_error()
        {
            _ready.Agency = null;
            var result = _sut.Validate(_ready);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(nameof(_ready.Agency), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void when_reference_is_empty_should_return_validation_error()
        {
            _ready.AgencyFileId = "";
            var result = _sut.Validate(_ready);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(nameof(_ready.AgencyFileId), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void when_fileId_is_empty_should_return_validation_error()
        {
            _ready.FileId = "";
            var result = _sut.Validate(_ready);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(nameof(_ready.FileId), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void when_Activity_is_incorrect_should_return_validation_error()
        {
            _ready.Activity = "Activity";
            var result = _sut.Validate(_ready);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(nameof(_ready.Activity), result.Errors.FirstOrDefault().PropertyName);
        }

        [Test]
        public void when_responseId_is_incorrect_should_return_validation_error()
        {
            _ready.ResponseGuid = Guid.Empty.ToString();
            var result = _sut.Validate(_ready);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(nameof(_ready.ResponseGuid), result.Errors.FirstOrDefault().PropertyName);
        }
    }
}
