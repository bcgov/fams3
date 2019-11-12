using System;
using System.Threading;
using System.Threading.Tasks;
using SearchApi.Core.Adapters.Contracts;

namespace SearchApi.Web.Notifications
{
    public interface ISearchApiNotifier
    {

        Task NotifyMatchFoundAsync(Guid searchRequestId, MatchFound matchFound, CancellationToken cancellationToken);


    }
}