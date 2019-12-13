using Microsoft.Extensions.DependencyInjection;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

namespace SearchApi.Web.Notifications
{
    public static class ServiceCollectionExtensions
    {
        public static void AddWebHooks(this IServiceCollection services)
        {
            
            services.AddHttpClient<ISearchApiNotifier<PersonSearchAdapterEvent>, WebHookNotifierSearchEventStatus>();
        }
    }
}