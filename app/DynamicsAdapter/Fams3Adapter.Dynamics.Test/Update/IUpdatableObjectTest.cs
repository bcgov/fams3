using BcGov.Fams3.Utils.Object;
using NUnit.Framework;
using System;
using Fams3Adapter.Dynamics.Update;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fams3Adapter.Dynamics.Test.Update
{
    public class IUpdatableObjectTest
    {
        private Guid _guid;

        [SetUp]
        public void Setup()
        {
            _guid = Guid.NewGuid();
        }

        [Test]
        public void same_data_updated_should_be_false()
        {
            Guid guid = Guid.NewGuid();
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls()
            {
                PropInt = 2,
                PropString = "test1",
                PropNullableDateTime = null,
                PropGuid = guid,
                PropBool = true,
            };
            TestUpdatableCls testUpdatableCls2 = testUpdatableCls1.Clone();
            TestUpdatableCls result = testUpdatableCls1.MergeUpdates(testUpdatableCls2);
            Assert.AreEqual(false, result.Updated);
        }

        [Test]
        public void null_data_no_updated_should_be_false()
        {
            Guid guid = Guid.NewGuid();
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls()
            {
                PropNullableDateTime = new DateTime(2000, 1, 1),
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls()
            {
                PropNullableDateTime = null,
            };
            TestUpdatableCls result = testUpdatableCls1.MergeUpdates(testUpdatableCls2);
            Assert.AreEqual(false, result.Updated);
        }

        [Test]
        public void different_data_updated_should_be_true()
        {
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls()
            {
                PropInt = 2,
                PropString = "test1",
                PropNullableDateTime = new DateTime(2000, 1, 1),
                PropBool = true,
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls()
            {
                PropInt = 2,
                PropString = "test1",
                PropNullableDateTime = null,
                PropBool = false,
            };
            TestUpdatableCls result = testUpdatableCls1.MergeUpdates(testUpdatableCls2);
            Assert.AreEqual(true, result.Updated);
            Assert.AreEqual(new DateTime(2000, 1, 1), result.PropNullableDateTime);
            Assert.AreEqual(false, result.PropBool);
        }

        [Test]
        public void different_data_GetUpdateEntries_should_get_correct_updateFields()
        {
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls
            {
                PropInt = 1,
                PropString = "test1",
                PropNullableDateTime = new DateTime(2000, 1, 1),
                PropBool = true,
                Phone="1234",
                Ignored="ignored",
                PropGuid=_guid,
                Updated=true               
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls
            {
                PropInt = 2,
                PropString = "test2",
                PropNullableDateTime = new DateTime(2001, 1, 1),
                PropBool = false,
                Phone = "12345",
                Ignored = "notshow",
                PropGuid = _guid,
                Updated = true
            };
            var updatedFields = testUpdatableCls1.GetUpdateEntries(testUpdatableCls2);
            Assert.AreEqual(5, updatedFields.Count);
            Assert.AreEqual(true, updatedFields.Contains(new KeyValuePair<string, object>("ssg_propInt", 2 )));
            Assert.AreEqual(true, updatedFields.Contains(new KeyValuePair<string, object>("ssg_propstring", "test2")));
            Assert.AreEqual(true, updatedFields.Contains(new KeyValuePair<string, object>("ssg_propnullabledatetime", new DateTime(2001, 1, 1))));
            Assert.AreEqual(true, updatedFields.Contains(new KeyValuePair<string, object>("ssg_propbool", false)));
            Assert.AreEqual(true, updatedFields.Contains(new KeyValuePair<string, object>("ssg_phonenumber", "12345")));
        }

        [Test]
        public void updateignored_data_GetUpdateEntries_should_be_ignored_in_updateFields()
        {
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls
            {
                Ignored = "ignored",
                PropGuid = _guid,
                Updated = true
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls
            {
                Ignored = "notshow",
                PropGuid = Guid.NewGuid(),
                Updated = false
            };
            var updatedFields = testUpdatableCls1.GetUpdateEntries(testUpdatableCls2);
            Assert.AreEqual(0, updatedFields.Count);
        }

        [Test]
        public void compareOnlyNumber_data_same_number_GetUpdateEntries_should_not_in_updateFields()
        {
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls
            {
                Phone = "123==4-5p678--9"
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls
            {
                Phone = "--123(45)678#9"
            };
            var updatedFields = testUpdatableCls1.GetUpdateEntries(testUpdatableCls2);
            Assert.AreEqual(0, updatedFields.Count);
        }

        [Test]
        public void null_json_property_GetUpdateEntries_should_not_in_updateFields()
        {
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls
            {
                NoJsonProperty = "json"
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls
            {
                NoJsonProperty = "differentJson"
            };
            var updatedFields = testUpdatableCls1.GetUpdateEntries(testUpdatableCls2);
            Assert.AreEqual(0, updatedFields.Count);
        }
    }

    public class TestUpdatableCls : IUpdatableObject
    {

        [JsonProperty("ssg_propInt")]
        public int PropInt { get; set; }

        [JsonProperty("ssg_propstring")]
        public string PropString { get; set; }

        [JsonProperty("ssg_propnullabledatetime")]
        public DateTime? PropNullableDateTime { get; set; }

        [JsonProperty("ssg_propbool")]
        public bool PropBool { get; set; }

        [JsonProperty("ssg_phonenumber")]
        [CompareOnlyNumber]
        public string Phone { get; set; }

        [JsonProperty("ssg_ignored")]
        [UpdateIgnore]
        public string Ignored { get; set; }

        [UpdateIgnore]
        [JsonProperty("ssg_propguid")]
        public Guid PropGuid { get; set; }

        [UpdateIgnore]
        public bool Updated { get; set; }

        public string NoJsonProperty { get; set; }
    }

}
