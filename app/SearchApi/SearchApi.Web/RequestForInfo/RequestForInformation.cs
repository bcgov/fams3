using System;
using System.Collections.Generic;
using System.ComponentModel;
using BcGov.Fams3.SearchApi.Contracts.Rfi;
using Newtonsoft.Json;

namespace SearchApi.Web.Controllers
{
	[Description("Represents a set of information to execute a request for information")]
	public class RequestForInformation : BcGov.Fams3.SearchApi.Contracts.Rfi.RequestForInformation
	{
        public RequestForInformation(Guid id){
            Id = id;
        }
		public Guid Id  {get;set;}

		public string Recipient  {get;set;}

		public string DocumentBody  {get;set;}
	}
}