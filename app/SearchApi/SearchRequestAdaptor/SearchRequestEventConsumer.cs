using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SearchRequestAdaptor
{
    public abstract class SearchRequestEventConsumer
    {

        private readonly ILogger<SearchRequestEventConsumer> _logger;


        //private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public SearchRequestEventConsumer(/*ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier,*/ ILogger<SearchRequestEventConsumer> logger)
        {
            //_searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<SearchRequestEvent> context, string eventName)
        {
            //using (LogContext.PushProperty("SearchRequestKey", context.Message?.SearchRequestKey))
            //{
            //    var cts = new CancellationTokenSource();
            //    _logger.LogInformation($"received new {nameof(PersonSearchAdapterEvent)} event from {context.Message.ProviderProfile?.Name}");
            //    await _searchApiNotifier.NotifyEventAsync(context.Message.SearchRequestKey, context.Message, eventName,
            //        cts.Token);
            //}
        }
    }



    public class SearchRequestOrderedConsumer : SearchRequestEventConsumer, IConsumer<SearchRequestOrdered>
    {
        private readonly ILogger<SearchRequestOrderedConsumer> _logger;
        //private readonly ISearchApiNotifier<PersonSearchAdapterEvent> _searchApiNotifier;

        public SearchRequestOrderedConsumer(/*ISearchApiNotifier<PersonSearchAdapterEvent> searchApiNotifier,*/ ILogger<SearchRequestOrderedConsumer> logger) : base(/*searchApiNotifier,*/ logger)
        {
            //_searchApiNotifier = searchApiNotifier;
            _logger = logger;

        }

        public async Task Consume(ConsumeContext<SearchRequestOrdered> context)
        {
            await base.Consume(context, "Ordered");

        }

    }

}
