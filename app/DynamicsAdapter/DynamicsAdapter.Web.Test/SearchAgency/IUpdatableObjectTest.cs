using BcGov.Fams3.Utils.Object;
using Fams3Adapter.Dynamics.Update;
using NUnit.Framework;
using System;

namespace DynamicsAdapter.Web.Test.SearchAgency
{
    public class IUpdatableObjectTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void same_data_updated_should_be_false()
        {
            Guid guid = Guid.NewGuid();
            TestUpdatableCls testUpdatableCls1 = new TestUpdatableCls()
            {
                PropDateTime = new DateTime(1999, 1, 1),
                PropInt = 2,
                PropString = "test1",
                PropNullableDateTime = null,
                PropNullableInt = null,
                PropGuid = guid,
                PropBool = true,
                Child = new ChildTestUpdatableCls()
                {
                    PropInt = 4
                }
            };
            TestUpdatableCls testUpdatableCls2 = testUpdatableCls1.Clone();
            TestUpdatableCls result = testUpdatableCls1.MergeUpdates(testUpdatableCls2);
            Assert.AreEqual(false, result.Updated);
            Assert.AreEqual(false, result.Child.Updated);
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
                PropDateTime = new DateTime(1999, 1, 1),
                PropInt = 2,
                PropString = "test1",
                PropNullableDateTime = new DateTime(2000, 1, 1),
                PropNullableInt = null,
                PropBool = true,
                Child = new ChildTestUpdatableCls()
                {
                    PropInt = 3
                }
            };
            TestUpdatableCls testUpdatableCls2 = new TestUpdatableCls()
            {
                PropDateTime = new DateTime(1999, 1, 1),
                PropInt = 2,
                PropString = "test1",
                PropNullableDateTime = null,
                PropNullableInt = 3,
                PropBool = false,
                Child = new ChildTestUpdatableCls()
                {
                    PropInt = 4
                }
            };
            TestUpdatableCls result = testUpdatableCls1.MergeUpdates(testUpdatableCls2);
            Assert.AreEqual(true, result.Updated);
            Assert.AreEqual(new DateTime(2000, 1, 1), result.PropNullableDateTime);
            Assert.AreEqual(false, result.PropBool);
            Assert.AreEqual(3, result.PropNullableInt);
            Assert.AreEqual(true, result.Child.Updated);
            Assert.AreEqual(4, result.Child.PropInt);
        }
    }

    public class TestUpdatableCls : IUpdatableObject
    {
        public int PropInt { get; set; }
        public int? PropNullableInt { get; set; }
        public string PropString { get; set; }
        public DateTime PropDateTime { get; set; }
        public DateTime? PropNullableDateTime { get; set; }
        public bool PropBool { get; set; }
        public bool Updated { get; set; }
        public Guid PropGuid { get; set; }
        public ChildTestUpdatableCls Child { get; set; }
    }

    public class ChildTestUpdatableCls : IUpdatableObject
    {
        public int PropInt { get; set; }
        public int? PropNullableInt { get; set; }
        public string PropString { get; set; }
        public DateTime PropDateTime { get; set; }
        public DateTime? PropNullableDateTime { get; set; }
        public bool PropBool { get; set; }
        public bool Updated { get; set; }
    }
}
