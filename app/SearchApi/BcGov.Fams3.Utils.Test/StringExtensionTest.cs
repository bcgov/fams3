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
    }
}
