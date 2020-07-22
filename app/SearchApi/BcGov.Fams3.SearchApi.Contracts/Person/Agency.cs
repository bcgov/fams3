using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BcGov.Fams3.SearchApi.Contracts.Person
{
    public class Agency

    {

        [Description("notes")]
        public string Notes { get; set; }

        [Description("request priority")]
        public RequestPriority RequestPriority { get; set; }

        [Description("agency code")]
        public string Code { get; set; }

        [Description("agency request Id")]
        public string RequestId { get; set; }


        [Description("agency search requeston reason code")]
        public string ReasonCode { get; set; }

        [Description("request date sent by agency")]
        public DateTime RequestDate { get; set; }


        [Description("Information requested by agency")]
        public List<string> InformationRequested { get; set; }

        [Description("first name and last name of agent")]
        public Name Agent { get; set; }

        [Description("Phone and fax of agent")]
        public IEnumerable<Phone> AgentContact { get; set; }

        [Description("agent email")]
        public string Email { get; set; }

       




    }

    public enum RequestAction
    {
        NEW,
        UPDATE,
        CANCEL,
        NOTIFY
    }
    public enum RequestPriority
    {
        Urgent,
        Rush,
        Normal,
    }
}
