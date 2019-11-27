using System.Linq;
using NUnit.Framework;
using SearchApi.Web.Configuration;

namespace SearchApi.Web.Test.Configuration
{
    public class SearchApiOptionsTest
    {
        [Test]
        public void Should_add_a_new_webHook()
        {

            var options = new SearchApiOptions().AddWebHook("test", "http://example.com/post","MatchFound");

            Assert.AreEqual(1, options.WebHooks.Count);
            Assert.AreEqual("test", options.WebHooks.FirstOrDefault().Name);
            Assert.AreEqual(true, options.WebHooks.FirstOrDefault().EventName.Contains("MatchFound"));
            Assert.AreEqual("http://example.com/post", options.WebHooks.FirstOrDefault().Uri);

        }


    }                                                                               
}