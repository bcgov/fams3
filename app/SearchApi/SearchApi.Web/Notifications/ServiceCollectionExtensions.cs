using Microsoft.Extensions.DependencyInjection;
using SearchApi.Core.Adapters.Contracts;
using SearchApi.Core.Adapters.Models;

namespace SearchApi.Web.Notifications
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWebHooks(this IServiceCollection services)
        {
            services.AddHttpClient<ISearchApiNotifier<MatchFound>, WebHookNotifierMatchFound>();
            services.AddHttpClient<ISearchApiNotifier<ProviderSearchEventStatus>, WebHookNotifierSearchStatus>();
        }
    }
}