using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using DynamicsAdapter.Web.SearchRequest.Models;
using Newtonsoft.Json;

namespace DynamicsAdapter.Web.Test.FakeMessages
{
    public static class FakeHttpMessageResponse
    {
        public static HttpResponseMessage GetList()
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = GetFakeStatusReasonContent()
            };

        }

        public static StatusReason GetFakeReason()
        {
            return new StatusReason()
            {
                OptionSet = new OpionSet()
                {
                    Options = new List<Option>()
                    {
                        new Option() {
                            Value  = 1, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Ready For Search" }
                            }
                        }
                    }
                }

            };
        }
        public static HttpContent GetFakeStatusReasonContent()
        {
            var reason = GetFakeReason();
            var json = JsonConvert.SerializeObject(reason);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

    }
}
