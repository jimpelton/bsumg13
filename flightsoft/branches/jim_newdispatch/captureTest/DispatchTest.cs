using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uGCapture;

namespace captureTest
{
    [TestClass]
    public class DispatchTest
    {



        [TestMethod]
        public void SimpleMessageDispatchTest()
        {
            BufferPool<byte> bp = new BufferPool<byte>(10,10);
            MockController mc1 = new MockController(bp, "Mc1");
            MockController mc2 = new MockController(bp, "Mc2");
            

        }
    }
}
