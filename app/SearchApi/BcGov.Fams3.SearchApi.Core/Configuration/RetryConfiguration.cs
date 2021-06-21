using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Core.Configuration
{
   public  class RetryConfiguration
    {
        public RetryConfiguration()
        {
            this.RetryTimes = 5;
            this.RetryInterval = 5;
            this.ConcurrencyLimit = 5;
            //PrefetchCount is set to 1 as default. But it does not make sense. It should be bigger than ConcurrencyLimit.
            this.PrefetchCount = 1; 
        }


        /// <summary>
        /// Retry Times
        /// </summary>
        public int RetryTimes { get; set; }

        /// <summary>
        /// Re Try Interval 
        /// </summary>
        public int RetryInterval { get; set; }

        /// <summary>
        /// Concurrency limit for consumer
        /// </summary>
        public int ConcurrencyLimit { get; set; }

        public ushort PrefetchCount { get; set; }
    }
}
