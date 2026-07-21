using BcGov.Fams3.Redis.Model;
using Moq;
using NUnit.Framework;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace BcGov.Fams3.Redis.Test
{
    public class CacheServiceTest
    {
        private Mock<IDistributedCache> _distributedCacheMock;
        private Mock<IRedisCacheClient> _stackRedisCacheClientMock;
        private Mock<IRedisDatabase> _mockRedisDB;
        private Mock<IDatabase> _mockDatabase;
        private Mock<ITransaction> _mockTransaction;
        private Mock<ILogger<CacheService>> _loggerMock;
        private ICacheService _sut;
        private string _existedRequestKey;
        private string _nonExistedRequestKey;
        private SearchRequest _validSearchRequest;
        private string _validRequestString;
        byte[] bytesSearch = null;

        [SetUp]
        public void SetUp()
        {
            _existedRequestKey = "111111_000000";
            _nonExistedRequestKey = "222222_111113";
            

            _validSearchRequest = new SearchRequest() { Person = null, SearchRequestId = Guid.NewGuid(), SearchRequestKey=_existedRequestKey };
            _validRequestString = JsonConvert.SerializeObject(_validSearchRequest);

            _distributedCacheMock = new Mock<IDistributedCache>();

            _stackRedisCacheClientMock = new Mock<IRedisCacheClient>();
            _mockRedisDB = new Mock<IRedisDatabase>();
            bytesSearch = Encoding.UTF8.GetBytes(_validRequestString);
            _distributedCacheMock.Setup(x => x.SetAsync(_existedRequestKey,
                It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Verifiable();

           // _distributedCacheMock.Setup(x => x.Get(It.IsAny<string>())).Returns(Task.FromResult("data"));


            _distributedCacheMock.Setup(x => x.SetAsync("key",
      It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
      .Verifiable();

            _distributedCacheMock.Setup(x => x.GetAsync(_existedRequestKey, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(bytesSearch));

            _distributedCacheMock.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Encoding.ASCII.GetBytes("data")));

            _distributedCacheMock.Setup(x => x.RemoveAsync(_existedRequestKey,
                It.IsAny<CancellationToken>()))
               .Verifiable();

            _distributedCacheMock.Setup(x => x.RemoveAsync("key",
                It.IsAny<CancellationToken>()))
               .Verifiable();

            _distributedCacheMock.Setup(x => x.GetAsync(_nonExistedRequestKey, It.IsAny<CancellationToken>()))
         .Returns(Task.FromResult<byte[]>(null));

            _distributedCacheMock.Setup(x => x.RemoveAsync(_nonExistedRequestKey,
            It.IsAny<CancellationToken>()))
           .Verifiable();

            _stackRedisCacheClientMock.Setup(x => x.Db0).Returns(_mockRedisDB.Object);

            _mockRedisDB.Setup(x =>
                x.AddAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>(),
                    It.IsAny<HashSet<string>>()))
                .ReturnsAsync(true);

            _mockRedisDB.Setup(x =>
                x.AddAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>(),
                    It.IsAny<HashSet<string>>()))
                .ReturnsAsync(true);

            _mockRedisDB.Setup(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<object>(), default, default)).Returns(Task.FromResult(true));
            _mockRedisDB.Setup(x => x.SearchKeysAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<string> { "first","second"}.AsEnumerable()));
            _mockRedisDB.Setup(x => x.SearchKeysAsync("test")).Returns(Task.FromResult(new List<string> {  }.AsEnumerable()));
            _mockRedisDB.Setup(x => x.GetAsync<string>("key", default)).Returns(Task.FromResult("data"));
            //_stackRedisCacheClient.Db0.GetAsync<string>(key)

            _mockDatabase = new Mock<IDatabase>();
            _mockRedisDB.Setup(x => x.Database).Returns(_mockDatabase.Object);

            _mockTransaction = new Mock<ITransaction>();
            _mockDatabase.Setup(x => x.CreateTransaction(It.IsAny<object>())).Returns(_mockTransaction.Object);
            _mockTransaction.Setup(x => x.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);
            _mockTransaction.Setup(x => x.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
            _mockTransaction.Setup(x => x.HashDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

            _loggerMock = new Mock<ILogger<CacheService>>();

            _sut = new CacheService(_distributedCacheMock.Object, _stackRedisCacheClientMock.Object, _loggerMock.Object);
        }

        [Test]
        public void with_existed_serachRequestKey_getRequest_return_SearchRequest()
        {
            SearchRequest sr = _sut.GetRequest(_existedRequestKey).Result;
            Assert.AreEqual(_existedRequestKey, sr.SearchRequestKey);
        }

        [Test]
        public void with_nonexisted_searchRequestId_getRequest_return_null()
        {
            SearchRequest sr = _sut.GetRequest(_nonExistedRequestKey).Result;
            Assert.AreEqual(null, sr);
        }

        [Test]
        public void save_request_throws_Exception_with_null_id()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SaveRequest(new SearchRequest()));
        }

        [Test]
        public void save_request_throws_Exception_with_null_object()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SaveRequest(null));
        }

        [Test]
        public void with_correct_searchRequest_saveRequest_successfully()
        {
            _sut.SaveRequest(_validSearchRequest);
            _distributedCacheMock.Verify(x => x.SetAsync(_existedRequestKey, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()), Times.AtLeastOnce ());

        }

        [Test]
        public void with_correct_data_and_key_saveString_in_cache_successfully()
        {
            _sut.SaveString("data","key");
            _distributedCacheMock.Verify(x => x.SetAsync("key",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.AtLeastOnce());

        }

        [Test]
        public async Task with_correct_data_and_key_gettring_in_cache_with_expiry_successfully()
        {
            await _sut.SaveString("data", "key", TimeSpan.FromSeconds(10));
            var data = await _sut.GetString("key");
            _distributedCacheMock.Verify(x => x.GetAsync("key", It.IsAny<CancellationToken>()), Times.AtLeastOnce());

        }

        [Test]
        public async Task with_correct_data_and_key_gettring_in_cache_successfully()
        {
          await   _sut.SaveString("data", "key");
            var data = await _sut.GetString("key");
            _distributedCacheMock.Verify(x => x.GetAsync("key", It.IsAny<CancellationToken>()), Times.AtLeastOnce());

        }

        [Test]
        public async Task search_request_keys_from_patter()
        {
          var items =  await _sut.SearchKeys("data");
            _mockRedisDB.Verify(x => x.SearchKeysAsync(It.IsAny<string>()), Times.AtLeastOnce());
            Assert.AreEqual(2, items.Count());
        }

        [Test]
        public async Task search_request_keys_from_pattern_returns_0_count()
        {
            var items = await _sut.SearchKeys("test");
            _mockRedisDB.Verify(x => x.SearchKeysAsync(It.IsAny<string>()), Times.AtLeastOnce());
            Assert.AreEqual(0, items.Count());
        }



        [Test]
        public void with_existed_searchRequestId_deleteRequest_successfully()
        {
            _sut.DeleteRequest(_existedRequestKey);
            _distributedCacheMock.Verify(x => x.RemoveAsync(_existedRequestKey, new CancellationToken()), Times.Once);
        }

        [Test]
        public void with_nonexisted_serachRequestId_deleteRequest_successfully()
        {
            _sut.DeleteRequest(_nonExistedRequestKey);
            _distributedCacheMock.Verify(x => x.RemoveAsync(_nonExistedRequestKey, new CancellationToken()), Times.Once);
        }

        [Test]
        public void when_there_null_deleteRequest_throws_it()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.DeleteRequest(""));
          
        }

        [Test]
        public void when_there_null_GetRequest_throws_it()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetRequest(""));
        }

        [Test]
        public void with_correct_key_data_save_successfully()
        {
            string key = "key";
            string data = "data";
            _sut.Save(key, data);
            _mockRedisDB.Verify(x =>
                x.AddAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<StackExchange.Redis.When>(),
                    It.IsAny<StackExchange.Redis.CommandFlags>(),
                    It.IsAny<HashSet<string>>()),
                Times.AtLeastOnce());
        
        }
        [Test]
        public void with_correct_key_data_update_successfully()
        {
            string key = "key";
            string data = "data";
            _sut.Update(key, data);
            _mockRedisDB.Verify(x => x.ReplaceAsync(It.IsAny<string>(), It.IsAny<object>(), default, default), Times.AtLeastOnce());

        }

        [Test]
        public void with_correct_key_data_save_successfully_with_expiratuon()
        {
            string key = "key";
            string data = "data";
            _sut.Save(key, data, new TimeSpan(0, 0, 2));
            _mockRedisDB.Verify(x =>
                x.AddAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>(),
                    It.IsAny<HashSet<string>>()),
                Times.AtLeastOnce());

        }

       

        [Test]
        public void with_null_key_save_throw_exception()
        {
            string data = "data";
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Save(null, data));
        }

        [Test]
        public void with_null_data_save_throw_exception()
        {
            string key = "key";
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Save(key, null));
        }

        [Test]
        public void with_correct_key_get_successfully()
        {
            string key = "key";
            string data = _sut.Get(key).Result;
            Assert.AreEqual("data", data);
        }

        [Test]
        public void with_incorrect_key_get_empty_string()
        {
            string key = "lala";
            string data = _sut.Get(key).Result;
            Assert.AreEqual(null, data);
        }

        [Test]
        public void with_null_key_get_throw_exception()
        {
            string key = null;
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Get(key));
        }

        [Test]
        public void with_existed_key_delete_successfully()
        {
            string key = "key";
            _sut.Delete(key);
            _distributedCacheMock.Verify(x => x.RemoveAsync(key, new CancellationToken()), Times.Once);
        }

        [Test]
        public void with_nonexisted_key_delete_successfully()
        {
            string key = "lala";
            _sut.Delete(key);
            _distributedCacheMock.Verify(x => x.RemoveAsync(key, new CancellationToken()), Times.Once);
        }

        [Test]
        public void when_null_key_delete_throws_it()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Delete(null));

        }

        [Test]
        public void update_data_partner_complete_status_with_null_key_throws_exception()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateDataPartnerCompleteStatus(null, "ICBC"));
        }

        [Test]
        public async Task update_data_partner_complete_status_with_missing_cache_entry_does_nothing()
        {
            string key = "missing-key";
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockDatabase.Verify(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()), Times.Once);
            _mockTransaction.Verify(x => x.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
            _mockTransaction.Verify(x => x.HashDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Never);
        }

        [Test]
        public async Task update_data_partner_complete_status_with_corrupt_data_removes_cache_field()
        {
            string key = "corrupt-key";
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue("null"));

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockTransaction.Verify(x => x.HashDeleteAsync(key, "data", It.IsAny<CommandFlags>()), Times.Once);
            _mockTransaction.Verify(x => x.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
        }

        [Test]
        public async Task update_data_partner_complete_status_with_matching_partner_marks_completed()
        {
            string key = "match-key";
            var searchRequest = new SearchRequest
            {
                SearchRequestKey = key,
                DataPartners = new List<DataPartner>
                {
                    new DataPartner { Name = "ICBC", Completed = false },
                    new DataPartner { Name = "BCHydro", Completed = false }
                }
            };
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue(JsonConvert.SerializeObject(searchRequest)));

            RedisValue capturedValue = RedisValue.Null;
            _mockTransaction
                .Setup(x => x.HashSetAsync(key, "data", It.IsAny<RedisValue>(), When.Always, CommandFlags.None))
                .Callback<RedisKey, RedisValue, RedisValue, When, CommandFlags>((k, f, v, w, flags) => capturedValue = v)
                .ReturnsAsync(true);

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockTransaction.Verify(x => x.HashSetAsync(key, "data", It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Once);
            var updated = JsonConvert.DeserializeObject<SearchRequest>(capturedValue.ToString());
            Assert.IsTrue(updated.DataPartners.First(p => p.Name == "ICBC").Completed);
            Assert.IsFalse(updated.DataPartners.First(p => p.Name == "BCHydro").Completed);
        }

        [Test]
        public async Task update_data_partner_complete_status_with_unknown_partner_still_writes_back_data()
        {
            string key = "unknown-partner-key";
            var searchRequest = new SearchRequest
            {
                SearchRequestKey = key,
                DataPartners = new List<DataPartner>
                {
                    new DataPartner { Name = "ICBC", Completed = false }
                }
            };
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue(JsonConvert.SerializeObject(searchRequest)));

            await _sut.UpdateDataPartnerCompleteStatus(key, "UnknownPartner");

            _mockTransaction.Verify(x => x.HashSetAsync(key, "data", It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Once);
        }

        [Test]
        public async Task update_data_partner_complete_status_retries_when_transaction_fails_then_succeeds()
        {
            string key = "retry-key";
            var searchRequest = new SearchRequest
            {
                SearchRequestKey = key,
                DataPartners = new List<DataPartner> { new DataPartner { Name = "ICBC", Completed = false } }
            };
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue(JsonConvert.SerializeObject(searchRequest)));

            _mockTransaction.SetupSequence(x => x.ExecuteAsync(It.IsAny<CommandFlags>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockDatabase.Verify(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()), Times.Exactly(2));
            _mockTransaction.Verify(x => x.ExecuteAsync(It.IsAny<CommandFlags>()), Times.Exactly(2));
        }

        [Test]
        public async Task update_data_partner_complete_status_exceeds_max_retries_returns_without_throwing()
        {
            string key = "always-fails-key";
            var searchRequest = new SearchRequest
            {
                SearchRequestKey = key,
                DataPartners = new List<DataPartner> { new DataPartner { Name = "ICBC", Completed = false } }
            };
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue(JsonConvert.SerializeObject(searchRequest)));

            _mockTransaction.Setup(x => x.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(false);

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockDatabase.Verify(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()), Times.Exactly(1000));
            _mockTransaction.Verify(x => x.ExecuteAsync(It.IsAny<CommandFlags>()), Times.Exactly(1000));
        }

        [Test]
        public async Task update_data_partner_complete_status_with_corrupt_data_does_not_retry_when_cleanup_transaction_fails()
        {
            string key = "corrupt-key-no-retry";
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue("null"));
            _mockTransaction.Setup(x => x.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(false);

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockDatabase.Verify(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()), Times.Once);
            _mockTransaction.Verify(x => x.HashDeleteAsync(key, "data", It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task update_data_partner_complete_status_with_null_data_partners_collection_still_writes_back_data()
        {
            string key = "null-partners-key";
            var searchRequest = new SearchRequest
            {
                SearchRequestKey = key,
                DataPartners = null
            };
            _mockDatabase.Setup(x => x.HashGetAsync(key, "data", It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue(JsonConvert.SerializeObject(searchRequest)));

            await _sut.UpdateDataPartnerCompleteStatus(key, "ICBC");

            _mockTransaction.Verify(x => x.HashSetAsync(key, "data", It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Once);
        }
    }
}
