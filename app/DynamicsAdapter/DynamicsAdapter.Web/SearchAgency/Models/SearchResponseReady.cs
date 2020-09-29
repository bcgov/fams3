using System;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchResponseReady
    {
        public string FileId { get; set; }
        public string Agency { get; set; }
        public string AgencyFileId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Activity { get; set; }
        public string ResponseGuid { get; set; }
        public string FSOName { get; set; }
        public Person Person { get; set; }
    }
}
