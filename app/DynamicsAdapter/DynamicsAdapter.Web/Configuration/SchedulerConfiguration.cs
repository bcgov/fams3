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
            ResultUpload = "0/10 * * * * ?"; 
        }

        /// <summary>
        /// A quartz.net cron expression
        /// https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html
        /// </summary>
        public string Cron { get; set; }

        public string Failed { get; set; }

        public string ResultUpload { get; set; }
    }
}