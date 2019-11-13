using System;
using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest.MatchFound;

namespace SearchAdapter.ICBC.Test.SearchRequest.MatchFound
{
    public class IcbcPersonBuilderTest
    {

        [Test]
        public void With_parameters_should_build_Person()
        {

            var sut = new IcbcPersonBuilder().WithFirstName("FirstName").WithLastName("LastName").WithDateOfBirth(new DateTime(2001,1,2)).Build();

            Assert.AreEqual("FirstName", sut.FirstName);
            Assert.AreEqual("LastName", sut.LastName);
            Assert.AreEqual(2001, sut.DateOfBirth.Year);
            Assert.AreEqual(1, sut.DateOfBirth.Month);
            Assert.AreEqual(2, sut.DateOfBirth.Day);

        }

    }
}