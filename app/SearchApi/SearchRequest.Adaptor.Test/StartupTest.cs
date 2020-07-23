using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NUnit.Framework;
using SearchRequestAdaptor;
using SearchRequestAdaptor.Publisher;

namespace SearchRequest.Adaptor.Test
{
    public class StartupTest
    {
        [Test]
        public void startup_should_registered_all_required_services()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.IsNotNull(webHost);
            Assert.IsNotNull(webHost.Services.GetService(typeof(ISearchRequestEventPublisher)));
            Assert.IsNotNull(webHost.Services.GetService(typeof(HealthCheckService)));
        }
    }
}