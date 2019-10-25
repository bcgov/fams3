using System;
using NUnit.Framework;
using SearchApi.Web.Controllers;

namespace SearchApi.Web.Test.People
{
    public class PersonSearchResponseTest
    {
        [Test]
        public void With_args_it_should_create()
        {
            Guid expectedId = Guid.NewGuid();

            var sut = new PersonSearchResponse(expectedId);

            Assert.AreEqual(expectedId, sut.Id);

        }
    }
}