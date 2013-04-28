using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace uGCapture
{
    /// <summary>
    /// The beginnings of a job scheduler.
    /// </summary>
    class Scheduler : Receiver
    {

        Timer baseClock;
        private DateTime lastBeat;
        public Scheduler(string id, bool receiving = true) 
            : base(id, receiving)
        {
            baseClock = new Timer();
            baseClock.Enabled = true;
            baseClock.Interval = 10;
            //baseClock.SynchronizingObject = null;
            baseClock.Elapsed += beat;
        }

        private void beat(object sender, ElapsedEventArgs e)
        {
            int dt = (DateTime.Now - lastBeat).Milliseconds;
            
            if (dt >= 500)
            {
                dp.Broadcast(new HeartBeatMessage(this));
            }
            if (dt >= 10)
            {
                dp.Broadcast(new AccumulateMessage(this));
            }
            lastBeat = DateTime.Now;
        }
    }
}
