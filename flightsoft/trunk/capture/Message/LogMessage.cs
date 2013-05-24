﻿// ******************************************************************************
//  BSU Microgravity Team 2013                                                 
//  In-Flight Data Capture Software                                            
//  Date: 2013-03-06                                                                      
// ******************************************************************************


using System;
using System.Collections.Generic;

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

        public LogMessage(Receiver sender, ErrStr er, Status str)
            : base(sender)
        {
            estr = er;
            message = Str.GetErrStr(er);
            _status = str;
        }

        public LogMessage(Receiver sender, string m, Status stat)
            : base(sender)
        {
            message = m;
            _status = stat;
        }

        public override void execute(Receiver r)
        {
            r.exLogMessage(r, this);
        }

        public override string ToString()
        {
            return message;
        }

        public override void AddSpecificReceiver(ReceiverIdPair r)
        {
        }

        public override IList<ReceiverIdPair> GetSpecificReceivers()
        {
            return null;
        }
    }
}
