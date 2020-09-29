using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchAgency.Models
{
    public class SearchRequestNotification
    {
        public string FileId { get; set; }
        public string AgencyFileId { get; set; }
        public string FSOName { get; set; }
        public string Acvitity { get; set; }
        public DateTime ActivityDate { get; set; }
        public int? PositionInQueue { get; set; }

        public DateTime? EstimatedCompletionDate { get; set; }
        public Person Person { get; set; }
        public string Agency { get; set; }
    }

}
