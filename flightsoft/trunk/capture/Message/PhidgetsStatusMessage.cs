﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uGCapture
{
    public class PhidgetsStatusMessage : Message
    {
        private StatusStr state = StatusStr.STAT_ERR;
        public  StatusStr getState() { return state; }

        public PhidgetsStatusMessage(Receiver s, StatusStr nstate)
            : base(s)
        {
            state = nstate;
        }

        PhidgetsStatusMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exPhidgetsStatusMessage(r, this);
        }
    }
}
