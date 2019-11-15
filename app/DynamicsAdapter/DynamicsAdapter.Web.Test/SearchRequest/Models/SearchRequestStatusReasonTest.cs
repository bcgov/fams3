using System;
using System.Collections.Generic;
using System.Text;
using DynamicsAdapter.Web.Extensions;
using DynamicsAdapter.Web.SearchRequest.Models;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.SearchRequest.Models
{
    public class SearchRequestStatusReasonTest
    {
        [Test]
        public void should_return_the_enum_name()
        {

            var sut = SearchRequestStatusReason.Complete.GetName();

            Assert.AreEqual("Complete", sut);


        }

     

        [Test]
        public void should_get_enum_object_for_value()
        {
            var enum_value = 867670000;

            var sut = enum_value.GetStatusReasonItem();

            Assert.AreEqual(SearchRequestStatusReason.InProgress, sut);


        }
    }
}
