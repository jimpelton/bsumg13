using System;
using System.Timers;


namespace uGCapture
{
    class UPSController : ReceiverController
    {
        public UPSController(BufferPool<byte> bp, string id, bool receiving = true, int frame_time = 500) : base(bp, id, receiving, frame_time)
        {
        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void exHeartBeatMessage(Receiver r, Message m)
        {
            base.exHeartBeatMessage(r, m);
            throw new NotImplementedException();

        }

        protected override bool init()
        {
            return false;
        }
    }
}
