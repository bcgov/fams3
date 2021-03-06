﻿using BcGov.Fams3.SearchApi.Contracts.Person;
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

        public string Agency { get; set; }
        public int? PositionInQueue { get;  set; }
        public DateTime? EstimatedCompletionDate { get;  set; }

        public Person Person { get; set; }

    }
}
