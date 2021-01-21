using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            // Return char and concat substring.  
            return char.ToUpper(value[0]) + value.Substring(1)?.ToLower();
        }

        public static string ToSHA1String(this string value)
        {
            SHA1CryptoServiceProvider sha1Hasher = new SHA1CryptoServiceProvider();
            byte[] hashedDataBytes = sha1Hasher.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }
}
