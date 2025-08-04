// using System.Security.Cryptography.X509Certificates;
using DynamicsAdapter.Web.Configuration;
using NUnit.Framework;

namespace DynamicsAdapter.Web.Test.Configuration
{
    public class SearchApiConfigurationTest
    {
        [Test]
        public void With_params_it_should_create_an_instance_of_SearchApiConfiguration()
        {
            var sut = new SearchApiConfiguration()
            {
                BaseUrl = "http://localhost:5000",
                AvailableDataPartner = "BCHYDRO:MSDPR:ICBC"
            };

            Assert.AreEqual("http://localhost:5000", sut.BaseUrl);
            Assert.AreEqual(sut.AvailableDataPartner, "BCHYDRO:MSDPR:ICBC");
            Assert.AreEqual(sut.AvailableDataPartner.Split(new char[] { ':' }).Length, 3);
        }

       

    }
}