using Quartz;

namespace DynamicsAdapter.Web.Configuration
{
    /// <summary>
    /// Represents the scheduler configuration
    /// Settings can be overriden with environment variables
    /// SCHEDULER__CRON=0/5 * * * * ?
    /// </summary>
    public class SchedulerConfiguration
    {

        public SchedulerConfiguration()
        {
            Cron = "0/5 * * * * ?";
        }

        /// <summary>
        /// A quartz.net cron expression
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
        /// </summary>
        public string Cron { get; set; }


    }
}