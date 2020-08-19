using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Infrastructure;
using DynamicsAdapter.Web.SearchRequest;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Spi;

namespace DynamicsAdapter.Web.Test.Infrastructure
{
    public class QuartzHostedServiceTest
    {

        private readonly Mock<IScheduler> _schedulerMock = new Mock<IScheduler>();
        private readonly Mock<ISchedulerFactory> _schedulerFactoryMock = new Mock<ISchedulerFactory>();
        private readonly Mock<IJobFactory> _jobFactoryMock = new Mock<IJobFactory>();
        private readonly List<JobSchedule> _jobSchedulesMock = new List<JobSchedule>();

        private QuartzHostedService sut;

        [SetUp]
        public void Setup()
        {
            _jobSchedulesMock.Add(new JobSchedule(typeof(SearchRequestJob), "0/5 * * * * ?"));
           // _jobSchedulesMock.Add(new JobSchedule(typeof(FailedSearchRequestJob), "0/5 * * * * ?"));
            _schedulerFactoryMock.Setup(x => x.GetScheduler(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_schedulerMock.Object));
            sut = new QuartzHostedService(_schedulerFactoryMock.Object, _jobFactoryMock.Object, _jobSchedulesMock);
        }

        [Test]
        public async Task It_should_start_the_scheduler()
        {

            await sut.StartAsync(new CancellationToken());

            _schedulerMock.Verify(x => x.ScheduleJob(It.IsAny<IJobDetail>(), It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _schedulerMock.Verify(x => x.Start(It.IsAny<CancellationToken>()), Times.AtLeastOnce());

        }

        [Test]
        public async Task It_should_shut_down_jobs()
        {
            await sut.StartAsync(new CancellationToken());
            await sut.StopAsync(new CancellationToken());
            _schedulerMock.Verify(x => x.Shutdown(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task It_should_not_shut_down_jobs_when_Scheduler_null()
        {
            await sut.StopAsync(new CancellationToken());
            _schedulerMock.Verify(x => x.Shutdown(It.IsAny<CancellationToken>()), Times.Never());
        }

    }
}