using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uGCapture
{
    public class SetCaptureStateMessage : Message
    {
        public bool running
        {
            get;
            set;
        }
        public bool init
        {
            get;
            set;
        }

        public SetCaptureStateMessage(Receiver s, bool run, bool ini = false)
            : base(s) { running = run; init = ini; }

        public override void execute(Receiver r)
        {          
            r.exSetCaptureState(r, this);
        }
    }
}
