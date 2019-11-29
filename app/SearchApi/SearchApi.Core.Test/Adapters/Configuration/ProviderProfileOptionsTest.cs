using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SearchApi.Core.Adapters.Configuration;

namespace SearchApi.Core.Test.Adapters.Configuration
{
    public class ProviderProfileOptionsTest
    {
        [Test]
        public void It_should_validate_options()
        {
            var services = new ServiceCollection();

            services.AddOptions<ProviderProfileOptions>().Configure(x => { }).ValidateDataAnnotations();

            var sp = services.BuildServiceProvider();
            
            
            var error = Assert.Throws<OptionsValidationException>(() =>
            {
                var options = sp.GetRequiredService<IOptionsMonitor<ProviderProfileOptions>>().CurrentValue;
            });

            Assert.AreEqual("DataAnnotation validation failed for members: 'Name' with the error: 'The Name field is required.'.", error.Message);

        }

    }
}