using BcGov.Fams3.SearchApi.Contracts.Person;
using NUnit.Framework;
using SearchApi.Web.DeepSearch.Schema;
using System.Collections.Generic;

namespace SearchApi.Web.Test.DeepSearch.Schema
{
    public class WaveMetaDataTest
    {
     

        [Test]
        public void should_have_current_wave_maximum_2()
        {

            var wave = new WaveMetaData
            {
                 CurrentWave = 2
            };

            Assert.AreEqual(2, wave.CurrentWave);

        }

        [Test]
        public void should_have_all_parameter()
        {

            var wave = new WaveMetaData
            {
                CurrentWave = 2,
                AllParameter = new List<Person>() { new Person() }
            };

            Assert.IsInstanceOf<List<Person>>(wave.AllParameter);

        }
    }
}
