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
                Cron = "* * * * * *"
            };

            Assert.AreEqual("* * * * * *", sut.Cron);

        }


    }
}