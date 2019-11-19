using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Fams3Adapter.Dynamics.OptionSets;
using Fams3Adapter.Dynamics.OptionSets.Models;
using Newtonsoft.Json;

namespace DynamicsAdapter.Web.Test.FakeMessages
{
    public static class FakeHttpMessageResponse
    {

        public static IEnumerable<GenericOption> GetFakeInvalidReason()
        {
            return new List<GenericOption>()
            {
                new GenericOption(1, "Ready For Search")
            };
        }


        public static IEnumerable<GenericOption> GetFakeValidReason()
        {
            return new List<GenericOption>()
                {
                new GenericOption(1, "Ready For Search"),
                new GenericOption(867670000, "In Progress"),
                new GenericOption(867670001, "Complete"),
                new GenericOption(2, "Other"),
            };
        }

        public static IEnumerable<GenericOption> GetFakeNullResult()
        {
            return new List<GenericOption>();
        }

        public static IEnumerable<GenericOption> GetFakeIdReason()
        {
            return new List<GenericOption>()
            {
                new GenericOption(1, "Ready For Search"),
                new GenericOption(867670000, "In Progress"),
                new GenericOption(867670001, "Complete"),
                new GenericOption(2, "Other"),
            };
        }


    }
}
