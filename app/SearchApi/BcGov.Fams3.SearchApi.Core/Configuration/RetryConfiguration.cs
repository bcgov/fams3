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
            this.ConcyrrencyLimit = 5;
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
        public int ConcyrrencyLimit { get; set; }
    }
}
