using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicsAdapter.Web.SearchRequest.Models
{
    public class StatusReason
    {
        public OpionSet OptionSet { get; set; } 

    }
    public class OpionSet
    {
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public int Value { get; set; }
        public Label Label { get; set; }
    }
    public class Label
    {
        public UserLocalizedLabel UserLocalizedLabel { get; set; }
    }
    public class UserLocalizedLabel
    {
        public string Label { get; set; }
    }
}
