using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchRequestEvent
    {
        /// <summary>
        /// This is Data Partner Id for the SearchRequest
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// This is the Key to identify the searchRequest in dynamics it is made of FileId_SequenceNumber
        /// </summary>
        public string SearchRequestKey { get; set; }
        /// <summary>
        /// This is Guid of the search request
        /// </summary>
        public Guid SearchRequestId { get; set; }
        public DateTime TimeStamp { get; set; }
        ////
        /// <summary>
        ///   action requested by agency, new, update, cancel, notify"
        /// </summary>
        public RequestAction Action { get; set; }
    }

    public enum RequestAction
    {
        NEW,
        UPDATE,
        CANCEL,
        NOTIFY
    }
}
