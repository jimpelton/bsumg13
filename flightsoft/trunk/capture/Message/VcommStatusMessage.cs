using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class VcommStatusMessage : Message
    {
        private StatusStr state = StatusStr.STAT_ERR;
        public  StatusStr getState() { return state; }

        public VcommStatusMessage(Receiver s, StatusStr nstate)
            : base(s)
        {
            state = nstate;
        }

        VcommStatusMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exVcommStatusMessage(r, this);
        }
    }
}
