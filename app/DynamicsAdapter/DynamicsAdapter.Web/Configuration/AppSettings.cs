using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;

namespace DynamicsAdapter.Web.Configuration
{
    public class AppSettings
    {
        public SchedulerConfig Scheduler { get; set; }
        public DynamicsAPIConfig DynamicsAPI { get; set; }
    }

    public class DynamicsAPIConfig
    {
        public string Timeout { get; set; }
        public string OAuthUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TokenTimeout { get; set; }
    }

    public class SchedulerConfig
    {
        public string Cron { get; set; }
    }
}
