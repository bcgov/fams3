using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using NUnit.Framework;
using SearchApi.Web.Controllers;

namespace SearchApi.Web.Test.People
{
    public class PersonSearchRequestTest
    {
        [Test]
        public void With_args_it_should_create()
        {


            var sut = new PersonSearchRequest("firstName", "lastName", new DateTime(2001, 1, 12), new List<PersonalIdentifier>(), new List<Address>(), new List<Phone>(), new List<Name>(),  new List<RelatedPerson>(), new List<Employment>(), new List<DataProvider>() );

            Assert.AreEqual("firstName", sut.FirstName);
            Assert.AreEqual("lastName", sut.LastName);
            if (sut.DateOfBirth != null)
            {
                Assert.AreEqual(2001, sut.DateOfBirth.Value.Year);
                Assert.AreEqual(1, sut.DateOfBirth.Value.Month);
                Assert.AreEqual(12, sut.DateOfBirth.Value.Day);
            }
        }

    }
}