using BcGov.Fams3.Utils.String;
using NUnit.Framework;
using System;

namespace BcGov.Fams3.Utils.Test
{
    public class StringExtensionsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void lower_ToTitleCase_successfuly()
        {
            string test = "test";
            string result = test.ToTitleCase();
            Assert.AreEqual("Test", result);
        }

        [Test]
        public void oneLetter_ToTitleCase_successfuly()
        {
            string test = "t";
            string result = test.ToTitleCase();
            Assert.AreEqual("T", result);
        }

        [Test]
        public void upper_ToTitleCase_successfuly()
        {
            string test = "TEST";
            string result = test.ToTitleCase();
            Assert.AreEqual("Test", result);
        }

        [Test]
        public void empty_ToTitleCase_successfuly()
        {
            string test = "";
            string result = test.ToTitleCase();
            Assert.AreEqual(null, result);
        }


        [Test]
        public void string_ToSHA1String_successfuly()
        {
            string test = "";
            string result = test.ToSHA1String();
            Assert.AreEqual("2jmj7l5rSw0yVb/vlWAYkK/YBwk=", result);
        }

        [Test]
        public void normalstring_ToSHA1String_successfuly()
        {
            string test = "lalala";
            string result = test.ToSHA1String();
            Assert.AreEqual("3y76Bg4zX5dijKOcn+9Uaas8uDc=", result);
        }
    }
}
