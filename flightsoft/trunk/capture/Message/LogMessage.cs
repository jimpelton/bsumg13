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

        public Status Status
        {
            get { return _status; }
        }
        private Status _status;

        public ErrStr ErrorString
        {
            get { return estr; }
        }
        private ErrStr estr;

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

        public LogMessage(Receiver sender, Status str, ErrStr er)
            : base(sender)
        {
            estr = er;
            _status = str;
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
