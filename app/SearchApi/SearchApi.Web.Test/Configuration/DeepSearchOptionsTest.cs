using NUnit.Framework;
using SearchApi.Web.Configuration;

namespace SearchApi.Web.Test.Configuration
{
    public class DeepSearchOptionsTest
    {
        [Test]
        public void should_set_maximum_in_code_if_not_set()
        {

            var options = new DeepSearchOptions();

            Assert.AreEqual(5, options.MaxWaveCount);

        }

        [Test]
        public void should_set_maximum_in_to_passed_value()
        {

            var options = new DeepSearchOptions { MaxWaveCount = 53};

            Assert.AreEqual(53, options.MaxWaveCount);

        }
    }
}
