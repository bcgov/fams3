


using System;

namespace BcGov.Fams3.SearchApi.Contracts.SearchRequest
{
    public  interface SearchRequestSaved : SearchRequestEvent
    {
        string Message { get; }


        public int? QueuePosition { get; set; }

        public DateTime? EstimatedCompletion { get; set; }
    }
}
