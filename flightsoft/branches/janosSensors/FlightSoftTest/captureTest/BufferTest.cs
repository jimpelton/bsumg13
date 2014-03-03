using System;
using NUnit.Framework;
using uGCapture;
namespace captureTest
{
    [TestFixture]
    public class BufferTest
    {
        [Test]
        public void BufferTest1()
        {
            Buffer<byte> buff = new Buffer<byte>(10);
            byte[] b = new byte[10];
            Random r = new Random();
            r.NextBytes(b);
            buff.setData(b, BufferType.USHORT_IMAGE405);
            Assert.IsTrue(b.Length == (int)buff.CapacityUtilization);
        }

        [Test]
        public void BufferTest_SetDataTest()
        {
            Buffer<byte> buff = new Buffer<byte>(10);
            byte[] b = new byte[10];
            new Random().NextBytes(b);
            buff.setData(b, BufferType.USHORT_IMAGE405);
            
            byte[] actual = buff.Data;
            Assert.IsTrue(BufferType.USHORT_IMAGE405 == buff.Type);
            Assert.IsTrue(b.Length == actual.Length);
            CollectionAssert.AreEqual(b, actual);
        }
 
        [Test]
        public void BufferTest_CopyConstructorTest()
        {
            Buffer<byte> buff = new Buffer<byte>(10);

            byte[] b = new byte[10];
            new Random().NextBytes(b);
            buff.setData(b, BufferType.USHORT_IMAGE405);

            Buffer<byte> buffcopy = new Buffer<byte>(buff);
            Assert.AreEqual(buff.Type, buffcopy.Type);                     //type
            Assert.AreEqual(buff.Text, buffcopy.Text);                     //text
            Assert.AreEqual(buff.Length, buffcopy.Length);                 //length
            CollectionAssert.AreEqual(buff.Data, buffcopy.Data);           //data contents
        }
    }
}
