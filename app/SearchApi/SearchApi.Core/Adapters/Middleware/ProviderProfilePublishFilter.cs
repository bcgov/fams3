using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Core.Adapters.Middleware
{
    /// <summary>
    /// The ProfilePublishFilter add the profile information to the context
    /// </summary>
    public class ProviderProfilePublishFilter : IFilter<PublishContext>
    {

        private readonly ProviderProfile _profile;

        public ProviderProfilePublishFilter(ProviderProfile profile)
        {
            this._profile = profile;
        }

        public async Task Send(PublishContext context, IPipe<PublishContext> next)
        {
            context.Headers.Set(nameof(ProviderProfile), _profile);
            await next.Send(context);
        }

        public void Probe(ProbeContext context)
        {

        }
    }
}