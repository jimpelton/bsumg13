// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-05-23                                                                      
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uGCapture;

namespace gui
{
    class GUIUpdater2 : Receiver
    {
        public delegate void debug_output(Message m);

        public debug_output DebugOutput { get; set; }

        public void BroadcastCmd(CommandStr m)
        {
            dp.Broadcast(new CommandMessage(this, m));
        }

        public void BroadcastLogError(string s)
        {
            dp.BroadcastLog(this, s, Status.STAT_ERR);
        }

        public GUIUpdater2(string id, bool receiving = true, bool executing = false) 
            : base(id, receiving, executing)
        {}

        public override void exLogMessage(Receiver r, Message m)
        {
            LogMessage lm = m as LogMessage;
            if (lm != null)
                DebugOutput(lm);
        }

        


    }
}
