using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.IO;

namespace RawSerialize.Test
{
    [TestClass]
    public class Tests
    {
        internal TestStruct TestStructInstance = new TestStruct(true, 123, 80543, 652145.903);
        internal byte[] TestStructInstanceData = new byte[] { 1, 0, 0, 0, 123, 0, 0, 0, 159, 58, 1, 0, 0, 0, 0, 0, 25, 4, 86, 206, 227, 230, 35, 65 };

        [TestMethod]
        public void ByteArraySerializationTest()
        {
            byte[] serialized = RawSerializer.GetRawData<TestStruct>(this.TestStructInstance);
            Assert.IsTrue(Enumerable.SequenceEqual<byte>(serialized, this.TestStructInstanceData));
        }

        [TestMethod]
        public void ByteArrayDeserializationTest()
        {
            TestStruct deserialized = RawSerializer.GetStructFromRawData<TestStruct>(this.TestStructInstanceData);
            Assert.AreEqual<TestStruct>(deserialized, this.TestStructInstance);
        }



        [TestMethod]
        public void StreamSerializationTest()
        {
            using (MemoryStream testStream = new MemoryStream())
            {
                RawSerializer.WriteRawData(testStream, this.TestStructInstance);
                byte[] serialized = testStream.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual<byte>(serialized, this.TestStructInstanceData));
            }
        }

        [TestMethod]
        public void StreamDeserializationTest()
        {
            using (MemoryStream testStream = new MemoryStream(this.TestStructInstanceData))
            {
                TestStruct deserialized = RawSerializer.ReadStructFromRawData<TestStruct>(testStream);
                Assert.AreEqual<TestStruct>(deserialized, this.TestStructInstance);
            }
        }



        [TestMethod]
        public void BinaryWriterSerializationTest()
        {
            using (MemoryStream testStream = new MemoryStream())
            using (BinaryWriter testWriter = new BinaryWriter(testStream))
            {
                RawSerializer.WriteRawData(testWriter, this.TestStructInstance);
                byte[] serialized = testStream.ToArray();
                Assert.IsTrue(Enumerable.SequenceEqual<byte>(serialized, this.TestStructInstanceData));
            }
        }

        [TestMethod]
        public void BinaryReaderDeserializationTest()
        {
            using (MemoryStream testStream = new MemoryStream(this.TestStructInstanceData))
            using (BinaryReader testWriter = new BinaryReader(testStream))
            {
                TestStruct deserialized = RawSerializer.ReadStructFromRawData<TestStruct>(testStream);
                Assert.AreEqual<TestStruct>(deserialized, this.TestStructInstance);
            }
        }
    }
}
