using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using Microsoft.Extensions.DependencyInjection;

namespace SearchRequestAdaptor.Notifier
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWebHooks(this IServiceCollection services)
        {
            services.AddHttpClient<ISearchRequestNotifier<SearchRequestOrdered>, WebHookSearchRequestNotifier>();
        }
    }
}
