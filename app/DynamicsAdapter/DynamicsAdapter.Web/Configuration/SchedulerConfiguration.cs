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
            Failed = "0 0 15 * * ?";
            AutoClose = "0 0/10 * ? * * *"; //default, every 10 mins
        }

        /// <summary>
        /// A quartz.net cron expression
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
        /// </summary>
        public string Cron { get; set; }

        public string Failed { get; set; }

        public string AutoClose { get; set; }

    }
}