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
        private Timer ticker = null;
        private int FRAME_TIME = 500;
        
        public ReceiverController() : base()
        {
            ticker = new Timer(FRAME_TIME);
            ticker.Elapsed += new ElapsedEventHandler(DoFrame);
            ticker.Enabled = true;
        }

        public abstract void DoFrame(object source, ElapsedEventArgs e);
    }
}
