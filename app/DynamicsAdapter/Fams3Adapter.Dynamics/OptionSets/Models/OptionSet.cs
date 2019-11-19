using System.Collections.Generic;

namespace Fams3Adapter.Dynamics.OptionSets.Models
{
    public class OptionSet
    {
        public List<Option> Options { get; set; }
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