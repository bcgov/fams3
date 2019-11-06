using System.Collections.Generic;
using System.Linq;
using GreenPipes;
using GreenPipes.Filters;
using MassTransit;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Middleware
{
    public class ProviderProfilePipeSpecification : IPipeSpecification<PublishContext>
    {

        private readonly ProviderProfile _providerProfile;

        public ProviderProfilePipeSpecification(ProviderProfile providerProfile)
        {
            this._providerProfile = providerProfile;
        }

        public void Apply(IPipeBuilder<PublishContext> builder)
        {
            builder.AddFilter(new ProviderProfilePublishFilter(this._providerProfile));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}