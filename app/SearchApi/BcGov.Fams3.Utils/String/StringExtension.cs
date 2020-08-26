using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BcGov.Fams3.Utils.String
{
    public static class StringExtension
    {
        public static bool IsValidXML(this string value)
        {
            try
            {
                XDocument.Parse(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
