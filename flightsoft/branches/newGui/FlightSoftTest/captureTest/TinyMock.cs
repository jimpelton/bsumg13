// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-09                                                                      
// ******************************************************************************

using System;
using uGCapture;

namespace captureTest
{
    public class TinyMockMessage : Message
    {
        public TinyMockMessage(Receiver sender) : base(sender)
        {
        }

        public override void execute(Receiver r)
        {
            r.exTest(r, this);
        }
    }

    public class TinyMockReceiver : Receiver
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

    public class TinyMockSender : Receiver
    {
        public TinyMockSender(string id, bool receiving = true) : base(id, receiving)
        {
        }

        public void SendTinyMockMessage(Receiver from)
        {
            dp.Broadcast(new TinyMockMessage(from));
        }
    }
}