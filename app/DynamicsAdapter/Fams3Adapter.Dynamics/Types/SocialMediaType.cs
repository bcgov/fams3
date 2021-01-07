using Fams3Adapter.Dynamics.OptionSets.Models;

namespace Fams3Adapter.Dynamics.Types
{
    public class SocialMediaType : Enumeration
    {
        public static SocialMediaType Facebook = new SocialMediaType(867670000, "Facebook");
        public static SocialMediaType Twitter = new SocialMediaType(867670001, "Twitter");
        public static SocialMediaType LInkedIn = new SocialMediaType(867670002, "LInkedIn");
        public static SocialMediaType Other = new SocialMediaType(867670003, "Other");

        protected SocialMediaType(int value, string name) : base(value, name)
        {

        }
    }
}
