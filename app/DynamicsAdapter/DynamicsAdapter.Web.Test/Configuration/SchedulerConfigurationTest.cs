using DynamicsAdapter.Web.Configuration;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Configuration
{
    public class SchedulerConfigurationTest
    {

        [Test]
        public void It_should_build_a_SchedulerConfiguration()
        {

            var sut = new SchedulerConfiguration()
            {
                Cron = "* * * * * *",
                AutoClose= "10/0 * * * * *"
            };

            Assert.AreEqual("* * * * * *", sut.Cron);
            Assert.AreEqual("10/0 * * * * *", sut.AutoClose);

        }


    }
}