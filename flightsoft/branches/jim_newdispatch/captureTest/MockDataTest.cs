using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uGCapture;

namespace CaptureTest
{
    [TestClass]
    public class MockDataTest
    {
        private MockController[] mockControllers;
        private BufferPool<byte> bp;
        private Dispatch dp;

        void setup()
        {

            bp = new BufferPool<byte>(10, (int) Math.Pow(2,24));
            dp = Dispatch.Instance();

            mockControllers = new MockController[7];
            for (int i = 0; i < mockControllers.Length; i++)
            {
                mockControllers[i] = new MockController(bp);
            }

            int mcidx = 0;
            foreach (MockController mc in mockControllers)
            {
                mc.init();
                dp.Register(mc, "mc"+mcidx);
                ++mcidx;
            }

        }


        [TestMethod]
        public void TestMethod1()
        {
            setup();
            foreach (MockController mc in mockControllers)
            {
                mc.TickerEnabled = true;
            }


        }
    }
}
