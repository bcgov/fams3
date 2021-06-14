using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics
{
    public class WebRequestExceptionTranslator 
    {
        public static string DUPLICATED_ERROR_CODE = "0x80060892";
        public static string GetErrorCode(WebRequestException webRequestException)
        {
            if (webRequestException?.Response != null)
            {
                var rootObj = JsonConvert.DeserializeObject<RootObject>(webRequestException.Response);
                return rootObj.Error.Code;
            }
            return null;
        }

        class Innererror
        {
            public string Message { get; set; }
            public string Type { get; set; }
            public string Stacktrace { get; set; }
            public Innererror Internalexception { get; set; }
        }

        class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public Innererror Innererror { get; set; }
        }

        class RootObject
        {
            public Error Error { get; set; }
        }
    }

}
