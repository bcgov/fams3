using System;
using System.Linq;
using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest;
using SearchApi.Core.Test.Fake;

namespace SearchAdapter.ICBC.Test.SearchRequest.MatchFound
{
    public class IcbcMatchFoundBuildTest
    {

        [Test]
        public void It_should_build_a_match_found()
        {
            Guid id = Guid.NewGuid();

            var sut = new IcbcMatchFoundBuilder(id)
                .WithPerson(new FakePerson())
                .Build();

            Assert.AreEqual("FirstName", sut.MatchedPerson.FirstName);
            Assert.AreEqual("LastName", sut.MatchedPerson.LastName);

        }

    }
}