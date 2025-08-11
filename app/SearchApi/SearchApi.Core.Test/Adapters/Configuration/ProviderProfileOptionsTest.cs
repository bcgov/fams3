using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using BcGov.Fams3.SearchApi.Core.Adapters.Configuration;

namespace SearchApi.Core.Test.Adapters.Configuration
{
    public class ProviderProfileOptionsTest
    {
        [Test]
        public void It_should_validate_options()
        {
            var services = new ServiceCollection();

            services
                .AddOptions<ProviderProfileOptions>()
                .Configure(x => { }) // Deliberately leaving properties unset
                .ValidateDataAnnotations();

            var sp = services.BuildServiceProvider();

            var error = Assert.Throws<OptionsValidationException>(() =>
            {
                var options = sp.GetRequiredService<IOptionsMonitor<ProviderProfileOptions>>().CurrentValue;
            });

            // Match modern error message
            StringAssert.Contains("ProviderProfileOptions", error.Message);
            StringAssert.Contains("members: 'Name'", error.Message);
            StringAssert.Contains("The Name field is required.", error.Message);
        }
    }
}