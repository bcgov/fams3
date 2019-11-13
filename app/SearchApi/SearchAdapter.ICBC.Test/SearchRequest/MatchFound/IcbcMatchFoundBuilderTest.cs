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
                .AddPersonId(new FakePersonId())
                .Build();

            Assert.AreEqual("FirstName", sut.Person.FirstName);
            Assert.AreEqual("LastName", sut.Person.LastName);
            
            Assert.AreEqual(1, sut.PersonIds.ToList().Count);

            Assert.AreEqual("Issuer", sut.PersonIds.ToList().FirstOrDefault().Issuer);


        }

    }
}