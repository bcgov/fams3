using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DynamicsAdapter.Web.Services.Dynamics
{
    public class ConfigureDynamicsModule

    {
        public static void ConfigureServices(IServiceCollection collection, AppSettings settings)
        {
            collection.AddTransient<IDynamicService>(x => new DynamicService( settings));
        }
    }
}
