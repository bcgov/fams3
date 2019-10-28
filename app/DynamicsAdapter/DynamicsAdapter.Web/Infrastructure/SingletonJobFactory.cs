using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace DynamicsAdapter.Web.Infrastructure
{
    public class SingletonJobFactory : IJobFactory
    {

        private readonly IServiceProvider _serviceProvider;

        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle triggerFiredBundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(triggerFiredBundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job) { }


    }
}