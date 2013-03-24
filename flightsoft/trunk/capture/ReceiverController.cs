using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace uGCapture.Controller
{
    public abstract class ReceiverController : Receiver
    {
        private Timer ticker;
        private readonly int FRAME_TIME;
        private BufferPool<byte> bufferPool;

        //public ReceiverController() : base() { }
        
        public ReceiverController(BufferPool<byte> bp, int frame_time=500) : base()
        {
            bufferPool = bp;
            FRAME_TIME = frame_time;
            ticker = new Timer(frame_time);
            ticker.Elapsed += new ElapsedEventHandler(DoFrame);
            //ticker.Enabled = true;
        }

        /// <summary>
        /// A receivercontroller must implement init() which will initialize
        /// hardware and other objects when called.
        /// </summary>
        public abstract void init(); 

        /// <summary>
        /// Called after every FRAME_TIME interval.
        /// </summary>
        /// <param name="source">The sender of the event</param>
        /// <param name="e"></param>
        public abstract void DoFrame(object source, ElapsedEventArgs e);
    }
}
