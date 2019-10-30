using DynamicsAdapter.Web.Services.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DynamicsAdapter.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.CompilerServices;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using EndPoint = DynamicsAdapter.Web.Configuration.EndPoint;

namespace DynamicsAdapter.Web.Test.Services.Dynamics
{
    public class DynamicsServiceTest
    {
        private Mock<IDynamicService> sut;
        private AppSettings settings;
        private string entity = "Search";
        [SetUp]
        public void Setup ()
        {
            settings = new AppSettings
            {
                DynamicsAPI = new DynamicsAPIConfig
                {
                    EndPoints = new List<EndPoint>
                    {
                        new EndPoint { Entity = "Search", URL = "http-search"}
                    }
                }
            };
            sut = new Mock<IDynamicService>();
            sut.Setup(x => x.GetToken()).Returns(Task.FromResult("123JAS-14AS123234ASD123123"));
            sut.Setup(x => x.Get(It.IsAny<string>())).Returns(Task.FromResult(new JObject()));
            sut.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<object>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
            sut.Setup(x => x.SaveBatch(It.IsAny<string>(), It.IsAny<MultipartContent>())).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        }
        [Test]
        public async Task it_should_get_entity()
        {
            var response =  await sut.Object.Get(entity);
            Assert.IsInstanceOf<JObject>(response);

        }

        [Test]
        public async Task it_should_save_entity()
        {
            var response = await sut.Object.Save(entity, new object());
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }

        [Test]
        public async Task it_should_save_batch_entity()
        {
            var response = await sut.Object.SaveBatch(entity, new MultipartContent());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }
    }
}
