using DynamicsAdapter.Web.SearchRequest;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using NUnit.Framework;
using System.Collections.Generic;

namespace DynamicsAdapter.Web.Test.SearchRequest
{
    public class SearchRequestRegisterTest
    {
        SearchRequestRegister _sut;

        [SetUp]
        public void Init()
        {
            _sut = new SearchRequestRegister();

        }

        [Test]
        public void duplicated_Identifier_searchapiRequest_filtered_correctly()
        {
            SSG_SearchApiRequest request = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
                Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC"
                        },
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="bc"
                        },
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BirthCertificate.Value,
                            IssuedBy="bc"
                        }
                    }.ToArray()
            };
            SSG_SearchApiRequest newRequest = _sut.FilterDuplicatedIdentifier(request);
            Assert.AreEqual("1234567", newRequest.Identifiers[0].Identification);
            Assert.AreEqual(IdentificationType.BCDriverLicense.Value, newRequest.Identifiers[0].IdentifierType);
            Assert.AreEqual("bc", newRequest.Identifiers[0].IssuedBy.ToLower());
            Assert.AreEqual("1234567", newRequest.Identifiers[1].Identification);
            Assert.AreEqual(IdentificationType.BirthCertificate.Value, newRequest.Identifiers[1].IdentifierType);
            Assert.AreEqual("bc", newRequest.Identifiers[1].IssuedBy.ToLower());
            Assert.AreEqual(2, newRequest.Identifiers.Length);
        }

        [Test]
        public void unique_Identifier_searchapiRequest_filtered_correctly()
        {
            SSG_SearchApiRequest request = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
                Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="123456",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC"
                        },
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="bc"
                        },
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BirthCertificate.Value,
                            IssuedBy="bc"
                        }
                    }.ToArray()
            };
            SSG_SearchApiRequest newRequest = _sut.FilterDuplicatedIdentifier(request);
            Assert.AreEqual(3, newRequest.Identifiers.Length);
        }
    }
}

