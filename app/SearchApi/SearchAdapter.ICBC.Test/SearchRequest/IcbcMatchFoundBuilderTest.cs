using System;
using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest;

namespace SearchAdapter.ICBC.Test.SearchRequest
{
    public class IcbcMatchFoundBuildTest
    {

        [Test]
        public void It_should_build_a_match_found()
        {
            Guid id = Guid.NewGuid();

            var sut = new IcbcMatchFoundBuilder(id)
                .WithFirstName("FirstName")
                .WithLastName("LastName")
                .WithDateOfBirth(new DateTime(2001, 1, 1))
                .Build();

            Assert.AreEqual(id, sut.SearchRequestId, "searchRequestId values are not equals");
            Assert.AreEqual("FirstName", sut.FirstName, "FirstName values are not equals");
            Assert.AreEqual("LastName", sut.LastName, "LastNames values are not equals");
            Assert.AreEqual(new DateTime(2001, 1,1), sut.DateOfBirth, "Date of Birth values are not equals" );
        }

    }
}