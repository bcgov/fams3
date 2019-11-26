using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using NUnit.Framework;
using Simple.OData.Client;

namespace Fams3Adapter.Dynamics.Test.SearchApiRequest
{
    public class PersonSearchRequestBuilderTest
    {
        private PersonSearchRequestBuilder _sut;
        private readonly Mock<SSG_SearchApiRequest> _ssG_SearchApiRequestMock = new Mock<SSG_SearchApiRequest>();

        [SetUp]
        public void SetUp()
        {
            _sut = new PersonSearchRequestBuilder();
            _ssG_SearchApiRequestMock.Object.PersonGivenName = "personGivenName";
            _ssG_SearchApiRequestMock.Object.PersonSurname = "personSurName";
            _ssG_SearchApiRequestMock.Object.Name = "test";
            List<SSG_Identifier> identifers = new List<SSG_Identifier>()
            {
                new SSG_Identifier()
                {
                    Identification="1111111",
                    IdentifierType=867670009
                }
            };
            _ssG_SearchApiRequestMock.Object.Identifiers = identifers.ToArray();
        }


        [Test]
        public void it_should_build_a_person_search_request()
        {
            var personSearchRequest = _sut.WithSearchApiRequest(_ssG_SearchApiRequestMock.Object).Build();
            Assert.AreEqual(1, personSearchRequest.Identifiers.Count());
            Assert.AreEqual("personGivenName", personSearchRequest.FirstName);
            Assert.AreEqual("personSurName", personSearchRequest.LastName);
            Assert.AreEqual(5, personSearchRequest.Identifiers.First().IdentifierType);
            Assert.AreEqual("1111111", personSearchRequest.Identifiers.First().Identification);
        }

    }
}