using System;

namespace DynamicsAdapter.Web.Infrastructure
{
    /// <summary>
    /// Represents a Job and its schedule.
    /// </summary>
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        /// <summary>
        /// The dotnet type of the job
        /// </summary>
        public Type JobType { get; }

        /// <summary>
        /// A Quartz.net cron Expression
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
        /// </summary>
        public string CronExpression { get; }
    }
}