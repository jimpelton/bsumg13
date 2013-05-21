using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uGCapture;

namespace uGCapture
{
    public class LogMessage : Message
    {

        public long time;
        public String message;
        public int severity;

        public LogMessage(Receiver sender, String m) 
            : base(sender)
        {
            time = DateTime.Now.Ticks;
            message = m;
            severity = 0;
        }

        public LogMessage(Receiver sender, String m, int s) 
            : base(sender)
        {
            time = DateTime.Now.Ticks;
            message = m;
            severity = s;
        }

        public override void execute(Receiver r)
        {
            r.exLogMessage(r, this);
        }

        public override string ToString()
        {
            return message;
        }
    }
}
