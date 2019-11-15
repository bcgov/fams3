using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicsAdapter.Web.SearchRequest.Models;

namespace DynamicsAdapter.Web.Extensions
{
    public static class DynamicsAdapterExtension
    {
        public static SearchRequestStatusReason  GetStatusReasonItem(this int reasonValue)
        {
            return (SearchRequestStatusReason) Enum.Parse(typeof(SearchRequestStatusReason),
                Enum.GetName(typeof(SearchRequestStatusReason), reasonValue));
        }
    }
}
