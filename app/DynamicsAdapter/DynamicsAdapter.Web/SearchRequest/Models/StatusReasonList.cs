using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest.Models
{
    public enum StatusReasonList
    {
        Received = 1,
        ReadyForSearch = 867670000,
        PreScreening = 867670001,
        AutoSearch = 867670002,
        ReadyForAnalysis = 867670003,
        InAnalysis = 867670004,
        Pending = 867670005,
        ManagerReview = 867670006,
        ReadyForReview = 867670007,
        ReviewClosure = 867670008,
        Closed = 2,
        Cancelled = 867670009
    }
}
