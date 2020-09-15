using BcGov.Fams3.SearchApi.Contracts.Person;
using NUnit.Framework;
using SearchApi.Web.DeepSearch;
using System.Collections.Generic;

namespace SearchApi.Web.Test.DeepSearch
{ 
    public class RegistryTest
    {
        [Test]
        public void should_return_type_list_identifier()
        {
            Assert.IsInstanceOf<List<PersonalIdentifierType>>(Registry.DataPartnerParameters["ICBC"]);
        }

    }
}