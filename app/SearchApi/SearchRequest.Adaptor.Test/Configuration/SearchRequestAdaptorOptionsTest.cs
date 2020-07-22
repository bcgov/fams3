using NUnit.Framework;
using SearchRequestAdaptor.Configuration;
using System.Linq;

namespace SearchRequest.Adaptor.Test.Configuration
{
    public class SearchRequestAdaptorOptionsTest
    {
        [Test]
        public void Should_add_a_new_webHook()
        {

            var options = new SearchRequestAdaptorOptions().AddWebHook("test", "http://example.com/post");

            Assert.AreEqual(1, options.WebHooks.Count);
            Assert.AreEqual("test", options.WebHooks.FirstOrDefault().Name);
            Assert.AreEqual("http://example.com/post", options.WebHooks.FirstOrDefault().Uri);

        }

        [Test]
        public void Should_add_a_new_webHook_even_name_is_null()
        {
            var options = new SearchRequestAdaptorOptions().AddWebHook(null, "http://example.com/post");

            Assert.AreEqual(1, options.WebHooks.Count);
            Assert.AreEqual(null, options.WebHooks.FirstOrDefault().Name);
            Assert.AreEqual("http://example.com/post", options.WebHooks.FirstOrDefault().Uri);

        }
    }
}
