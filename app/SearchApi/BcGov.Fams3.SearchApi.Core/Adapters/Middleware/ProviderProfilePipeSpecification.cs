using System.Collections.Generic;
using System.Linq;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;
using GreenPipes;
using MassTransit;

namespace BcGov.Fams3.SearchApi.Core.Adapters.Middleware
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