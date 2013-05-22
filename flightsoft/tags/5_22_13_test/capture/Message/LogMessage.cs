// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-06                                                                      
// ******************************************************************************


using System;

namespace uGCapture
{
    public class LogMessage : Message
    {
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
            message = m;
            severity = 0;
        }

        public LogMessage(Receiver sender, String m, int s)
            : base(sender)
        {
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
