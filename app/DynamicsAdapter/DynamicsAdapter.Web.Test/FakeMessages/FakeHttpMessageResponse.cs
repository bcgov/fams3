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

        public static StatusReason GetFakeInvalidReason()
        {
            return new StatusReason()
            {
                OptionSet = new OpionSet()
                {
                    Options = new List<Option>()
                    {
                        new Option() {
                            Value  = 1, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Ready For Search" }}
                        }
                    }
                }

            };
        }


        public static StatusReason GetFakeValidReason()
        {
            return new StatusReason()
            {
                OptionSet = new OpionSet()
                {
                    Options = new List<Option>()
                    {
                        new Option() { Value  = 1, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Ready For Search" }}},
                        new Option() { Value  = 867670000, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="In Progress" }}},
                        new Option() { Value  = 867670001, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Complete" }}},
                        new Option() { Value  = 2, Label = new Label{ UserLocalizedLabel = new UserLocalizedLabel{ Label ="Other" }}}
                    }
                }

            };
        }

        public static StatusReason GetFakeNullResult()
        {
            return new StatusReason();
        }



        public static HttpContent GetFakeStatusReasonContent()
        {
            var reason = GetFakeInvalidReason();
            var json = JsonConvert.SerializeObject(reason);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

    }
}
