using System;
using System.Threading;
using System.Threading.Tasks;

namespace SearchApi.Web.Notifications
{
  
    public interface ISearchApiNotifier<T>
    {

        Task NotifyEventAsync(Guid searchRequestId, T notificationStatus,string eventName, CancellationToken cancellationToken);

    }

}