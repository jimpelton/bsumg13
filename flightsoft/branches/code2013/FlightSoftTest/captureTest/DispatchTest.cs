using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using uGCapture;

namespace captureTest
{
    [TestFixture]
    public class DispatchTest
    {

        [Test]
        public void SimpleMessageDispatchTest()
        {
            TinyMockReceiver tmr = new TinyMockReceiver("tmr");
            tmr.init();
            TinyMockSender sender = new TinyMockSender("tinySender");
            sender.SendTinyMockMessage(sender);

            //give the message time to queue up and be delivered.
            Thread.Sleep(10);
            Assert.AreEqual(1, tmr.MsgCount);

            // kick the thread out of the execute loop.
            tmr.IsReceiving = false;
            Dispatch.Instance().CleanUpMessageThreads();
        }
    }
}
