using DynamicsAdapter.Web.Infrastructure;
using DynamicsAdapter.Web.SearchRequest;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Infrastructure
{
    public class JobScheduleTest
    {

        [Test]
        public void It_should_create_a_job_schedule()
        {

            var sut = new JobSchedule(typeof(SearchRequestJob), "* * * * * *");

            Assert.AreEqual(typeof(SearchRequestJob), sut.JobType);
            Assert.AreEqual("* * * * * *", sut.CronExpression);

        }

        [Test]
        public void It_should_create_a_failed_job_schedule()
        {

            var sut = new JobSchedule(typeof(FailedSearchRequestJob), "* 8 * * * ");

            Assert.AreEqual(typeof(FailedSearchRequestJob), sut.JobType);
            Assert.AreEqual("* 8 * * * ", sut.CronExpression);

        }

    }
}