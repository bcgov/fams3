using Microsoft.Extensions.DependencyInjection;

namespace SearchApi.Web.Notifications
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWebHooks(this IServiceCollection services)
        {
            services.AddHttpClient<ISearchApiNotifier, WebHookNotifier>();
        }
    }
}