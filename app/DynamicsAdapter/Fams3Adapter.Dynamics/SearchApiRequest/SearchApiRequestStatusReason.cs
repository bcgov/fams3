using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.SearchApiRequest
{
    public class SearchApiRequestStatusReason : Enumeration
    {
        public static SearchApiRequestStatusReason ReadyForSearch =
            new SearchApiRequestStatusReason(1, "Ready For Search");

        public static SearchApiRequestStatusReason InProgress =
            new SearchApiRequestStatusReason(867670000, "In Progress");

        public static SearchApiRequestStatusReason Complete = 
            new SearchApiRequestStatusReason(867670001, "Complete");

        protected SearchApiRequestStatusReason(int value, string name) : base(value, name)
        {
        }

    }
}