using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Web.Notifications
{
    public enum EventType
    {
        PersonSearchAccepted,
        PersonSearchRejected,
        PersonSearchMatchNotFound,
        PersonSearchFailed,
        PersonSearchMatchFound  
    }
    public interface ISearchApiNotifier<T>
    {

        Task NotifyEventAsync(Guid searchRequestId, T notificationStatus, CancellationToken cancellationToken);

    }

}