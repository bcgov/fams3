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

namespace BcGov.Fams3.Redis.Test
{
    public class CacheServiceTest
    {
        private Mock<IDistributedCache> _distributedCacheMock;
        private Mock<ILogger<CacheService>> _loggerMock;
        private ICacheService _sut;
        private Guid _existedReqestGuid;
        private Guid _nonExistedReqestGuid;
        private SearchRequest _validSearchRequest;
        private string _validRequestString;
        byte[] bytesSearch = null;

        [SetUp]
        public void SetUp()
        {
            _existedReqestGuid = Guid.Parse("6AE89FE6-9909-EA11-B813-00505683FBF4");
            _nonExistedReqestGuid = Guid.Parse("66666666-9909-EA11-B813-00505683FBF4");
            

            _validSearchRequest = new SearchRequest() { Person = null, SearchRequestId = _existedReqestGuid };
            _validRequestString = JsonConvert.SerializeObject(_validSearchRequest);

            _distributedCacheMock = new Mock<IDistributedCache>();

            bytesSearch = Encoding.UTF8.GetBytes(_validRequestString);
            _distributedCacheMock.Setup(x => x.SetAsync(_existedReqestGuid.ToString(),
                It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Verifiable();

            _distributedCacheMock.Setup(x => x.SetAsync("key",
      It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
      .Verifiable();

            _distributedCacheMock.Setup(x => x.GetAsync(_existedReqestGuid.ToString(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(bytesSearch));

            _distributedCacheMock.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Encoding.ASCII.GetBytes("data")));

            _distributedCacheMock.Setup(x => x.RemoveAsync(_existedReqestGuid.ToString(),
                It.IsAny<CancellationToken>()))
               .Verifiable();

            _distributedCacheMock.Setup(x => x.RemoveAsync("key",
                It.IsAny<CancellationToken>()))
               .Verifiable();

            _distributedCacheMock.Setup(x => x.GetAsync(_nonExistedReqestGuid.ToString(), It.IsAny<CancellationToken>()))
         .Returns(Task.FromResult<byte[]>(null));

            _distributedCacheMock.Setup(x => x.RemoveAsync(_nonExistedReqestGuid.ToString(),
            It.IsAny<CancellationToken>()))
           .Verifiable();


            _loggerMock = new Mock<ILogger<CacheService>>();
         
            _sut = new CacheService(_distributedCacheMock.Object);

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
            _distributedCacheMock.Verify(x => x.SetAsync(_existedReqestGuid.ToString(), 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()), Times.AtLeastOnce ());

        }

        [Test]
        public void with_correct_data_and_key_saveRequest_successfully()
        {
            _sut.SaveRequest("data","key");
            _distributedCacheMock.Verify(x => x.SetAsync("key",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.AtLeastOnce());

        }



        [Test]
        public void with_existed_searchRequestId_deleteRequest_successfully()
        {
            _sut.DeleteRequest(_existedReqestGuid);
            _distributedCacheMock.Verify(x => x.RemoveAsync(_existedReqestGuid.ToString(), new CancellationToken()), Times.Once);
        }

        [Test]
        public void with_nonexisted_serachRequestId_deleteRequest_successfully()
        {
            _sut.DeleteRequest(_nonExistedReqestGuid);
            _distributedCacheMock.Verify(x => x.RemoveAsync(_nonExistedReqestGuid.ToString(), new CancellationToken()), Times.Once);
        }

        [Test]
        public void when_there_null_deleteRequest_throws_it()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.DeleteRequest(new Guid()));
          
        }

        [Test]
        public void when_there_null_GetRequest_throws_it()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetRequest(new Guid()));
        }

        [Test]
        public void with_correct_key_data_save_successfully()
        {
            string key = "key";
            string data = "data";
            _sut.Save(key, data);
            _distributedCacheMock.Verify(x => x.SetAsync(key,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.AtLeastOnce());
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
            Assert.AreEqual("", data);
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
    }
}
