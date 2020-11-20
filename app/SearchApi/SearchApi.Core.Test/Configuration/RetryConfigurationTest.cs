using BcGov.Fams3.SearchApi.Core.Configuration;
using NUnit.Framework;

namespace SearchApi.Core.Test.Configuration
{
    public class RetryConfigurationTest
    {
        [Test]
        public void With_no_param_should_set_default_rabbit_config()
        {
            var sut = new RetryConfiguration();

            Assert.AreEqual(5, sut.RetryInterval);
            Assert.AreEqual(5, sut.RetryTimes);
            Assert.AreEqual(5, sut.ConcurrencyLimit);

        }

        [Test]
        public void With_param_should_configure_retry()
        {
            var sut = new RetryConfiguration()
            {
                RetryTimes = 3,
                RetryInterval = 20,
                ConcurrencyLimit = 9
            };

            Assert.AreEqual(20, sut.RetryInterval);
            Assert.AreEqual(3, sut.RetryTimes);
            Assert.AreEqual(9, sut.ConcurrencyLimit);

        }
    }
}
