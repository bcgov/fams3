using BcGov.Fams3.Redis.Model;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;
using Newtonsoft.Json;


namespace BcGov.Fams3.Redis.Test
{
    public class CacheServiceTest
    {
        private Mock<IDatabase> _databaseMock;
        private ICacheService _sut;
        private Guid _existedReqestGuid;
        private Guid _nonExistedReqestGuid;
        private SearchRequest _validSearchRequest;
        [SetUp]
        public void SetUp()
        {
            _existedReqestGuid = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
            _nonExistedReqestGuid = Guid.Parse("66666666-9909-EA11-B813-00505683FBF4");

            _validSearchRequest = new SearchRequest() { Person = null, Providers = null, SearchRequestId = _existedReqestGuid };
            string validSearchRequestStr = JsonConvert.SerializeObject(_validSearchRequest);

            _databaseMock = new Mock<IDatabase>();
            _databaseMock.Setup(db => db.StringGet(It.Is<RedisKey>(m=>m== _existedReqestGuid.ToString()), It.IsAny<CommandFlags>()))
                         .Returns(validSearchRequestStr);

            _databaseMock.Setup(db => db.StringGet(It.Is<RedisKey>(m => m == _nonExistedReqestGuid.ToString()), It.IsAny<CommandFlags>()))
             .Returns("");

            _databaseMock.Setup(db => db.StringSet(It.Is<RedisKey>(m => m == _existedReqestGuid.ToString()), It.IsAny<RedisValue>(),null,When.Always, CommandFlags.None))
             .Returns(true);

            _databaseMock.Setup(db => db.KeyDelete(It.Is<RedisKey>(m => m == _existedReqestGuid.ToString()),CommandFlags.None))
            .Returns(true);

            _databaseMock.Setup(db => db.KeyDelete(It.Is<RedisKey>(m => m == _nonExistedReqestGuid.ToString()), CommandFlags.None))
            .Returns(true);

            _sut = new CacheService(_databaseMock.Object);
        }

        [Test]
        public void with_existed_serachRequestId_getRequest_return_SearchRequest()
        {
            SearchRequest sr = _sut.GetRequest(_existedReqestGuid);
            Assert.AreEqual(_existedReqestGuid, sr.SearchRequestId);
        }

        [Test]
        public void with_nonexisted_searchRequestId_getRequest_return_null()
        {
            SearchRequest sr = _sut.GetRequest(_nonExistedReqestGuid);
            Assert.AreEqual(null, sr);
        }

        [Test]
        public void with_correct_searchRequest_saveRequest_successfully()
        {
            bool result = _sut.SaveRequest(_validSearchRequest);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void with_null_searchRequest_saveRequest_retrun_false()
        {
            bool result = _sut.SaveRequest(null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void with_existed_searchRequestId_deleteRequest_successfully()
        {
            bool result = _sut.DeleteRequest(_existedReqestGuid);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void with_nonexisted_serachRequestId_deleteRequest_successfully()
        {
            bool result = _sut.DeleteRequest(_nonExistedReqestGuid);
            Assert.AreEqual(true, result);
        }
    }
}
