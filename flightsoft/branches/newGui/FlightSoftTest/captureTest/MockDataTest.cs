using System;
using System.Linq;
using NUnit.Framework;
using uGCapture;

namespace captureTest
{
    [TestFixture]
    public class MockDataTest
    {
        private MockController[] mockControllers;
        private BufferPool<byte> bp;
        private Dispatch dp;

        [SetUp]
        void setup()
        {
            bp = new BufferPool<byte>(10, (int) Math.Pow(2,24));
            dp = Dispatch.Instance();

            mockControllers = new MockController[7];
            for (int i = 0; i < mockControllers.Length; i++)
            {
                mockControllers[i] = new MockController(bp,"MockController"+i);
            }

            foreach (MockController mc in mockControllers)
            {
                mc.Initialize();
                dp.Register(mc);
            }
        }


        [Test]
        public void TestMethod1()
        {
            foreach (MockController mc in mockControllers)
            {
            }


        }
    }
}
