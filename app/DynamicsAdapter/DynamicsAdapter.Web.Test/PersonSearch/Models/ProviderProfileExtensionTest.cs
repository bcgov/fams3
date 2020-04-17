using DynamicsAdapter.Web.PersonSearch.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicsAdapter.Web.Test.PersonSearch.Models
{
    public class ProviderProfileExtensionTest
    {
        private ProviderProfile _sut;

     
        [Test]
        public void map_adaptor_code_name_from_dynamics_icbc()
        {
            _sut = new ProviderProfile { Name = "ICBC" };
            Assert.AreEqual(867670001, _sut.DynamicsID());
        }
        [Test]
        public void map_adaptor_code_name_from_dynamics_bchydro()
        {
            _sut = new ProviderProfile { Name = "BCHydro" };
            Assert.AreEqual(867670005, _sut.DynamicsID());
        }
        [Test]
        public void map_adaptor_code_name_from_dynamics_msdh()
        {
            _sut = new ProviderProfile { Name = "MHSD" };
            Assert.AreEqual(867670023, _sut.DynamicsID());
        }
    }
}
