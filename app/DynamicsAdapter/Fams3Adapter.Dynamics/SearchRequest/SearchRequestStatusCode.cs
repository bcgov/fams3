using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.SearchRequest
{
    public class SearchRequestStatusCode : Enumeration
    {
        public static SearchRequestStatusCode AgencyCancelled =
            new SearchRequestStatusCode(867670010, "Agency Cancelled");
        public static SearchRequestStatusCode SystemCancelled =
            new SearchRequestStatusCode(867670015, "System Cancelled");

        protected SearchRequestStatusCode(int value, string name) : base(value, name)
        {
        }

    }
}
