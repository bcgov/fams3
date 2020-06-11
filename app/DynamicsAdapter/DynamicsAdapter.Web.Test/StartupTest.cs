
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using DynamicsAdapter.Web;
using Microsoft.Extensions.DependencyInjection;
using DynamicsAdapter.Web.PersonSearch;
using OpenTracing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Generation;
using DynamicsAdapter.Web.Auth;
using DynamicsAdapter.Web.ApiGateway;
using Fams3Adapter.Dynamics.OptionSets;
using Simple.OData.Client;
using Fams3Adapter.Dynamics.SearchApiRequest;
using Fams3Adapter.Dynamics.SearchRequest;
using Microsoft.Extensions.Caching.Distributed;
using Quartz.Spi;
using Quartz;
using DynamicsAdapter.Web.SearchRequest;
using DynamicsAdapter.Web.Infrastructure;
using AutoMapper;
using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.Register;

namespace DynamicsAdapter.Web.Test
{
    public class StartupTest
    {
        [Test]
        public void startup_should_registered_all_required_services()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<MyStartup>().Build();
            Assert.IsNotNull(webHost);
            Assert.IsNotNull(webHost.Services.GetService<HealthCheckService>());
            Assert.IsNotNull(webHost.Services.GetService<ITracer>());
            Assert.IsNotNull(webHost.Services.GetService<IOpenApiDocumentGenerator>());
            Assert.IsNotNull(webHost.Services.GetService<ISearchApiClient>());
            Assert.IsNotNull(webHost.Services.GetService<ApiGatewayHandler>());           
            Assert.IsNotNull(webHost.Services.GetService<IDistributedCache>());
            Assert.IsNotNull(webHost.Services.GetService<IJobFactory>());
            Assert.IsNotNull(webHost.Services.GetService<ISchedulerFactory>());
            Assert.IsNotNull(webHost.Services.GetService<IMapper>());
            Assert.IsNotNull(webHost.Services.GetService<ICacheService>());
            Assert.IsNotNull(webHost.Services.GetService<ISearchRequestRegister>());
        }
    }

    public class MyStartup : Startup
    {
        public MyStartup(IConfiguration config) : base(config) { }
    }
}
