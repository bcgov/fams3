using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BcGov.Fams3.SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Web.Notifications
{
  
    public interface ISearchApiNotifier<T>
    {

        Task NotifyEventAsync(Guid searchRequestId, T notificationStatus,string eventName, CancellationToken cancellationToken);

    }

}