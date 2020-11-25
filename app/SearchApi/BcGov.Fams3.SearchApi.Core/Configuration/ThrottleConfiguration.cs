using System;
using System.Collections.Generic;
using System.Text;

namespace BcGov.Fams3.SearchApi.Core.Configuration
{
   public  class ThrottleConfiguration
    {
        public ThrottleConfiguration()
        {
            IntervalInMinutes = 15;
            MessagePerTime = 20;

        }

        public int IntervalInMinutes { get; set; }
        public int MessagePerTime { get; set; }
    }
}
