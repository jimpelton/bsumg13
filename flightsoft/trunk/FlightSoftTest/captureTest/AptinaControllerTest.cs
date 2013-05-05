using System;
using NUnit.Framework;
using uGCapture;
using System.Threading;

namespace CaptureTest
{
    [TestFixture]
    public class AptinaControllerTest
    {
        [Test]
        public void InitMidlibTest()
        {
            BufferPool<byte> bp = new BufferPool<byte>(10, 16777216);
            AptinaController ac = new AptinaController(bp, "Ac1");
            ac.Initialize();

            Thread t1 = new Thread(() => AptinaController.go(ac));
            t1.Start();
        }
    }
}
