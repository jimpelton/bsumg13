using System;
using System.Timers;


namespace uGCapture
{
    class UPSController : ReceiverController
    {
        public UPSController(BufferPool<byte> bp) : base(bp)
        {

        }

        public override void DoFrame(object source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void init()
        {
            throw new NotImplementedException();
        }
    }
}
