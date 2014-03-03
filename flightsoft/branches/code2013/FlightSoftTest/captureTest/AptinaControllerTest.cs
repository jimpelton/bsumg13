using System;
using NUnit.Framework;
using uGCapture;
using System.Threading;

namespace captureTest
{
    [TestFixture]
    public class AptinaControllerTest
    {
        [Test]
        public void InitMidlibTest()
        {
            const int pool_size_bytes = 16777216;
            BufferPool<byte> bp = new BufferPool<byte>(10, pool_size_bytes);
            AptinaController ac = new AptinaController(bp, "Ac1");
            Assert.IsTrue(ac.Initialize());
            Assert.AreEqual(ac.Errno, ErrStr.INIT_OK_APTINA);

            Thread t1 = new Thread(() => AptinaController.go(ac));
            t1.Start();
            
            Assert.IsTrue(t1.IsAlive);
            
        }
    }
}
