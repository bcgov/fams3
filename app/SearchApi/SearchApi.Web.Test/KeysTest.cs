using NUnit.Framework;

namespace SearchApi.Web.Test
{
    public class KeysTest
    {

        [Test]
        public void should_format_key_correctly()
        {
            Assert.AreEqual("deepsearch-1123123_909090-ICBC", string.Format(Keys.DEEP_SEARCH_REDIS_KEY_FORMAT,"1123123_909090","ICBC"));
        }
    }
}
