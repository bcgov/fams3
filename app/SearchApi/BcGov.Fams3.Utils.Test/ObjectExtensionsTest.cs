using BcGov.Fams3.Utils.Object;
using NUnit.Framework;
using System;

namespace BcGov.Fams3.Utils.Test
{
    public class ObjectExtensionsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void clone_successfuly()
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
            TestUpdatableCls cloned = testUpdatableCls1.Clone();
            Assert.AreEqual(new DateTime(1999, 1, 1), cloned.PropDateTime);
            Assert.AreEqual(2, cloned.PropInt);
            Assert.AreEqual("test1", cloned.PropString);
            Assert.AreEqual(null, cloned.PropNullableDateTime);
            Assert.AreEqual(null, cloned.PropNullableInt);
            Assert.AreEqual(guid, cloned.PropGuid);
            Assert.AreEqual(true, cloned.PropBool);
            Assert.AreEqual(4, cloned.Child.PropInt);
        }

    }

    public class TestUpdatableCls
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

    public class ChildTestUpdatableCls
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
