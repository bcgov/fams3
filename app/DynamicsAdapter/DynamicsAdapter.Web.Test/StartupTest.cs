
using AutoMapper;
using BcGov.Fams3.Redis;
using DynamicsAdapter.Web.ApiGateway;
using DynamicsAdapter.Web.Register;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Generation;
using NUnit.Framework;
using OpenTracing;
using Quartz;
using Quartz.Spi;

namespace DynamicsAdapter.Web.Test
{
    public class StartupTest
    {
        [Test]
        public void startup_should_registered_all_required_services()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
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
}
