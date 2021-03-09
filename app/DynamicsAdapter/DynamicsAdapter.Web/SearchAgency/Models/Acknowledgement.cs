using BcGov.Fams3.SearchApi.Contracts.SearchRequest;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class Acknowledgement : SearchRequestEvent
    {
        public NotificationType NotificationType { get; set; }
        public NotificationStatusEnum Status { get; set; }

    }
}
