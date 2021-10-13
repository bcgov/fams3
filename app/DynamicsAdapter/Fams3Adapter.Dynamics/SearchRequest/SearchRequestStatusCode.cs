using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SearchRequestStatusCode : Enumeration
    {
        public static SearchRequestStatusCode AgencyCancelled =
            new SearchRequestStatusCode(867670010, "Agency Cancelled");
        public static SearchRequestStatusCode SystemCancelled =
            new SearchRequestStatusCode(867670015, "System Cancelled");
        public static SearchRequestStatusCode SearchRequestCancelled =
            new SearchRequestStatusCode(867670009, "Search Request Cancelled");
        public static SearchRequestStatusCode SearchRequestClosed =
                    new SearchRequestStatusCode(2, "Search Request Closed");
        public static SearchRequestStatusCode SearchRequestAutoClosed =
            new SearchRequestStatusCode(867670016, "AutoClosed");

        protected SearchRequestStatusCode(int value, string name) : base(value, name)
        {
        }

    }
}
