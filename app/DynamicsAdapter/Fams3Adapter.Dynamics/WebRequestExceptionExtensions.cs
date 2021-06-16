using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fams3Adapter.Dynamics
{
    public static class WebRequestExceptionExtensions 
    {
        public static string DUPLICATED_ERROR_CODE = "0x80060892";

        public static bool IsDuplicateHashError(this WebRequestException exception)
        {
            if(exception != null && exception.Code==System.Net.HttpStatusCode.PreconditionFailed)
            {
                if (exception.Response != null)
                {
                    var rootObj = JsonConvert.DeserializeObject<RootObject>(exception.Response);
                    if (rootObj.Error.Code == DUPLICATED_ERROR_CODE)
                        return true;
                    return false;
                }
                return false;
            }
            return false;
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
