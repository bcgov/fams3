using Quartz;

namespace DynamicsAdapter.Web.Configuration
{
    public class SchedulerConfiguration
    {

        public SchedulerConfiguration()
        {
            CronExpression = "0/5 * * * * ?";
        }

        public string CronExpression { get; set; }


    }
}