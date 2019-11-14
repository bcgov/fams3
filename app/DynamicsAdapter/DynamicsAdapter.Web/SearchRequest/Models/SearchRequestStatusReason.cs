using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest.Models
{
    [Flags]
    public enum SearchRequestStatusReason
    {
        ReadyForSearch = 1,
        InProgress = 867670000,
        Complete = 867670001
    }
    public static class SearchRequestStatusReasonExtensions
    {
        public static string GetName(this SearchRequestStatusReason reason)
        {
            return Enum.GetName(typeof(SearchRequestStatusReason), reason);
        }
    }
}
