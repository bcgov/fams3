using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchRequest.Adaptor.Notifier.Models
{
    public class Notification
    {
        public string FileId { get; set; }
        public string AgencyFileId { get; set; }
        public string FSOName { get; set; }
        public string Acvitity { get; set; }
        public DateTime ActivityDate { get; set; } 
    }
}
