using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.Register
{
    public class SearchRequestRegisterTest
    {
        private SearchRequestRegister _sut;
        private readonly Mock<ICacheService> _cacheServiceMock = new Mock<ICacheService>();
        private Guid _validSearchApiRequestGuid;
        private Guid _wrongSearchApiRequestGuid;
        private SSG_SearchApiRequest _fakeRequest;

        [SetUp]
        public void Init()
        {
            _validSearchApiRequestGuid = Guid.NewGuid();
            _wrongSearchApiRequestGuid = Guid.NewGuid();

            _fakeRequest = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
            };

            _cacheServiceMock.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<object>()))
             .Returns(Task.FromResult(true));

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m.ToString() == _validSearchApiRequestGuid.ToString())))
             .Returns(Task.FromResult(JsonConvert.SerializeObject(_fakeRequest)));

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m.ToString() == _wrongSearchApiRequestGuid.ToString())))
             .Returns(Task.FromResult(""));

            _sut = new SearchRequestRegister(_cacheServiceMock.Object);
        }

        [Test]
        public void duplicated_Identifier_searchapiRequest_filtered_correctly()
        {
            Guid identifier1Guid = Guid.NewGuid();
            Guid identifier2Guid = Guid.NewGuid();
            Guid identifier3Guid = Guid.NewGuid();
            SSG_SearchApiRequest request = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
                Identifiers = new List<SSG_Identifier_WithGuid>()
                    {
                        new SSG_Identifier_WithGuid()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC",
                            IdentifierId=identifier1Guid
                        },
                        new SSG_Identifier_WithGuid()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="bc",
                            IdentifierId=identifier2Guid
                        },
                        new SSG_Identifier_WithGuid()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BirthCertificate.Value,
                            IssuedBy="bc",
                            IdentifierId=identifier3Guid
                        }
                    }.ToArray()
            };
            SSG_SearchApiRequest newRequest = _sut.FilterDuplicatedIdentifier(request);
            Assert.AreEqual("1234567", newRequest.Identifiers[0].Identification);
            Assert.AreEqual(IdentificationType.BCDriverLicense.Value, newRequest.Identifiers[0].IdentifierType);
            Assert.AreEqual("bc", newRequest.Identifiers[0].IssuedBy.ToLower());
            Assert.AreEqual(identifier1Guid, newRequest.Identifiers[0].IdentifierId);
            Assert.AreEqual("1234567", newRequest.Identifiers[1].Identification);
            Assert.AreEqual(IdentificationType.BirthCertificate.Value, newRequest.Identifiers[1].IdentifierType);
            Assert.AreEqual("bc", newRequest.Identifiers[1].IssuedBy.ToLower());
            Assert.AreEqual(identifier3Guid, newRequest.Identifiers[1].IdentifierId);
            Assert.AreEqual(2, newRequest.Identifiers.Length);
        }

        [Test]
        public void unique_Identifier_searchapiRequest_filtered_correctly()
        {
            SSG_SearchApiRequest request = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
                Identifiers = new List<SSG_Identifier_WithGuid>()
                    {
                        new SSG_Identifier_WithGuid()
                        {
                            Identification="333123456",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC",
                            IdentifierId=Guid.NewGuid()
                        },
                        new SSG_Identifier_WithGuid()
                        {
                            Identification = "1234567",
                            IdentifierType = IdentificationType.BCDriverLicense.Value,
                            IssuedBy = "bc",
                            IdentifierId=Guid.NewGuid()
                        },
                        new SSG_Identifier_WithGuid()
                        {
                            Identification = "1234567",
                            IdentifierType = IdentificationType.BirthCertificate.Value,
                            IssuedBy = "bc",
                            IdentifierId=Guid.NewGuid()
                        }
                }.ToArray()
            };
            SSG_SearchApiRequest newRequest = _sut.FilterDuplicatedIdentifier(request);
            Assert.AreEqual(3, newRequest.Identifiers.Length);
        }

        [Test]
        public async Task valid_searchapiRequest_added_to_cache_correctly()
        {
            bool result = await _sut.RegisterSearchApiRequest(_fakeRequest);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task null_searchapiRequest_wont_add_to_cache()
        {
            bool result = await _sut.RegisterSearchApiRequest(null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public async Task valid_id_get_SearchApiRequest_from_cache_correctly()
        {
            SSG_SearchApiRequest result = await _sut.GetSearchApiRequest(_validSearchApiRequestGuid);
            Assert.AreEqual("111111", result.SearchRequest.FileId);
        }

        [Test]
        public async Task wrong_guid_wont_get_searchApiRequest()
        {
            SSG_SearchApiRequest result = await _sut.GetSearchApiRequest(_wrongSearchApiRequestGuid);
            Assert.AreEqual(null, result);
        }
    }
}

