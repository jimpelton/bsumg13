// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-04-27                                                                      
// ******************************************************************************
using System;
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
        private DateTime lastAccum;

        public Scheduler(string id, bool receiving = true) 
            : base(id, receiving)
        {
            baseClock = new Timer();
            baseClock.Enabled = true;
            baseClock.Interval = 10;
            baseClock.Elapsed += beat;
        }

        private void beat(object sender, ElapsedEventArgs e)
        {
            int dt = (DateTime.Now - lastBeat).Milliseconds;
            
            if (dt >= 450)
            {
                dp.Broadcast(new HeartBeatMessage(this));
                lastBeat = DateTime.Now;
            }
            if (dt >= 10)
            {
                dp.Broadcast(new AccumulateMessage(this));
                lastAccum = DateTime.Now;
            }
        }

        public override void exReceiverCleanUpMessage(Receiver r, Message m)
        {
	    // overriden to do nothing (we don't want the parent method's behavior 
	    // in the Scheduler.
        }
    }
}
