using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using DynamicsAdapter.Web.Configuration;
using NUnit.Framework;


namespace DynamicsAdapter.Web.Test.Configuration
{
    public class AppSettingsTest
    {
        private AppSettings settings;

        [SetUp]
        public void Setup()
        {
             settings = new AppSettings
            {
                DynamicsAPI = new DynamicsAPIConfig
                {
                    EndPoints = new List<EndPoint>() {
                        new EndPoint() { Entity = "Search", URL = "http-search"},
                        new EndPoint() { Entity = "Address", URL = "http-address"},
                    }
                }
            };
        }

        [Test]
        public void endpoint_should_return_list()
        {
            
            Assert.IsInstanceOf<List<EndPoint>>(settings.DynamicsAPI.EndPoints);
        }

        [Test]
        public void url_should_be_found_for_entity()
        {
            Assert.AreEqual("http-search", settings.DynamicsAPI.EndPoints.Single((x => x.Entity == "Search")).URL);
        }
    }
}
