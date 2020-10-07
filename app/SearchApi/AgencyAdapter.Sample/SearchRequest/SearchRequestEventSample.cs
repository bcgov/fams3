using System;
using System.Collections.Generic;
using BcGov.Fams3.SearchApi.Contracts.Person;
using BcGov.Fams3.SearchApi.Contracts.PersonSearch;
using BcGov.Fams3.SearchApi.Contracts.SearchRequest;
using BcGov.Fams3.SearchApi.Core.Adapters.Models;

namespace AgencyAdapter.Sample.SearchRequest
{
    public class SearchRequestNotificationSample : SearchRequestNotification
    {


        public Guid SearchRequestId { get; set; }
        public string SearchRequestKey { get; set; }
        public DateTime TimeStamp { get; set; }
        public ProviderProfile ProviderProfile { get; set; }
        public IEnumerable<PersonFound> MatchedPersons { get; set; }

        public NotificationType NotificationType { get; set; }

        public string RequestId { get; set; }
        public RequestAction Action { get; set; }
        public int? QueuePosition { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
        public string Message { get; set; }
        public Person Person { get; set; }
        public string FSOName { get; set; }
    }
    public class AgencySample : ProviderProfile
    {
        public string Name { get; set; }

        public SearchSpeedType SearchSpeedType { get; set; }
    }

    public static class FakeSearchrequestResponseBuilder
    {
        public static SearchRequestNotification BuildFakeSearchRequestNotification(Guid searchrequestId, string searchRequestKey, string requestId, NotificationType notification, string agency, RequestAction action)
        {

            return new SearchRequestNotificationSample()
            {
                ProviderProfile = new AgencySample { Name = agency },
                SearchRequestId = searchrequestId,
                SearchRequestKey = searchRequestKey,
                RequestId = requestId,
                TimeStamp = DateTime.Now,
                NotificationType = notification,
                Action = action,
                Message = $"{action} occured"

            };

        }

    }



}
