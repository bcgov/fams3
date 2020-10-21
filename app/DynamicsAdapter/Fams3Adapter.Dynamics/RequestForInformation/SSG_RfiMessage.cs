using System;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Newtonsoft.Json;

namespace Fams3Adapter.Dynamics.RfiService
{
	public class SSG_RfiMessage
    {
        
        [JsonProperty("ssg_rfimessageId")]
        public Guid Id{get;set;}

        [JsonProperty("ssg_recipient")]
        public string Recipient{get;set;}
		
        [JsonProperty("statecode")]
        public int StateCode { get; set; }
		
        [JsonProperty("statuscode")]
        public int StatusCode { get; set; }

        [JsonProperty("ssg_completionstatus")]
        public int CompletionStatus { get; set; }

        [JsonProperty("ssg_channel")]
        public int Channel {get;set;}

		[JsonProperty("ssg_notewithattachmentguid")]
		public Guid NoteId{get;set;}

        public string DocumentBody{get;set;}
	}

    public class RfiStateCodes : Enumeration{
        
        public static RfiStateCodes Active =
            new RfiStateCodes(1, "Active");

        public static RfiStateCodes Inactive =
            new RfiStateCodes(867670000, "Inactive");

        protected RfiStateCodes(int value, string name) : base(value, name)
        {
        }
    }

    public class RfiStatusCodes : Enumeration{
        
        public static RfiStatusCodes New =
            new RfiStatusCodes(1, "New");

        public static RfiStatusCodes InProgress =
            new RfiStatusCodes(867670000, "In Progress");

        public static RfiStatusCodes Complete = 
            new RfiStatusCodes(2, "Complete");

        protected RfiStatusCodes(int value, string name) : base(value, name)
        {
        }
    }

    
    public class RfiCompletionStatusCodes : Enumeration{
        
        public static RfiCompletionStatusCodes WithErrors =
            new RfiCompletionStatusCodes(867670000, "With Errors");

        public static RfiCompletionStatusCodes WithoutErrors =
            new RfiCompletionStatusCodes(867670001, "Without Errors");

        protected RfiCompletionStatusCodes(int value, string name) : base(value, name)
        {
        }
    }
    

    public class RfiChannels : Enumeration{
        
        public static RfiChannels Fax =
            new RfiChannels(867670000, "Fax");

        public static RfiChannels Email =
            new RfiChannels(867670001, "Email");
            
        public static RfiChannels Mail =
            new RfiChannels(867670002, "Mail");

        protected RfiChannels(int value, string name) : base(value, name)
        {
        }
    }
    
    public class Annotation{
        [JsonProperty("DocumentBody")]
        public string DocumentBody{get;set;}
    }
}