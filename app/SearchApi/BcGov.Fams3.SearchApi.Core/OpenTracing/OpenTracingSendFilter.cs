using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using OpenTracing.Propagation;
using OpenTracing.Util;

namespace BcGov.Fams3.SearchApi.Core.OpenTracing
{
    public class OpenTracingSendFilter : IFilter<SendContext>
    {

        public void Probe(ProbeContext context)
        {

        }

        public async Task Send(SendContext context, IPipe<SendContext> next)
        {
            try
            {
                var operationName = $"Sending Message: {context.DestinationAddress.GetExchangeName()}";

                var spanBuilder = GlobalTracer.Instance.BuildSpan(operationName)
                    .AsChildOf(GlobalTracer.Instance.ActiveSpan.Context)
                    .WithTag("destination-address", context.DestinationAddress?.ToString())
                    .WithTag("source-address", context.SourceAddress?.ToString())
                    .WithTag("initiator-id", context.InitiatorId?.ToString())
                    .WithTag("message-id", context.MessageId?.ToString());

                using (var scope = spanBuilder.StartActive())
                {
                    GlobalTracer.Instance.Inject(
                        GlobalTracer.Instance.ActiveSpan.Context,
                        BuiltinFormats.TextMap,
                        new MassTransitSendContextTextMapInjectAdapter(context));

                    await next.Send(context);
                }
            }
            catch (Exception)
            {
                await next.Send(context);
            }
        }

        
    }
}
