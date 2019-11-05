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

            var sut = new IcbcMatchFoundBuilder()
                .WithFirstName("FirstName")
                .WithLastName("LastName")
                .WithDateOfBirth(new DateTime(2001, 1, 1))
                .Build();

            Assert.AreEqual("FirstName", sut.FirstName, "FirstName values are not equals");
            Assert.AreEqual("LastName", sut.LastName, "LastNames values are not equals");
            Assert.AreEqual(new DateTime(2001, 1,1), sut.DateOfBirth, "Date of Birth values are not equals" );
        }

    }
}