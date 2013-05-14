using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class NI6008StatusMessage : Message
    {
        private StatusStr state = StatusStr.STAT_ERR;
        public  StatusStr getState() { return state; }

        public NI6008StatusMessage(Receiver s, StatusStr nstate)
            : base(s)
        {
            state = nstate;
        }

        NI6008StatusMessage(Receiver s)
            : base(s) { ; }

        public override void execute(Receiver r)
        {
            r.exNI6008StatusMessage(r, this);
        }
    }
}
