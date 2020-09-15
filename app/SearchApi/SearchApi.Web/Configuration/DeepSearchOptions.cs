using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApi.Web.Configuration
{
    public class DeepSearchOptions
    {

        public DeepSearchOptions()
        {
            MaxWaveCount = 15;
        }
        public int MaxWaveCount { get; set; }
    }
}
