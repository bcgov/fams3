using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Register;
using Fams3Adapter.Dynamics.DataProvider;
using Fams3Adapter.Dynamics.Identifier;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Fams3Adapter.Dynamics.Types;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.Test.Register
{
    public class SearchRequestRegisterTest
    {
        private SearchRequestRegister _sut;
        private readonly Mock<ICacheService> _cacheServiceMock = new Mock<ICacheService>();
        private Guid _validSearchApiRequestGuid;
        private Guid _validSouceIdentifierGuid;
        private Guid _wrongSearchApiRequestGuid;
        private Guid _wrongSourceIdentifierGuid;
        private string _validSearchRequestKey;
        private string _invalidSearchRequestKey;
        private SSG_SearchApiRequest _fakeRequest;
        private PersonalIdentifier _fakeIdentifier;
        private PersonalIdentifier _wrongIdentifier;
        private Mock<ILogger<SearchRequestRegister>> _loggerMock;
        private string adaptorName = "ICBC";
        private int retries = 10;
        private Mock<IDataPartnerService> dataProviderMock = new Mock<IDataPartnerService>();

        [SetUp]
        public void Init()
        {
            _validSearchApiRequestGuid = Guid.NewGuid();
            _validSouceIdentifierGuid = Guid.NewGuid();
            _wrongSearchApiRequestGuid = Guid.NewGuid();
            _wrongSourceIdentifierGuid = Guid.NewGuid();
            _validSearchRequestKey = "1234_0000";
            _invalidSearchRequestKey = "1234_7777";

            dataProviderMock.Setup(x => x.GetAllDataProviders(It.IsAny<CancellationToken>()))
              .Returns(Task.FromResult<IEnumerable<SSG_DataProvider>>(

                  new List<SSG_DataProvider>()
                  {
                      new SSG_DataProvider()
                      {
                        AdaptorName = adaptorName,
                        NumberOfDaysToRetry =retries,
                        SearchSpeed = AutoSearchSpeedType.Fast.Value,
                        TimeBetweenRetries = 60,
                        NumberOfRetries = 3

                      }
                  }
               ));

            _fakeRequest = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
                Identifiers = new List<SSG_Identifier>()
                {
                    new SSG_Identifier() { 
                        Identification = "1234567", 
                        IdentifierType = IdentificationType.BirthCertificate.Value,
                        IdentifierId = _validSouceIdentifierGuid
                    }
                }.ToArray()
            };

            _fakeIdentifier = new PersonalIdentifier
            {
                Value = "1234567",
                Type = PersonalIdentifierType.BirthCertificate
            };

            _wrongIdentifier = new PersonalIdentifier
            {
                Value ="7654321",
                Type= PersonalIdentifierType.BCDriverLicense
            };

            _cacheServiceMock.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<object>()))
             .Returns(Task.FromResult(true));

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m.ToString() == Keys.REDIS_KEY_PREFIX+_validSearchApiRequestGuid.ToString())))
             .Returns(Task.FromResult(JsonConvert.SerializeObject(_fakeRequest)));

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m.ToString() == Keys.REDIS_KEY_PREFIX + Keys.DATA_PROVIDER_KEY)))
             .Returns(Task.FromResult(JsonConvert.SerializeObject(new List<SSG_DataProvider>()
                  {
                      new SSG_DataProvider()
                      {
                        AdaptorName = adaptorName,
                        NumberOfDaysToRetry =retries,
                        SearchSpeed = AutoSearchSpeedType.Fast.Value,
                        TimeBetweenRetries = 60,
                        NumberOfRetries = 3

                      }
                  }.ToArray())));

         

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m == $"{Keys.REDIS_KEY_PREFIX}{_validSearchRequestKey}")))
            .Returns(Task.FromResult(JsonConvert.SerializeObject(_fakeRequest)));

            _cacheServiceMock.Setup(x => x.Delete(It.Is<string>(m => m.ToString() == Keys.REDIS_KEY_PREFIX + _validSearchApiRequestGuid.ToString())))
            .Returns(Task.FromResult(true));

            _cacheServiceMock.Setup(x => x.Delete(It.Is<string>(m => m.ToString() == $"{Keys.REDIS_KEY_PREFIX}{_validSearchRequestKey}")))
            .Returns(Task.FromResult(true));

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m.ToString() == Keys.REDIS_KEY_PREFIX + _wrongSearchApiRequestGuid.ToString())))
             .Returns(Task.FromResult(""));

            _cacheServiceMock.Setup(x => x.Get(It.Is<string>(m => m.ToString() == $"{Keys.REDIS_KEY_PREFIX}{_invalidSearchRequestKey}")))
             .Returns(Task.FromResult(""));

            _loggerMock = new Mock<ILogger<SearchRequestRegister>>();
            _sut = new SearchRequestRegister(_cacheServiceMock.Object, _loggerMock.Object, dataProviderMock.Object);
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
                Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC",
                            IdentifierId=identifier1Guid
                        },
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="bc",
                            IdentifierId=identifier2Guid
                        },
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IdentifierType=IdentificationType.BirthCertificate.Value,
                            IssuedBy="bc",
                            IdentifierId=identifier3Guid
                        },
                        new SSG_Identifier()
                        {
                            Identification="12340000",
                            IdentifierType=IdentificationType.BirthCertificate.Value,
                            IssuedBy="bc",
                            IsCouldNotLocate=true
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
        public void couldNotLocate_Identifier_searchapiRequest_filtered_correctly()
        {
            Guid identifier1Guid = Guid.NewGuid();
            SSG_SearchApiRequest request = new SSG_SearchApiRequest
            {
                SearchRequest = new SSG_SearchRequest() { FileId = "111111" },
                Identifiers = new List<SSG_Identifier>()
                    {
                        new SSG_Identifier()
                        {
                            Identification="1234567",
                            IsCouldNotLocate=true,
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC",
                            IdentifierId=identifier1Guid
                        }
                       
                    }.ToArray()
            };
            SSG_SearchApiRequest newRequest = _sut.FilterDuplicatedIdentifier(request);

            Assert.AreEqual(0, newRequest.Identifiers.Length);
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
                            Identification="333123456",
                            IdentifierType=IdentificationType.BCDriverLicense.Value,
                            IssuedBy="BC",
                            IdentifierId=Guid.NewGuid()
                        },
                        new SSG_Identifier()
                        {
                            Identification = "1234567",
                            IdentifierType = IdentificationType.BCDriverLicense.Value,
                            IssuedBy = "bc",
                            IdentifierId=Guid.NewGuid()
                        },
                        new SSG_Identifier()
                        {
                            Identification = "1234567",
                            IdentifierType = IdentificationType.BCDriverLicense.Value,
                            IssuedBy = "request",
                            IdentifierId=Guid.NewGuid()
                        },
                        new SSG_Identifier()
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
        public async Task valid_fileId_seqNumber_get_SearchApiRequest_from_cache_correctly()
        {
            SSG_SearchApiRequest result = await _sut.GetSearchApiRequest(_validSearchRequestKey);
            Assert.AreEqual("111111", result.SearchRequest.FileId);
        }

        [Test]
        public async Task valid_register_data_providers_list_to_cache_correctly()
        {
            var result = await _sut.RegisterDataProviders((new List<SSG_DataProvider>() { new SSG_DataProvider { } }).ToArray());
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task valid_key_should_return_data_providers_list_from_cache()
        {
            var result = await _sut.GetDataProvidersList();
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task non_registerd_should_call_dynamics_to_pull_list_and_save_in_cache()
        {
            _cacheServiceMock.Setup(x => x.Get(It.IsAny<string>()))
          .Returns(Task.FromResult(""));
            var result = await _sut.GetDataProvidersList();
            Assert.AreEqual(1, result.Count());
            dataProviderMock.Verify(x => x.GetAllDataProviders(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }


        [Test]
        public async Task wrong_guid_wont_get_searchApiRequest()
        {
            SSG_SearchApiRequest result = await _sut.GetSearchApiRequest(_wrongSearchApiRequestGuid);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task wrong_fileId_wont_get_searchApiRequest()
        {
            SSG_SearchApiRequest result = await _sut.GetSearchApiRequest(_invalidSearchRequestKey);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task correct_sourceId_will_get_correct_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(_fakeIdentifier, _validSearchApiRequestGuid);
            Assert.AreEqual(IdentificationType.BirthCertificate.Value, result.IdentifierType);
            Assert.AreEqual("1234567", result.Identification);
            Assert.AreEqual(_validSouceIdentifierGuid, result.IdentifierId);
        }

        [Test]
        public async Task correct_sourceId_validFileId_will_get_correct_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(_fakeIdentifier, _validSearchRequestKey);
            Assert.AreEqual(IdentificationType.BirthCertificate.Value, result.IdentifierType);
            Assert.AreEqual("1234567", result.Identification);
            Assert.AreEqual(_validSouceIdentifierGuid, result.IdentifierId);
        }

        [Test]
        public async Task wrong_sourceId_will_get_null_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(_wrongIdentifier, _validSearchApiRequestGuid);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task null_sourceId_will_get_null_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(null, _validSearchApiRequestGuid);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task wrong_sourceId_validFileId_will_get_null_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(_wrongIdentifier, _validSearchRequestKey);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task null_sourceId_validSearchRequestKey_will_get_null_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(null, _validSearchRequestKey);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task wrong_searchApiRequestId_will_get_null_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(_fakeIdentifier, _wrongSearchApiRequestGuid);
            _loggerMock.VerifyLog(LogLevel.Error, "Cannot find the searchApiRequest in Redis Cache.", Times.Once());
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task wrong_fileId_will_get_null_ssgIdentifier()
        {
            SSG_Identifier result = await _sut.GetMatchedSourceIdentifier(_fakeIdentifier,_invalidSearchRequestKey);
            _loggerMock.VerifyLog(LogLevel.Error, "Cannot find the searchApiRequest in Redis Cache.", Times.Once());
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task valid_searchApiRequestId_will_be_removed_successfully()
        {
            bool result = await _sut.RemoveSearchApiRequest(_validSearchApiRequestGuid);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task valid_searchApiRequest_will_be_removed_successfully()
        {
            bool result = await _sut.RemoveSearchApiRequest(_fakeRequest);
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task valid_fileId_seqNumber_request_will_be_removed_successfully()
        {
            bool result = await _sut.RemoveSearchApiRequest(_validSearchRequestKey);
            Assert.AreEqual(true, result);
        }
    }
}

