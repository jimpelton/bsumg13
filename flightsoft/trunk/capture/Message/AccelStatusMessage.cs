using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class  AccelStatusMessage : Message
    {
        private StatusStr state = StatusStr.STAT_ERR;
        public  StatusStr getState() { return state; }

        public AccelStatusMessage(Receiver s, StatusStr nstate)
            : base(s)
        {
            state = nstate;
        }

        AccelStatusMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exAccelStatusMessage(r, this);
        }
    }
}
