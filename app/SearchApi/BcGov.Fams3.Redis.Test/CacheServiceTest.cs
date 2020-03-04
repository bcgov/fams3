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
        private Mock<IRedisConnectionFactory> _factoryMock;
        private Mock<IRedisConnectionFactory> _factoryExceptionMock;
        private ICacheService _sut;
        private Guid _existedReqestGuid;
        private Guid _nonExistedReqestGuid;
        private Guid _exceptionReqestGuid;
        private SearchRequest _validSearchRequest;

        [SetUp]
        public void SetUp()
        {
            _existedReqestGuid = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
            _nonExistedReqestGuid = Guid.Parse("66666666-9909-EA11-B813-00505683FBF4");
            _exceptionReqestGuid = Guid.Parse("6AE89FE6-9909-EA11-B813-55555683FBF4"); ;

            _validSearchRequest = new SearchRequest() { Person = null, Providers = null, SearchRequestId = _existedReqestGuid };
            string validSearchRequestStr = JsonConvert.SerializeObject(_validSearchRequest);

            _databaseMock = new Mock<IDatabase>();
            _databaseMock.Setup(db => db.StringGet(It.Is<RedisKey>(m=>m== _existedReqestGuid.ToString()), It.IsAny<CommandFlags>()))
                         .Returns(validSearchRequestStr);

            _databaseMock.Setup(db => db.StringGet(It.Is<RedisKey>(m => m == _nonExistedReqestGuid.ToString()), It.IsAny<CommandFlags>()))
             .Returns("");

            _databaseMock.Setup(db => db.StringGet(It.Is<RedisKey>(m => m == _exceptionReqestGuid.ToString()), It.IsAny<CommandFlags>()))
              .Throws(new RedisException("timeout exception"));

            _databaseMock.Setup(db => db.StringSet(It.Is<RedisKey>(m => m == _existedReqestGuid.ToString()), It.IsAny<RedisValue>(),null,When.Always, CommandFlags.None))
             .Returns(true);

            _databaseMock.Setup(db => db.StringSet(It.Is<RedisKey>(m => m == _exceptionReqestGuid.ToString()), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Throws(new RedisException("timeout exception"));

            _databaseMock.Setup(db => db.KeyDelete(It.Is<RedisKey>(m => m == _existedReqestGuid.ToString()),CommandFlags.None))
            .Returns(true);

            _databaseMock.Setup(db => db.KeyDelete(It.Is<RedisKey>(m => m == _nonExistedReqestGuid.ToString()), CommandFlags.None))
            .Returns(true);

            _databaseMock.Setup(db => db.KeyDelete(It.Is<RedisKey>(m => m == _exceptionReqestGuid.ToString()), CommandFlags.None))
             .Throws(new RedisException("timeout exception"));

            _factoryExceptionMock = new Mock<IRedisConnectionFactory>();
            _factoryExceptionMock.Setup(factory => factory.OpenConnection()).Throws(new RedisException("fail in connecting to redis ") );

            _factoryMock = new Mock<IRedisConnectionFactory>();
            _factoryMock.Setup(factory => factory.GetDatabase()).Returns(_databaseMock.Object);

            _sut = new CacheService(_factoryMock.Object);
        }

        [Test]
        public void fail_redis_connectin_throws_redis_exception()
        {
            Assert.Throws<RedisException>( ()=> { new CacheService(_factoryExceptionMock.Object); });
        }


        [Test]
        public void with_existed_serachRequestId_getRequest_return_SearchRequest()
        {
            SearchRequest sr = _sut.GetRequest(_existedReqestGuid).Result;
            Assert.AreEqual(_existedReqestGuid, sr.SearchRequestId);
        }

        [Test]
        public void with_nonexisted_searchRequestId_getRequest_return_null()
        {
            SearchRequest sr = _sut.GetRequest(_nonExistedReqestGuid).Result;
            Assert.AreEqual(null, sr);
        }

        [Test]
        public void when_there_has_redis_exception_getRequest_throws_it()
        {
            Assert.Throws<RedisException>(()=>_sut.GetRequest(_exceptionReqestGuid));
        }

        [Test]
        public void with_correct_searchRequest_saveRequest_successfully()
        {
            bool result = _sut.SaveRequest(_validSearchRequest).Result;
            Assert.AreEqual(true, result);
        }

        [Test]
        public void with_null_searchRequest_saveRequest_retrun_false()
        {
            bool result = _sut.SaveRequest(null).Result;
            Assert.AreEqual(false, result);
        }

        [Test]
        public void when_there_has_redis_exception_saveRequest_throws_it()
        {
            Assert.Throws<RedisException>(() => _sut.SaveRequest(new SearchRequest() { SearchRequestId=_exceptionReqestGuid, Person = null, Providers = null }));
        }

        [Test]
        public void with_existed_searchRequestId_deleteRequest_successfully()
        {
            bool result = _sut.DeleteRequest(_existedReqestGuid).Result;
            Assert.AreEqual(true, result);
        }

        [Test]
        public void with_nonexisted_serachRequestId_deleteRequest_successfully()
        {
            bool result = _sut.DeleteRequest(_nonExistedReqestGuid).Result;
            Assert.AreEqual(true, result);
        }

        [Test]
        public void when_there_has_redis_exception_deleteRequest_throws_it()
        {
            Assert.Throws<RedisException>(() => _sut.DeleteRequest(_exceptionReqestGuid));
        }
    }
}
