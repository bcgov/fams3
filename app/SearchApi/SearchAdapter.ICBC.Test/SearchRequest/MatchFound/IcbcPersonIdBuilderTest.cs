using NUnit.Framework;
using SearchAdapter.ICBC.SearchRequest.MatchFound;
using SearchApi.Core.Person.Contracts;

namespace SearchAdapter.ICBC.Test.SearchRequest.MatchFound
{
    public class IcbcPersonIdBuilderTest
    {
        [Test]
        public void With_parameters_should_build_PersonId()
        {

            var sut = new IcbcPersonIdBuilder(PersonIDKind.DriverLicense).WithIssuer("Issuer").WithNumber("Number")
                .Build();

            Assert.AreEqual(PersonIDKind.DriverLicense, sut.Kind);
            Assert.AreEqual("Issuer", sut.Issuer);
            Assert.AreEqual("Number", sut.Number);

        }
    }
}