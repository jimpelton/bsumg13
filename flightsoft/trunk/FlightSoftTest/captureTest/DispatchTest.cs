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
        internal class TinyMockMessage : Message
        {
            public TinyMockMessage(Receiver sender) : base(sender)
            {
            }

            public override void execute(Receiver r)
            {
                r.exTest(r, this);
            }
        }

        internal class TinyMockReceiver : Receiver
        {
            
            public TinyMockReceiver(string id, bool receiving = true) 
                : base(id, receiving)
            {
            }

            public void init()
            {
                dp.Register(this);
            }

            private int msgCount = 0;
            public int MsgCount
            {
                get { return msgCount; }
                set { msgCount = value; }
            }

            public override void exTest(Receiver r, Message m)
            {
                MsgCount++;
                Console.WriteLine("Received! " + MsgCount);
            }
        }

        internal class TinyMockSender : Receiver
        {
            public TinyMockSender(string id, bool receiving = true) : base(id, receiving)
            {
            }
            
            public void SendTinyMockMessage(Receiver from)
            {
                dp.Broadcast(new TinyMockMessage(from));
            }

        }

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
            Dispatch.Instance().CleanUpThreads();
        }
    }
}
