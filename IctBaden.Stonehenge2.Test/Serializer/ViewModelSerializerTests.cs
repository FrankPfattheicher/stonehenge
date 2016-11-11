using System;
using IctBaden.Stonehenge2.ViewModel;
using NUnit.Framework;

namespace IctBaden.Stonehenge2.Test.Serializer
{
    [TestFixture]
    public class ViewModelSerializerTests
    {
        [Test]
        public void SimpleClassSerializonShouldWork()
        {
            var model = new SimpleClass
            {
                Integer = 5,
                Floatingpoint = 1.23,
                Text = "test",
                PrivateText = "invisible",
                Timestamp = new DateTime(2016, 11, 11, 12, 13, 14, DateTimeKind.Utc)
            };

            var json = JsonSerializer.SerializeObjectString(null, model);

            // public properties - not NULL
            Assert.IsTrue(json.Contains("Integer"));
            Assert.IsTrue(json.Contains("5"));

            Assert.IsTrue(json.Contains("Boolean"));
            Assert.IsTrue(json.Contains("false"));

            Assert.IsTrue(json.Contains("Floatingpoint"));
            Assert.IsTrue(json.Contains("1.23"));

            Assert.IsTrue(json.Contains("Text"));
            Assert.IsTrue(json.Contains("test"));

            Assert.IsTrue(json.Contains("Timestamp"));
            Assert.IsTrue(json.Contains("2016-11-11T12:13:14Z"));

            // private fields
            Assert.IsFalse(json.Contains("PrivateText"));
            Assert.IsFalse(json.Contains("invisible"));
        }

        [Test]
        public void StringsIncludingNewlineShouldBeEscaped()
        {
            var model = new SimpleClass
            {
                Text = "line1" + Environment.NewLine + "line2"
            };

            var json = JsonSerializer.SerializeObjectString(null, model);

            Assert.IsTrue(json.Contains("\\n"));
        }

        [Test]
        public void SerializerShouldRespectAttributes()
        {
            
        }

        [Test]
        public void SerializerShouldRespectCustomSerializers()
        {

        }

    }
}
