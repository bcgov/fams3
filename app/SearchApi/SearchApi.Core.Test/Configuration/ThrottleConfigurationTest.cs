using BcGov.Fams3.SearchApi.Core.Configuration;
using NUnit.Framework;

namespace SearchApi.Core.Test.Configuration
{
    public class ThrottleConfigurationTest
    {

        [Test]
        public void With_no_param_should_set_default_config()
        {
            var sut = new ThrottleConfiguration();

            Assert.AreEqual(20, sut.MessagePerTime);
            Assert.AreEqual(15, sut.IntervalInMinutes);

        }

        [Test]
        public void With_param_should_configure_throttling()
        {
            var sut = new ThrottleConfiguration()
            {
                IntervalInMinutes = 50,
                MessagePerTime = 20
            };

            Assert.AreEqual(20, sut.MessagePerTime);
            Assert.AreEqual(50, sut.IntervalInMinutes);

        }
    }
}
